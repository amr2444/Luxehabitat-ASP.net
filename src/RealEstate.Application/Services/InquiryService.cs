using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using RealEstate.Application.DTOs.Inquiries;
using RealEstate.Application.DTOs.Notifications;
using RealEstate.Application.Exceptions;
using RealEstate.Application.Interfaces.Persistence;
using RealEstate.Application.Interfaces.Services;
using RealEstate.Domain.Enums;

namespace RealEstate.Application.Services;

public sealed class InquiryService(
    IApplicationDbContext context,
    INotificationService notificationService) : IInquiryService
{
    public async Task<InquiryDto> SendAsync(string tenantUserId, SendInquiryRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.Subject) || string.IsNullOrWhiteSpace(request.Message))
        {
            throw new ValidationAppException("Le sujet et le message sont requis.");
        }

        var tenant = await context.TenantProfiles.FirstOrDefaultAsync(x => x.UserId == tenantUserId, cancellationToken)
            ?? throw new NotFoundAppException("Profil locataire introuvable.");

        var property = await context.Properties
            .Include(x => x.AgentProfile)
            .ThenInclude(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == request.PropertyId, cancellationToken)
            ?? throw new NotFoundAppException("Bien introuvable.");

        if (property.Status != PropertyStatus.Published)
        {
            throw new ValidationAppException("Les demandes d'information ne sont autorisees que sur des biens publies.");
        }

        var inquiry = new Domain.Entities.Inquiry
        {
            PropertyId = property.Id,
            TenantProfileId = tenant.Id,
            AgentProfileId = property.AgentProfileId,
            Subject = request.Subject.Trim(),
            Message = request.Message.Trim(),
            CreatedByUserId = tenantUserId
        };

        await context.Inquiries.AddAsync(inquiry, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        await notificationService.CreateAsync(new CreateNotificationRequest(
            property.AgentProfile.UserId,
            NotificationType.Inquiry,
            "Nouvelle demande d'information",
            $"Vous avez recu une demande pour le bien '{property.Title}'.",
            "/Agent/Inquiries"), cancellationToken);

        return await context.Inquiries
            .AsNoTracking()
            .Where(x => x.Id == inquiry.Id)
            .Select(MapInquiry())
            .FirstAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<InquiryDto>> GetAgentInquiriesAsync(string agentUserId, CancellationToken cancellationToken = default)
    {
        var agentProfile = await context.AgentProfiles.FirstOrDefaultAsync(x => x.UserId == agentUserId, cancellationToken)
            ?? throw new NotFoundAppException("Profil agent introuvable.");

        return await context.Inquiries
            .AsNoTracking()
            .Where(x => x.AgentProfileId == agentProfile.Id)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(MapInquiry())
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<InquiryDto>> GetTenantInquiriesAsync(string tenantUserId, CancellationToken cancellationToken = default)
    {
        var tenantProfile = await context.TenantProfiles.FirstOrDefaultAsync(x => x.UserId == tenantUserId, cancellationToken)
            ?? throw new NotFoundAppException("Profil locataire introuvable.");

        return await context.Inquiries
            .AsNoTracking()
            .Where(x => x.TenantProfileId == tenantProfile.Id)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(MapInquiry())
            .ToListAsync(cancellationToken);
    }

    public async Task ChangeStatusAsync(string agentUserId, Guid inquiryId, InquiryStatus status, CancellationToken cancellationToken = default)
    {
        var agentProfile = await context.AgentProfiles.FirstOrDefaultAsync(x => x.UserId == agentUserId, cancellationToken)
            ?? throw new NotFoundAppException("Profil agent introuvable.");

        var inquiry = await context.Inquiries
            .Include(x => x.TenantProfile)
            .ThenInclude(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == inquiryId, cancellationToken)
            ?? throw new NotFoundAppException("Demande introuvable.");

        if (inquiry.AgentProfileId != agentProfile.Id)
        {
            throw new ForbiddenAppException("Vous ne pouvez pas modifier une demande qui ne vous appartient pas.");
        }

        ValidateStatusTransition(inquiry.Status, status);

        inquiry.Status = status;
        inquiry.RespondedAtUtc = status is InquiryStatus.Closed or InquiryStatus.InProgress ? DateTime.UtcNow : inquiry.RespondedAtUtc;
        inquiry.UpdatedAtUtc = DateTime.UtcNow;
        inquiry.UpdatedByUserId = agentUserId;

        await context.SaveChangesAsync(cancellationToken);

        await notificationService.CreateAsync(new CreateNotificationRequest(
            inquiry.TenantProfile.UserId,
            NotificationType.Inquiry,
            "Mise a jour de votre demande",
            $"Le statut de votre demande est maintenant '{status}'.",
            "/Tenant/Inquiries"), cancellationToken);
    }

    public async Task<InquiryDto> GetAgentInquiryAsync(string agentUserId, Guid inquiryId, CancellationToken cancellationToken = default)
    {
        var agentProfile = await context.AgentProfiles.FirstOrDefaultAsync(x => x.UserId == agentUserId, cancellationToken)
            ?? throw new NotFoundAppException("Profil agent introuvable.");

        return await context.Inquiries
            .AsNoTracking()
            .Where(x => x.Id == inquiryId && x.AgentProfileId == agentProfile.Id)
            .Select(MapInquiry())
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundAppException("Demande introuvable.");
    }

    public async Task<InquiryDto> GetTenantInquiryAsync(string tenantUserId, Guid inquiryId, CancellationToken cancellationToken = default)
    {
        var tenantProfile = await context.TenantProfiles.FirstOrDefaultAsync(x => x.UserId == tenantUserId, cancellationToken)
            ?? throw new NotFoundAppException("Profil locataire introuvable.");

        return await context.Inquiries
            .AsNoTracking()
            .Where(x => x.Id == inquiryId && x.TenantProfileId == tenantProfile.Id)
            .Select(MapInquiry())
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundAppException("Demande introuvable.");
    }

    private static Expression<Func<Domain.Entities.Inquiry, InquiryDto>> MapInquiry()
        => x => new InquiryDto(
            x.Id,
            x.PropertyId,
            x.Property.Title,
            x.TenantProfileId,
            $"{x.TenantProfile.User.FirstName} {x.TenantProfile.User.LastName}".Trim(),
            x.Subject,
            x.Message,
            x.Status,
            x.CreatedAtUtc);

    private static void ValidateStatusTransition(InquiryStatus current, InquiryStatus next)
    {
        if (current == InquiryStatus.Closed && next != InquiryStatus.Archived)
        {
            throw new ValidationAppException("Une demande cloturee ne peut etre rouverte.");
        }
    }
}
