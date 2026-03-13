using Microsoft.EntityFrameworkCore;
using RealEstate.Application.DTOs.Files;
using RealEstate.Application.DTOs.Properties;
using RealEstate.Application.Exceptions;
using RealEstate.Application.Interfaces.Persistence;
using RealEstate.Application.Interfaces.Services;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Services;

public sealed class PropertyImageService(
    IApplicationDbContext context,
    IFileStorageService fileStorageService) : IPropertyImageService
{
    private static readonly HashSet<string> AllowedContentTypes =
    [
        "image/jpeg",
        "image/png",
        "image/webp"
    ];

    private static readonly HashSet<string> AllowedExtensions =
    [
        ".jpg",
        ".jpeg",
        ".png",
        ".webp"
    ];

    private const long MaxFileSize = 5 * 1024 * 1024;
    private const int MaxImagesPerProperty = 12;

    public async Task<IReadOnlyCollection<PropertyImageDto>> UploadAsync(
        string agentUserId,
        Guid propertyId,
        IReadOnlyCollection<FileUploadRequest> files,
        CancellationToken cancellationToken = default)
    {
        if (files.Count == 0)
        {
            return Array.Empty<PropertyImageDto>();
        }

        var property = await GetOwnedPropertyWithImagesAsync(agentUserId, propertyId, cancellationToken);
        if (property.Images.Count + files.Count > MaxImagesPerProperty)
        {
            throw new ValidationAppException($"Une propriete ne peut pas contenir plus de {MaxImagesPerProperty} images.");
        }

        var currentOrder = property.Images.Any() ? property.Images.Max(x => x.DisplayOrder) : 0;
        var hasCover = property.Images.Any(x => x.IsCover);

        foreach (var file in files)
        {
            ValidateFile(file);

            var imageUrl = await fileStorageService.SavePropertyImageAsync(propertyId, file, cancellationToken);
            var image = new PropertyImage
            {
                PropertyId = propertyId,
                ImageUrl = imageUrl,
                DisplayOrder = ++currentOrder,
                IsCover = !hasCover,
                CreatedByUserId = agentUserId
            };
            await context.PropertyImages.AddAsync(image, cancellationToken);
            property.Images.Add(image);

            hasCover = true;
        }

        await context.SaveChangesAsync(cancellationToken);
        return Map(property.Images);
    }

    public async Task DeleteAsync(string agentUserId, Guid propertyId, Guid imageId, CancellationToken cancellationToken = default)
    {
        var property = await GetOwnedPropertyWithImagesAsync(agentUserId, propertyId, cancellationToken);
        var image = property.Images.FirstOrDefault(x => x.Id == imageId)
            ?? throw new NotFoundAppException("Image introuvable.");

        await fileStorageService.DeletePropertyImageAsync(image.ImageUrl, cancellationToken);
        property.Images.Remove(image);
        context.PropertyImages.Remove(image);

        NormalizeOrders(property.Images);
        EnsureCover(property.Images);

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task SetCoverAsync(string agentUserId, Guid propertyId, Guid imageId, CancellationToken cancellationToken = default)
    {
        var property = await GetOwnedPropertyWithImagesAsync(agentUserId, propertyId, cancellationToken);
        var image = property.Images.FirstOrDefault(x => x.Id == imageId)
            ?? throw new NotFoundAppException("Image introuvable.");

        foreach (var item in property.Images)
        {
            item.IsCover = item.Id == imageId;
            item.UpdatedAtUtc = DateTime.UtcNow;
            item.UpdatedByUserId = agentUserId;
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task ReorderAsync(string agentUserId, Guid propertyId, IReadOnlyCollection<Guid> orderedImageIds, CancellationToken cancellationToken = default)
    {
        var property = await GetOwnedPropertyWithImagesAsync(agentUserId, propertyId, cancellationToken);
        if (orderedImageIds.Count == 0)
        {
            return;
        }

        var imageMap = property.Images.ToDictionary(x => x.Id);
        if (orderedImageIds.Count != property.Images.Count || orderedImageIds.Any(x => !imageMap.ContainsKey(x)))
        {
            throw new ValidationAppException("Le nouvel ordre des images est invalide.");
        }

        var order = 1;
        foreach (var imageId in orderedImageIds)
        {
            var image = imageMap[imageId];
            image.DisplayOrder = order++;
            image.UpdatedAtUtc = DateTime.UtcNow;
            image.UpdatedByUserId = agentUserId;
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    private async Task<Property> GetOwnedPropertyWithImagesAsync(string agentUserId, Guid propertyId, CancellationToken cancellationToken)
    {
        var agentId = await context.AgentProfiles
            .Where(x => x.UserId == agentUserId)
            .Select(x => (Guid?)x.Id)
            .FirstOrDefaultAsync(cancellationToken)
            ?? throw new NotFoundAppException("Profil agent introuvable.");

        var property = await context.Properties
            .Include(x => x.Images)
            .FirstOrDefaultAsync(x => x.Id == propertyId, cancellationToken)
            ?? throw new NotFoundAppException("Bien introuvable.");

        if (property.AgentProfileId != agentId)
        {
            throw new ForbiddenAppException("Vous ne pouvez gerer les images que de vos propres biens.");
        }

        return property;
    }

    private static IReadOnlyCollection<PropertyImageDto> Map(ICollection<PropertyImage> images)
        => images
            .OrderBy(x => x.DisplayOrder)
            .Select(x => new PropertyImageDto(x.Id, x.ImageUrl, x.IsCover, x.DisplayOrder))
            .ToArray();

    private static void NormalizeOrders(ICollection<PropertyImage> images)
    {
        var order = 1;
        foreach (var image in images.OrderBy(x => x.DisplayOrder))
        {
            image.DisplayOrder = order++;
        }
    }

    private static void EnsureCover(ICollection<PropertyImage> images)
    {
        if (images.Count == 0)
        {
            return;
        }

        if (images.Any(x => x.IsCover))
        {
            return;
        }

        images.OrderBy(x => x.DisplayOrder).First().IsCover = true;
    }

    private static void ValidateFile(FileUploadRequest file)
    {
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!AllowedExtensions.Contains(extension))
        {
            throw new ValidationAppException("Seuls les formats JPG, PNG et WEBP sont autorises.");
        }

        if (!AllowedContentTypes.Contains(file.ContentType))
        {
            throw new ValidationAppException("Le type MIME de l'image est invalide.");
        }

        if (file.Length <= 0 || file.Length > MaxFileSize)
        {
            throw new ValidationAppException("Chaque image doit peser entre 1 octet et 5 Mo.");
        }

        if (!HasValidSignature(file.Content, extension))
        {
            throw new ValidationAppException("La signature binaire du fichier image est invalide.");
        }
    }

    private static bool HasValidSignature(Stream stream, string extension)
    {
        if (!stream.CanRead)
        {
            return false;
        }

        var origin = stream.CanSeek ? stream.Position : 0;
        try
        {
            if (stream.CanSeek)
            {
                stream.Position = 0;
            }

            Span<byte> header = stackalloc byte[12];
            var bytesRead = stream.Read(header);
            if (stream.CanSeek)
            {
                stream.Position = origin;
            }

            return extension switch
            {
                ".jpg" or ".jpeg" => bytesRead >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF,
                ".png" => bytesRead >= 8 && header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47,
                ".webp" => bytesRead >= 12 &&
                           header[0] == 0x52 && header[1] == 0x49 && header[2] == 0x46 && header[3] == 0x46 &&
                           header[8] == 0x57 && header[9] == 0x45 && header[10] == 0x42 && header[11] == 0x50,
                _ => false
            };
        }
        finally
        {
            if (stream.CanSeek)
            {
                stream.Position = origin;
            }
        }
    }
}
