using Microsoft.AspNetCore.Hosting;
using RealEstate.Application.DTOs.Files;
using RealEstate.Application.Interfaces.Services;

namespace RealEstate.Infrastructure.Services;

public sealed class LocalFileStorageService(IWebHostEnvironment environment) : IFileStorageService
{
    public async Task<string> SavePropertyImageAsync(Guid propertyId, FileUploadRequest file, CancellationToken cancellationToken = default)
    {
        var rootPath = environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot");
        var uploadsRoot = Path.Combine(rootPath, "uploads", "properties", propertyId.ToString("N"));
        Directory.CreateDirectory(uploadsRoot);

        var extension = Path.GetExtension(file.FileName);
        var safeExtension = string.IsNullOrWhiteSpace(extension) ? ".bin" : extension.ToLowerInvariant();
        var fileName = $"{Guid.NewGuid():N}{safeExtension}";
        var fullPath = Path.Combine(uploadsRoot, fileName);

        await using var output = File.Create(fullPath);
        await file.Content.CopyToAsync(output, cancellationToken);

        return $"/uploads/properties/{propertyId:N}/{fileName}";
    }

    public Task DeletePropertyImageAsync(string imageUrl, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
        {
            return Task.CompletedTask;
        }

        var relativePath = imageUrl.TrimStart('/').Replace('/', Path.DirectorySeparatorChar);
        var rootPath = environment.WebRootPath ?? Path.Combine(environment.ContentRootPath, "wwwroot");
        var fullPath = Path.Combine(rootPath, relativePath);

        if (File.Exists(fullPath))
        {
            File.Delete(fullPath);
        }

        return Task.CompletedTask;
    }
}
