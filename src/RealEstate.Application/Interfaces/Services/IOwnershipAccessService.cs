using RealEstate.Application.Enums;

namespace RealEstate.Application.Interfaces.Services;

public interface IOwnershipAccessService
{
    Task<bool> CanManageAsync(string userId, ProtectedResourceType resourceType, Guid resourceId, CancellationToken cancellationToken = default);
    Task<bool> CanViewAsync(string userId, ProtectedResourceType resourceType, Guid resourceId, CancellationToken cancellationToken = default);
}
