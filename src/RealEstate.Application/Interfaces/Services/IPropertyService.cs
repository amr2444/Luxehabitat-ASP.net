using RealEstate.Application.DTOs.Properties;
using RealEstate.Application.DTOs.Common;

namespace RealEstate.Application.Interfaces.Services;

public interface IPropertyService
{
    Task<PropertyDetailsDto> CreateAsync(string agentUserId, PropertyUpsertRequest request, CancellationToken cancellationToken = default);
    Task<PropertyDetailsDto> UpdateAsync(string agentUserId, Guid propertyId, PropertyUpsertRequest request, CancellationToken cancellationToken = default);
    Task ArchiveAsync(string agentUserId, Guid propertyId, CancellationToken cancellationToken = default);
    Task PublishAsync(string agentUserId, Guid propertyId, CancellationToken cancellationToken = default);
    Task MarkAsRentedAsync(string agentUserId, Guid propertyId, CancellationToken cancellationToken = default);
    Task SuspendAsync(Guid propertyId, string adminUserId, CancellationToken cancellationToken = default);
    Task RestoreAsync(Guid propertyId, string userId, bool isAdmin, CancellationToken cancellationToken = default);
    Task SoftDeleteAsync(Guid propertyId, string adminUserId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<PropertyListItemDto>> GetAgentPropertiesAsync(string agentUserId, CancellationToken cancellationToken = default);
    Task<PropertyDetailsDto> GetAgentPropertyDetailsAsync(string agentUserId, Guid propertyId, CancellationToken cancellationToken = default);
    Task<PropertyDetailsDto> GetPrivateDetailsAsync(Guid propertyId, CancellationToken cancellationToken = default);
    Task<PagedResult<PropertyListItemDto>> GetPublishedAsync(PropertyFilterRequest filter, CancellationToken cancellationToken = default);
    Task<PropertyDetailsDto> GetPublishedDetailsAsync(Guid propertyId, CancellationToken cancellationToken = default);
}
