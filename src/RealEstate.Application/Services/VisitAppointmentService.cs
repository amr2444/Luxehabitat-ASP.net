using Microsoft.EntityFrameworkCore;
using RealEstate.Application.DTOs.Notifications;
using RealEstate.Application.DTOs.Visits;
using RealEstate.Application.Exceptions;
using RealEstate.Application.Interfaces.Persistence;
using RealEstate.Application.Interfaces.Services;
using RealEstate.Domain.Enums;

namespace RealEstate.Application.Services;

public sealed class VisitAppointmentService(
    IApplicationDbContext context,
    INotificationService notificationService) : IVisitAppointmentService
{
    public async Task<VisitAppointmentDto> RequestAsync(string tenantUserId, RequestVisitAppointmentRequest request, CancellationToken cancellationToken = default)
    {
        var tenant = await context.TenantProfiles.FirstOrDefaultAsync(x => x.UserId == tenantUserId, cancellationToken)
            ?? throw new NotFoundAppException("Profil locataire introuvable.");

        var property = await context.Properties
            .Include(x => x.AgentProfile)
            .ThenInclude(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == request.PropertyId, cancellationToken)
            ?? throw new NotFoundAppException("Bien introuvable.");

        if (property.Status != PropertyStatus.Published)
        {
            throw new ValidationAppException("Les visites ne peuvent etre demandees que sur des biens publies.");
        }

        if (request.ScheduledAtUtc <= DateTime.UtcNow)
        {
            throw new ValidationAppException("La visite doit etre planifiee dans le futur.");
        }

        var visit = new Domain.Entities.VisitAppointment
        {
            PropertyId = property.Id,
            TenantProfileId = tenant.Id,
            AgentProfileId = property.AgentProfileId,
            ScheduledAtUtc = request.ScheduledAtUtc,
            Notes = request.Notes?.Trim(),
            Status = VisitAppointmentStatus.Pending,
            CreatedByUserId = tenantUserId
        };

        await context.VisitAppointments.AddAsync(visit, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        await notificationService.CreateAsync(new CreateNotificationRequest(
            property.AgentProfile.UserId,
            NotificationType.Visit,
            "Nouvelle demande de visite",
            $"Une visite a ete demandee pour '{property.Title}'.",
            "/Agent/Visits"), cancellationToken);

        return await ProjectVisitForAgentAsync(visit.Id, cancellationToken);
    }

    public async Task<IReadOnlyCollection<VisitAppointmentDto>> GetAgentVisitsAsync(string agentUserId, CancellationToken cancellationToken = default)
    {
        var agent = await context.AgentProfiles.FirstOrDefaultAsync(x => x.UserId == agentUserId, cancellationToken)
            ?? throw new NotFoundAppException("Profil agent introuvable.");

        return await context.VisitAppointments
            .AsNoTracking()
            .Where(x => x.AgentProfileId == agent.Id)
            .OrderBy(x => x.ScheduledAtUtc)
            .Select(x => new VisitAppointmentDto(
                x.Id,
                x.PropertyId,
                x.Property.Title,
                x.ScheduledAtUtc,
                x.Status,
                x.Notes,
                $"{x.TenantProfile.User.FirstName} {x.TenantProfile.User.LastName}".Trim()))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<VisitAppointmentDto>> GetTenantVisitsAsync(string tenantUserId, CancellationToken cancellationToken = default)
    {
        var tenant = await context.TenantProfiles.FirstOrDefaultAsync(x => x.UserId == tenantUserId, cancellationToken)
            ?? throw new NotFoundAppException("Profil locataire introuvable.");

        return await context.VisitAppointments
            .AsNoTracking()
            .Where(x => x.TenantProfileId == tenant.Id)
            .OrderBy(x => x.ScheduledAtUtc)
            .Select(x => new VisitAppointmentDto(
                x.Id,
                x.PropertyId,
                x.Property.Title,
                x.ScheduledAtUtc,
                x.Status,
                x.Notes,
                x.Property.AgentProfile.AgencyName))
            .ToListAsync(cancellationToken);
    }

    public async Task ConfirmAsync(string agentUserId, Guid visitId, CancellationToken cancellationToken = default)
    {
        var visit = await GetOwnedVisitAsync(agentUserId, visitId, cancellationToken);
        if (visit.Status is not (VisitAppointmentStatus.Pending or VisitAppointmentStatus.Rescheduled))
        {
            throw new ValidationAppException("Seule une visite en attente ou reprogrammee peut etre confirmee.");
        }

        visit.Status = VisitAppointmentStatus.Confirmed;
        visit.ConfirmedAtUtc = DateTime.UtcNow;
        Stamp(visit, agentUserId);
        await context.SaveChangesAsync(cancellationToken);

        await NotifyTenantAsync(visit.TenantProfileId, "Visite confirmee", "Votre demande de visite a ete confirmee.", cancellationToken);
    }

    public async Task RejectAsync(string agentUserId, Guid visitId, CancellationToken cancellationToken = default)
    {
        var visit = await GetOwnedVisitAsync(agentUserId, visitId, cancellationToken);
        if (visit.Status == VisitAppointmentStatus.Completed)
        {
            throw new ValidationAppException("Une visite terminee ne peut pas etre refusee.");
        }

        visit.Status = VisitAppointmentStatus.Rejected;
        Stamp(visit, agentUserId);
        await context.SaveChangesAsync(cancellationToken);

        await NotifyTenantAsync(visit.TenantProfileId, "Visite refusee", "Votre demande de visite n'a pas pu etre acceptee.", cancellationToken);
    }

    public async Task RescheduleAsync(string agentUserId, Guid visitId, DateTime scheduledAtUtc, CancellationToken cancellationToken = default)
    {
        if (scheduledAtUtc <= DateTime.UtcNow)
        {
            throw new ValidationAppException("La nouvelle date de visite doit etre future.");
        }

        var visit = await GetOwnedVisitAsync(agentUserId, visitId, cancellationToken);
        if (visit.Status == VisitAppointmentStatus.Completed)
        {
            throw new ValidationAppException("Une visite terminee ne peut pas etre reprogrammee.");
        }

        visit.ScheduledAtUtc = scheduledAtUtc;
        visit.Status = VisitAppointmentStatus.Rescheduled;
        Stamp(visit, agentUserId);
        await context.SaveChangesAsync(cancellationToken);

        await NotifyTenantAsync(visit.TenantProfileId, "Visite reprogrammee", "Une nouvelle date de visite vous a ete proposee.", cancellationToken);
    }

    public async Task CompleteAsync(string agentUserId, Guid visitId, CancellationToken cancellationToken = default)
    {
        var visit = await GetOwnedVisitAsync(agentUserId, visitId, cancellationToken);
        if (visit.Status != VisitAppointmentStatus.Confirmed)
        {
            throw new ValidationAppException("Seule une visite confirmee peut etre marquee comme terminee.");
        }

        visit.Status = VisitAppointmentStatus.Completed;
        Stamp(visit, agentUserId);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<VisitAppointmentDto> GetAgentVisitAsync(string agentUserId, Guid visitId, CancellationToken cancellationToken = default)
    {
        await GetOwnedVisitAsync(agentUserId, visitId, cancellationToken);
        return await ProjectVisitForAgentAsync(visitId, cancellationToken);
    }

    public async Task<VisitAppointmentDto> GetTenantVisitAsync(string tenantUserId, Guid visitId, CancellationToken cancellationToken = default)
    {
        var tenant = await context.TenantProfiles.FirstOrDefaultAsync(x => x.UserId == tenantUserId, cancellationToken)
            ?? throw new NotFoundAppException("Profil locataire introuvable.");

        return await context.VisitAppointments
            .AsNoTracking()
            .Where(x => x.Id == visitId && x.TenantProfileId == tenant.Id)
            .Select(x => new VisitAppointmentDto(
                x.Id,
                x.PropertyId,
                x.Property.Title,
                x.ScheduledAtUtc,
                x.Status,
                x.Notes,
                x.Property.AgentProfile.AgencyName))
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundAppException("Visite introuvable.");
    }

    private async Task<Domain.Entities.VisitAppointment> GetOwnedVisitAsync(string agentUserId, Guid visitId, CancellationToken cancellationToken)
    {
        var agent = await context.AgentProfiles.FirstOrDefaultAsync(x => x.UserId == agentUserId, cancellationToken)
            ?? throw new NotFoundAppException("Profil agent introuvable.");

        var visit = await context.VisitAppointments.FirstOrDefaultAsync(x => x.Id == visitId, cancellationToken)
            ?? throw new NotFoundAppException("Visite introuvable.");

        if (visit.AgentProfileId != agent.Id)
        {
            throw new ForbiddenAppException("Vous ne pouvez pas gerer une visite qui ne vous appartient pas.");
        }

        return visit;
    }

    private async Task<VisitAppointmentDto> ProjectVisitForAgentAsync(Guid visitId, CancellationToken cancellationToken)
        => await context.VisitAppointments
            .AsNoTracking()
            .Where(x => x.Id == visitId)
            .Select(x => new VisitAppointmentDto(
                x.Id,
                x.PropertyId,
                x.Property.Title,
                x.ScheduledAtUtc,
                x.Status,
                x.Notes,
                $"{x.TenantProfile.User.FirstName} {x.TenantProfile.User.LastName}".Trim()))
            .FirstAsync(cancellationToken);

    private async Task NotifyTenantAsync(Guid tenantProfileId, string title, string message, CancellationToken cancellationToken)
    {
        var tenantUserId = await context.TenantProfiles
            .Where(x => x.Id == tenantProfileId)
            .Select(x => x.UserId)
            .FirstAsync(cancellationToken);

        await notificationService.CreateAsync(new CreateNotificationRequest(
            tenantUserId,
            NotificationType.Visit,
            title,
            message,
            "/Tenant/Visits"), cancellationToken);
    }

    private static void Stamp(Domain.Entities.VisitAppointment visit, string userId)
    {
        visit.UpdatedAtUtc = DateTime.UtcNow;
        visit.UpdatedByUserId = userId;
    }
}
