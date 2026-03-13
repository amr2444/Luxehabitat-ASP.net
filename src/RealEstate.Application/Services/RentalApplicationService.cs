using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using RealEstate.Application.DTOs.Notifications;
using RealEstate.Application.DTOs.RentalApplications;
using RealEstate.Application.Exceptions;
using RealEstate.Application.Interfaces.Persistence;
using RealEstate.Application.Interfaces.Services;
using RealEstate.Domain.Enums;

namespace RealEstate.Application.Services;

public sealed class RentalApplicationService(
    IApplicationDbContext context,
    INotificationService notificationService,
    IPropertyService propertyService) : IRentalApplicationService
{
    public async Task<RentalApplicationDto> SubmitAsync(string tenantUserId, SubmitRentalApplicationRequest request, CancellationToken cancellationToken = default)
    {
        var tenant = await context.TenantProfiles.FirstOrDefaultAsync(x => x.UserId == tenantUserId, cancellationToken)
            ?? throw new NotFoundAppException("Profil locataire introuvable.");

        var property = await context.Properties
            .Include(x => x.AgentProfile)
            .ThenInclude(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == request.PropertyId, cancellationToken)
            ?? throw new NotFoundAppException("Bien introuvable.");

        if (property.Status != PropertyStatus.Published || property.ListingType != ListingType.Rent)
        {
            throw new ValidationAppException("La candidature est autorisee uniquement sur une annonce locative publiee.");
        }

        var hasActiveApplication = await context.RentalApplications.AnyAsync(
            x => x.PropertyId == property.Id &&
                 x.TenantProfileId == tenant.Id &&
                 x.Status != RentalApplicationStatus.Rejected &&
                 x.Status != RentalApplicationStatus.Withdrawn,
            cancellationToken);

        if (hasActiveApplication)
        {
            throw new ConflictAppException("Une candidature active existe deja pour ce bien.");
        }

        var application = new Domain.Entities.RentalApplication
        {
            PropertyId = property.Id,
            TenantProfileId = tenant.Id,
            AgentProfileId = property.AgentProfileId,
            CoverLetter = request.CoverLetter?.Trim(),
            RequestedMoveInDateUtc = request.RequestedMoveInDateUtc,
            SubmittedAtUtc = DateTime.UtcNow,
            Status = RentalApplicationStatus.Submitted,
            CreatedByUserId = tenantUserId
        };

        await context.RentalApplications.AddAsync(application, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        await notificationService.CreateAsync(new CreateNotificationRequest(
            property.AgentProfile.UserId,
            NotificationType.Application,
            "Nouvelle candidature locative",
            $"Une nouvelle candidature a ete soumise pour '{property.Title}'.",
            "/Agent/Applications"), cancellationToken);

        return await context.RentalApplications
            .AsNoTracking()
            .Where(x => x.Id == application.Id)
            .Select(MapApplication())
            .FirstAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<RentalApplicationDto>> GetAgentApplicationsAsync(string agentUserId, CancellationToken cancellationToken = default)
    {
        var agent = await context.AgentProfiles.FirstOrDefaultAsync(x => x.UserId == agentUserId, cancellationToken)
            ?? throw new NotFoundAppException("Profil agent introuvable.");

        return await context.RentalApplications
            .AsNoTracking()
            .Where(x => x.AgentProfileId == agent.Id)
            .OrderByDescending(x => x.SubmittedAtUtc)
            .Select(MapApplication())
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<RentalApplicationDto>> GetTenantApplicationsAsync(string tenantUserId, CancellationToken cancellationToken = default)
    {
        var tenant = await context.TenantProfiles.FirstOrDefaultAsync(x => x.UserId == tenantUserId, cancellationToken)
            ?? throw new NotFoundAppException("Profil locataire introuvable.");

        return await context.RentalApplications
            .AsNoTracking()
            .Where(x => x.TenantProfileId == tenant.Id)
            .OrderByDescending(x => x.SubmittedAtUtc)
            .Select(MapApplication())
            .ToListAsync(cancellationToken);
    }

    public async Task ChangeStatusAsync(string agentUserId, Guid applicationId, RentalApplicationStatus status, CancellationToken cancellationToken = default)
    {
        var agent = await context.AgentProfiles.FirstOrDefaultAsync(x => x.UserId == agentUserId, cancellationToken)
            ?? throw new NotFoundAppException("Profil agent introuvable.");

        var application = await context.RentalApplications
            .Include(x => x.Property)
            .FirstOrDefaultAsync(x => x.Id == applicationId, cancellationToken)
            ?? throw new NotFoundAppException("Candidature introuvable.");

        if (application.AgentProfileId != agent.Id)
        {
            throw new ForbiddenAppException("Vous ne pouvez pas modifier une candidature qui ne vous appartient pas.");
        }

        ValidateTransition(application.Status, status, application.Property.Status);

        application.Status = status;
        application.ReviewedAtUtc = DateTime.UtcNow;
        application.UpdatedAtUtc = DateTime.UtcNow;
        application.UpdatedByUserId = agentUserId;

        await context.SaveChangesAsync(cancellationToken);

        if (status == RentalApplicationStatus.Accepted)
        {
            await propertyService.MarkAsRentedAsync(agentUserId, application.PropertyId, cancellationToken);
        }

        var tenantUserId = await context.TenantProfiles
            .Where(x => x.Id == application.TenantProfileId)
            .Select(x => x.UserId)
            .FirstAsync(cancellationToken);

        await notificationService.CreateAsync(new CreateNotificationRequest(
            tenantUserId,
            NotificationType.Application,
            "Mise a jour de candidature",
            $"Le statut de votre candidature est maintenant '{status}'.",
            "/Tenant/Applications"), cancellationToken);
    }

    public async Task<RentalApplicationDto> GetAgentApplicationAsync(string agentUserId, Guid applicationId, CancellationToken cancellationToken = default)
    {
        var agent = await context.AgentProfiles.FirstOrDefaultAsync(x => x.UserId == agentUserId, cancellationToken)
            ?? throw new NotFoundAppException("Profil agent introuvable.");

        return await context.RentalApplications
            .AsNoTracking()
            .Where(x => x.Id == applicationId && x.AgentProfileId == agent.Id)
            .Select(MapApplication())
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundAppException("Candidature introuvable.");
    }

    public async Task<RentalApplicationDto> GetTenantApplicationAsync(string tenantUserId, Guid applicationId, CancellationToken cancellationToken = default)
    {
        var tenant = await context.TenantProfiles.FirstOrDefaultAsync(x => x.UserId == tenantUserId, cancellationToken)
            ?? throw new NotFoundAppException("Profil locataire introuvable.");

        return await context.RentalApplications
            .AsNoTracking()
            .Where(x => x.Id == applicationId && x.TenantProfileId == tenant.Id)
            .Select(MapApplication())
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundAppException("Candidature introuvable.");
    }

    private static Expression<Func<Domain.Entities.RentalApplication, RentalApplicationDto>> MapApplication()
        => x => new RentalApplicationDto(
            x.Id,
            x.PropertyId,
            x.Property.Title,
            x.Status,
            x.RequestedMoveInDateUtc,
            x.SubmittedAtUtc);

    private static void ValidateTransition(RentalApplicationStatus current, RentalApplicationStatus next, PropertyStatus propertyStatus)
    {
        if (propertyStatus != PropertyStatus.Published && next == RentalApplicationStatus.Accepted)
        {
            throw new ValidationAppException("Une candidature ne peut etre acceptee que si la propriete est disponible.");
        }

        if (current == RentalApplicationStatus.Accepted)
        {
            throw new ValidationAppException("Une candidature deja acceptee ne peut plus changer de statut.");
        }

        if (current == RentalApplicationStatus.Rejected && next == RentalApplicationStatus.Accepted)
        {
            throw new ValidationAppException("Une candidature rejetee ne peut pas etre acceptee.");
        }
    }
}
