using RealEstate.Application.DTOs.Files;
using RealEstate.Application.DTOs.Properties;

namespace RealEstate.Application.Interfaces.Services;

public interface IPropertyImageService
{
    Task<IReadOnlyCollection<PropertyImageDto>> UploadAsync(
        string agentUserId,
        Guid propertyId,
        IReadOnlyCollection<FileUploadRequest> files,
        CancellationToken cancellationToken = default);
    Task DeleteAsync(string agentUserId, Guid propertyId, Guid imageId, CancellationToken cancellationToken = default);
    Task SetCoverAsync(string agentUserId, Guid propertyId, Guid imageId, CancellationToken cancellationToken = default);
    Task ReorderAsync(string agentUserId, Guid propertyId, IReadOnlyCollection<Guid> orderedImageIds, CancellationToken cancellationToken = default);
}
