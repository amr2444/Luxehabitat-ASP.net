using RealEstate.Application.DTOs.Admin;

namespace RealEstate.Application.Interfaces.Services;

public interface IAdminService
{
    Task<AdminDashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<AdminUserListItemDto>> GetUsersAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<AdminAgentListItemDto>> GetAgentsAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<AdminPropertyListItemDto>> GetPropertiesAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<AdminReviewListItemDto>> GetReviewsAsync(CancellationToken cancellationToken = default);
    Task ApproveAgentAsync(Guid agentProfileId, string adminUserId, CancellationToken cancellationToken = default);
    Task SuspendAgentAsync(Guid agentProfileId, string adminUserId, CancellationToken cancellationToken = default);
    Task SuspendPropertyAsync(Guid propertyId, string adminUserId, CancellationToken cancellationToken = default);
    Task DeletePropertyAsync(Guid propertyId, string adminUserId, CancellationToken cancellationToken = default);
    Task HideReviewAsync(Guid reviewId, string adminUserId, CancellationToken cancellationToken = default);
    Task DeleteReviewAsync(Guid reviewId, string adminUserId, CancellationToken cancellationToken = default);
}
