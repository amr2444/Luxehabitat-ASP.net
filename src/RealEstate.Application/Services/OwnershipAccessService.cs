using Microsoft.EntityFrameworkCore;
using RealEstate.Application.Enums;
using RealEstate.Application.Interfaces.Persistence;
using RealEstate.Application.Interfaces.Services;

namespace RealEstate.Application.Services;

public sealed class OwnershipAccessService(IApplicationDbContext context) : IOwnershipAccessService
{
    public async Task<bool> CanManageAsync(string userId, ProtectedResourceType resourceType, Guid resourceId, CancellationToken cancellationToken = default)
    {
        if (!await UserExistsAsync(userId, cancellationToken))
        {
            return false;
        }

        return resourceType switch
        {
            ProtectedResourceType.Property => await CanManagePropertyAsync(userId, resourceId, cancellationToken),
            ProtectedResourceType.Inquiry => await CanManageInquiryAsync(userId, resourceId, cancellationToken),
            ProtectedResourceType.Visit => await CanManageVisitAsync(userId, resourceId, cancellationToken),
            ProtectedResourceType.RentalApplication => await CanManageRentalApplicationAsync(userId, resourceId, cancellationToken),
            ProtectedResourceType.Favorite => await CanManageFavoriteAsync(userId, resourceId, cancellationToken),
            ProtectedResourceType.PropertyPrivateDetails => await CanManagePropertyAsync(userId, resourceId, cancellationToken),
            ProtectedResourceType.ViewInquiry => await CanViewInquiryAsync(userId, resourceId, cancellationToken),
            ProtectedResourceType.ViewVisit => await CanViewVisitAsync(userId, resourceId, cancellationToken),
            ProtectedResourceType.ViewRentalApplication => await CanViewRentalApplicationAsync(userId, resourceId, cancellationToken),
            _ => false
        };
    }

    public async Task<bool> CanViewAsync(string userId, ProtectedResourceType resourceType, Guid resourceId, CancellationToken cancellationToken = default)
    {
        if (!await UserExistsAsync(userId, cancellationToken))
        {
            return false;
        }

        return resourceType switch
        {
            ProtectedResourceType.Property => await CanManagePropertyAsync(userId, resourceId, cancellationToken),
            ProtectedResourceType.PropertyPrivateDetails => await CanManagePropertyAsync(userId, resourceId, cancellationToken),
            ProtectedResourceType.ViewInquiry => await CanViewInquiryAsync(userId, resourceId, cancellationToken),
            ProtectedResourceType.ViewVisit => await CanViewVisitAsync(userId, resourceId, cancellationToken),
            ProtectedResourceType.ViewRentalApplication => await CanViewRentalApplicationAsync(userId, resourceId, cancellationToken),
            ProtectedResourceType.Inquiry => await CanViewInquiryAsync(userId, resourceId, cancellationToken),
            ProtectedResourceType.Visit => await CanViewVisitAsync(userId, resourceId, cancellationToken),
            ProtectedResourceType.RentalApplication => await CanViewRentalApplicationAsync(userId, resourceId, cancellationToken),
            ProtectedResourceType.Favorite => await CanManageFavoriteAsync(userId, resourceId, cancellationToken),
            _ => false
        };
    }

    private Task<bool> UserExistsAsync(string userId, CancellationToken cancellationToken)
        => context.Users.AnyAsync(x => x.Id == userId, cancellationToken);

    private async Task<Guid?> GetAgentIdAsync(string userId, CancellationToken cancellationToken)
        => await context.AgentProfiles.Where(x => x.UserId == userId).Select(x => (Guid?)x.Id).FirstOrDefaultAsync(cancellationToken);

    private async Task<Guid?> GetTenantIdAsync(string userId, CancellationToken cancellationToken)
        => await context.TenantProfiles.Where(x => x.UserId == userId).Select(x => (Guid?)x.Id).FirstOrDefaultAsync(cancellationToken);

    private async Task<bool> CanManagePropertyAsync(string userId, Guid propertyId, CancellationToken cancellationToken)
    {
        var agentId = await GetAgentIdAsync(userId, cancellationToken);
        return agentId.HasValue && await context.Properties.AnyAsync(x => x.Id == propertyId && x.AgentProfileId == agentId.Value, cancellationToken);
    }

    private async Task<bool> CanManageInquiryAsync(string userId, Guid inquiryId, CancellationToken cancellationToken)
    {
        var agentId = await GetAgentIdAsync(userId, cancellationToken);
        return agentId.HasValue && await context.Inquiries.AnyAsync(x => x.Id == inquiryId && x.AgentProfileId == agentId.Value, cancellationToken);
    }

    private async Task<bool> CanManageVisitAsync(string userId, Guid visitId, CancellationToken cancellationToken)
    {
        var agentId = await GetAgentIdAsync(userId, cancellationToken);
        return agentId.HasValue && await context.VisitAppointments.AnyAsync(x => x.Id == visitId && x.AgentProfileId == agentId.Value, cancellationToken);
    }

    private async Task<bool> CanManageRentalApplicationAsync(string userId, Guid applicationId, CancellationToken cancellationToken)
    {
        var agentId = await GetAgentIdAsync(userId, cancellationToken);
        return agentId.HasValue && await context.RentalApplications.AnyAsync(x => x.Id == applicationId && x.AgentProfileId == agentId.Value, cancellationToken);
    }

    private async Task<bool> CanManageFavoriteAsync(string userId, Guid propertyId, CancellationToken cancellationToken)
    {
        var tenantId = await GetTenantIdAsync(userId, cancellationToken);
        return tenantId.HasValue && await context.Favorites.AnyAsync(x => x.PropertyId == propertyId && x.TenantProfileId == tenantId.Value, cancellationToken);
    }

    private async Task<bool> CanViewInquiryAsync(string userId, Guid inquiryId, CancellationToken cancellationToken)
    {
        var agentId = await GetAgentIdAsync(userId, cancellationToken);
        var tenantId = await GetTenantIdAsync(userId, cancellationToken);

        return await context.Inquiries.AnyAsync(
            x => x.Id == inquiryId &&
                 ((agentId.HasValue && x.AgentProfileId == agentId.Value) ||
                  (tenantId.HasValue && x.TenantProfileId == tenantId.Value)),
            cancellationToken);
    }

    private async Task<bool> CanViewVisitAsync(string userId, Guid visitId, CancellationToken cancellationToken)
    {
        var agentId = await GetAgentIdAsync(userId, cancellationToken);
        var tenantId = await GetTenantIdAsync(userId, cancellationToken);

        return await context.VisitAppointments.AnyAsync(
            x => x.Id == visitId &&
                 ((agentId.HasValue && x.AgentProfileId == agentId.Value) ||
                  (tenantId.HasValue && x.TenantProfileId == tenantId.Value)),
            cancellationToken);
    }

    private async Task<bool> CanViewRentalApplicationAsync(string userId, Guid applicationId, CancellationToken cancellationToken)
    {
        var agentId = await GetAgentIdAsync(userId, cancellationToken);
        var tenantId = await GetTenantIdAsync(userId, cancellationToken);

        return await context.RentalApplications.AnyAsync(
            x => x.Id == applicationId &&
                 ((agentId.HasValue && x.AgentProfileId == agentId.Value) ||
                  (tenantId.HasValue && x.TenantProfileId == tenantId.Value)),
            cancellationToken);
    }
}
