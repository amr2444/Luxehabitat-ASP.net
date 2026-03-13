using RealEstate.Application.DTOs.Files;

namespace RealEstate.Application.Interfaces.Services;

public interface IFileStorageService
{
    Task<string> SavePropertyImageAsync(Guid propertyId, FileUploadRequest file, CancellationToken cancellationToken = default);
    Task DeletePropertyImageAsync(string imageUrl, CancellationToken cancellationToken = default);
}
