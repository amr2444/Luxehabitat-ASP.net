using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using RealEstate.Application.DTOs.Common;
using RealEstate.Application.DTOs.Properties;
using RealEstate.Application.Exceptions;
using RealEstate.Application.Interfaces.Persistence;
using RealEstate.Application.Interfaces.Services;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;

namespace RealEstate.Application.Services;

public sealed class PropertyService(IApplicationDbContext context) : IPropertyService
{
    public async Task<PropertyDetailsDto> CreateAsync(string agentUserId, PropertyUpsertRequest request, CancellationToken cancellationToken = default)
    {
        ValidateRequest(request);
        var agentProfile = await GetAgentProfileAsync(agentUserId, cancellationToken);
        await EnsureLocationExistsAsync(request.CityId, request.DistrictId, cancellationToken);

        var property = new Property
        {
            AgentProfileId = agentProfile.Id,
            CityId = request.CityId,
            DistrictId = request.DistrictId,
            DistrictName = NormalizeDistrictName(request.DistrictName),
            Title = request.Title.Trim(),
            Slug = BuildSlug(request.Title),
            Description = request.Description.Trim(),
            PropertyType = request.PropertyType,
            ListingType = request.ListingType,
            Price = request.Price,
            SecurityDeposit = request.SecurityDeposit,
            AreaSqm = request.AreaSqm,
            Bedrooms = request.Bedrooms,
            Bathrooms = request.Bathrooms,
            Floor = request.Floor,
            AddressLine = request.AddressLine.Trim(),
            PostalCode = request.PostalCode.Trim(),
            AvailableFromUtc = request.AvailableFromUtc,
            Status = PropertyStatus.Draft,
            CreatedByUserId = agentUserId
        };

        await context.Properties.AddAsync(property, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return await GetPropertyDetailsInternalAsync(property.Id, cancellationToken);
    }

    public async Task<PropertyDetailsDto> UpdateAsync(string agentUserId, Guid propertyId, PropertyUpsertRequest request, CancellationToken cancellationToken = default)
    {
        ValidateRequest(request);
        var property = await GetOwnedPropertyAsync(agentUserId, propertyId, cancellationToken);
        await EnsureLocationExistsAsync(request.CityId, request.DistrictId, cancellationToken);
        EnsureEditable(property.Status);

        property.CityId = request.CityId;
        property.DistrictId = request.DistrictId;
        property.DistrictName = NormalizeDistrictName(request.DistrictName);
        property.Title = request.Title.Trim();
        property.Slug = BuildSlug(request.Title);
        property.Description = request.Description.Trim();
        property.PropertyType = request.PropertyType;
        property.ListingType = request.ListingType;
        property.Price = request.Price;
        property.SecurityDeposit = request.SecurityDeposit;
        property.AreaSqm = request.AreaSqm;
        property.Bedrooms = request.Bedrooms;
        property.Bathrooms = request.Bathrooms;
        property.Floor = request.Floor;
        property.AddressLine = request.AddressLine.Trim();
        property.PostalCode = request.PostalCode.Trim();
        property.AvailableFromUtc = request.AvailableFromUtc;
        property.UpdatedAtUtc = DateTime.UtcNow;
        property.UpdatedByUserId = agentUserId;

        await context.SaveChangesAsync(cancellationToken);
        return await GetPropertyDetailsInternalAsync(property.Id, cancellationToken);
    }

    public async Task ArchiveAsync(string agentUserId, Guid propertyId, CancellationToken cancellationToken = default)
    {
        var property = await GetOwnedPropertyAsync(agentUserId, propertyId, cancellationToken);
        if (property.Status == PropertyStatus.Suspended)
        {
            throw new ValidationAppException("Un bien suspendu par un administrateur ne peut pas etre archive par l'agent.");
        }

        property.Status = PropertyStatus.Archived;
        Stamp(property, agentUserId);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task PublishAsync(string agentUserId, Guid propertyId, CancellationToken cancellationToken = default)
    {
        var property = await GetOwnedPropertyAsync(agentUserId, propertyId, cancellationToken);
        if (property.Status is PropertyStatus.Archived or PropertyStatus.Rented or PropertyStatus.Suspended)
        {
            throw new ValidationAppException("Ce bien ne peut pas etre publie dans son statut actuel.");
        }

        property.Status = PropertyStatus.Published;
        property.PublishedAtUtc ??= DateTime.UtcNow;
        Stamp(property, agentUserId);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkAsRentedAsync(string agentUserId, Guid propertyId, CancellationToken cancellationToken = default)
    {
        var property = await GetOwnedPropertyAsync(agentUserId, propertyId, cancellationToken);
        if (property.Status != PropertyStatus.Published)
        {
            throw new ValidationAppException("Un agent ne peut marquer comme loue qu'un bien publie.");
        }

        property.Status = PropertyStatus.Rented;
        Stamp(property, agentUserId);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task SuspendAsync(Guid propertyId, string adminUserId, CancellationToken cancellationToken = default)
    {
        var property = await context.Properties.FirstOrDefaultAsync(x => x.Id == propertyId, cancellationToken)
            ?? throw new NotFoundAppException("Le bien demande est introuvable.");

        property.Status = PropertyStatus.Suspended;
        Stamp(property, adminUserId);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task RestoreAsync(Guid propertyId, string userId, bool isAdmin, CancellationToken cancellationToken = default)
    {
        var property = isAdmin
            ? await context.Properties.FirstOrDefaultAsync(x => x.Id == propertyId, cancellationToken)
            : await GetOwnedPropertyAsync(userId, propertyId, cancellationToken);

        if (property is null)
        {
            throw new NotFoundAppException("Le bien demande est introuvable.");
        }

        if (property.Status == PropertyStatus.Rented)
        {
            throw new ValidationAppException("Un bien loue ne peut pas etre restaure vers un statut actif.");
        }

        property.Status = PropertyStatus.Draft;
        property.IsDeleted = false;
        property.DeletedAtUtc = null;
        Stamp(property, userId);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task SoftDeleteAsync(Guid propertyId, string adminUserId, CancellationToken cancellationToken = default)
    {
        var property = await context.Properties.FirstOrDefaultAsync(x => x.Id == propertyId, cancellationToken)
            ?? throw new NotFoundAppException("Le bien demande est introuvable.");

        property.IsDeleted = true;
        property.DeletedAtUtc = DateTime.UtcNow;
        property.Status = PropertyStatus.Archived;
        Stamp(property, adminUserId);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<PropertyListItemDto>> GetAgentPropertiesAsync(string agentUserId, CancellationToken cancellationToken = default)
    {
        var agentProfile = await GetAgentProfileAsync(agentUserId, cancellationToken);

        return await context.Properties
            .AsNoTracking()
            .Where(x => x.AgentProfileId == agentProfile.Id)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(MapListItem())
            .ToListAsync(cancellationToken);
    }

    public async Task<PropertyDetailsDto> GetAgentPropertyDetailsAsync(string agentUserId, Guid propertyId, CancellationToken cancellationToken = default)
    {
        await GetOwnedPropertyAsync(agentUserId, propertyId, cancellationToken);
        return await GetPropertyDetailsInternalAsync(propertyId, cancellationToken);
    }

    public async Task<PropertyDetailsDto> GetPrivateDetailsAsync(Guid propertyId, CancellationToken cancellationToken = default)
        => await GetPropertyDetailsInternalAsync(propertyId, cancellationToken);

    public async Task<PagedResult<PropertyListItemDto>> GetPublishedAsync(PropertyFilterRequest filter, CancellationToken cancellationToken = default)
    {
        var query = context.Properties
            .AsNoTracking()
            .Where(x => x.Status == PropertyStatus.Published);

        if (filter.CityId.HasValue)
        {
            query = query.Where(x => x.CityId == filter.CityId.Value);
        }

        if (filter.DistrictId.HasValue)
        {
            query = query.Where(x => x.DistrictId == filter.DistrictId.Value);
        }

        if (filter.MinPrice.HasValue)
        {
            query = query.Where(x => x.Price >= filter.MinPrice.Value);
        }

        if (filter.MaxPrice.HasValue)
        {
            query = query.Where(x => x.Price <= filter.MaxPrice.Value);
        }

        if (filter.PropertyType.HasValue)
        {
            query = query.Where(x => x.PropertyType == filter.PropertyType.Value);
        }

        if (filter.ListingType.HasValue)
        {
            query = query.Where(x => x.ListingType == filter.ListingType.Value);
        }

        if (filter.Bedrooms.HasValue)
        {
            query = query.Where(x => x.Bedrooms >= filter.Bedrooms.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(x => x.PublishedAtUtc)
            .Skip((Math.Max(filter.PageNumber, 1) - 1) * Math.Max(filter.PageSize, 1))
            .Take(Math.Max(filter.PageSize, 1))
            .Select(MapListItem())
            .ToListAsync(cancellationToken);

        return new PagedResult<PropertyListItemDto>(items, Math.Max(filter.PageNumber, 1), Math.Max(filter.PageSize, 1), totalCount);
    }

    public async Task<PropertyDetailsDto> GetPublishedDetailsAsync(Guid propertyId, CancellationToken cancellationToken = default)
    {
        var property = await context.Properties
            .AsNoTracking()
            .Include(x => x.City)
            .Include(x => x.District)
            .Include(x => x.Images)
            .Include(x => x.Amenities)
            .Include(x => x.AgentProfile)
                .ThenInclude(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == propertyId && x.Status == PropertyStatus.Published, cancellationToken);

        if (property is null)
        {
            throw new NotFoundAppException("Le bien demande est introuvable.");
        }

        return MapDetails(property);
    }

    private async Task<Property> GetOwnedPropertyAsync(string agentUserId, Guid propertyId, CancellationToken cancellationToken)
    {
        var agentProfile = await GetAgentProfileAsync(agentUserId, cancellationToken);
        var property = await context.Properties
            .FirstOrDefaultAsync(x => x.Id == propertyId, cancellationToken);

        if (property is null)
        {
            throw new NotFoundAppException("Le bien demande est introuvable.");
        }

        if (property.AgentProfileId != agentProfile.Id)
        {
            throw new ForbiddenAppException("Un agent ne peut modifier que ses propres biens.");
        }

        return property;
    }

    private async Task<AgentProfile> GetAgentProfileAsync(string agentUserId, CancellationToken cancellationToken)
    {
        var agentProfile = await context.AgentProfiles.FirstOrDefaultAsync(x => x.UserId == agentUserId, cancellationToken);
        if (agentProfile is null)
        {
            throw new NotFoundAppException("Profil agent introuvable.");
        }

        return agentProfile;
    }

    private async Task EnsureLocationExistsAsync(Guid cityId, Guid? districtId, CancellationToken cancellationToken)
    {
        if (!await context.Cities.AnyAsync(x => x.Id == cityId, cancellationToken))
        {
            throw new ValidationAppException("La ville selectionnee est invalide.");
        }

        if (districtId.HasValue && !await context.Districts.AnyAsync(x => x.Id == districtId.Value && x.CityId == cityId, cancellationToken))
        {
            throw new ValidationAppException("Le quartier selectionne est invalide pour cette ville.");
        }
    }

    private async Task<PropertyDetailsDto> GetPropertyDetailsInternalAsync(Guid propertyId, CancellationToken cancellationToken)
    {
        var property = await context.Properties
            .AsNoTracking()
            .Include(x => x.City)
            .Include(x => x.District)
            .Include(x => x.Images)
            .Include(x => x.Amenities)
            .Include(x => x.AgentProfile)
                .ThenInclude(x => x.User)
            .FirstAsync(x => x.Id == propertyId, cancellationToken);

        return MapDetails(property);
    }

    private static Expression<Func<Property, PropertyListItemDto>> MapListItem()
        => x => new PropertyListItemDto(
            x.Id,
            x.Title,
            x.Slug,
            x.City.Name,
            x.DistrictName ?? (x.District != null ? x.District.Name : null),
            x.Price,
            x.AreaSqm,
            x.Bedrooms,
            x.Bathrooms,
            x.PropertyType,
            x.ListingType,
            x.Status,
            x.Images.Where(i => i.IsCover).Select(i => i.ImageUrl).FirstOrDefault(),
            x.PublishedAtUtc);

    private static PropertyDetailsDto MapDetails(Property property)
        => new(
            property.Id,
            property.CityId,
            property.DistrictId,
            property.DistrictName ?? property.District?.Name,
            property.Title,
            property.Slug,
            property.Description,
            property.City.Name,
            property.DistrictName ?? property.District?.Name,
            property.Price,
            property.SecurityDeposit,
            property.AreaSqm,
            property.Bedrooms,
            property.Bathrooms,
            property.Floor,
            property.AddressLine,
            property.PostalCode,
            property.PropertyType,
            property.ListingType,
            property.Status,
            property.AvailableFromUtc,
            property.PublishedAtUtc,
            $"{property.AgentProfile.User.FirstName} {property.AgentProfile.User.LastName}".Trim(),
            property.Images.OrderBy(x => x.DisplayOrder).Select(x => new PropertyImageDto(x.Id, x.ImageUrl, x.IsCover, x.DisplayOrder)).ToArray(),
            property.Amenities.Select(x => x.Label).ToArray());

    private static void ValidateRequest(PropertyUpsertRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Title) || string.IsNullOrWhiteSpace(request.Description))
        {
            throw new ValidationAppException("Le titre et la description du bien sont requis.");
        }

        if (!string.IsNullOrWhiteSpace(request.DistrictName) && request.DistrictName.Trim().Length > 150)
        {
            throw new ValidationAppException("Le nom du quartier ne peut pas depasser 150 caracteres.");
        }

        if (request.Price <= 0 || request.AreaSqm <= 0)
        {
            throw new ValidationAppException("Le prix et la surface doivent etre superieurs a zero.");
        }
    }

    private static void EnsureEditable(PropertyStatus status)
    {
        if (status is PropertyStatus.Rented or PropertyStatus.Suspended)
        {
            throw new ValidationAppException("Ce bien ne peut plus etre modifie dans son statut actuel.");
        }
    }

    private static void Stamp(Property property, string userId)
    {
        property.UpdatedAtUtc = DateTime.UtcNow;
        property.UpdatedByUserId = userId;
    }

    private static string BuildSlug(string title)
        => string.Join('-', title.Trim().ToLowerInvariant().Split(' ', StringSplitOptions.RemoveEmptyEntries));

    private static string? NormalizeDistrictName(string? districtName)
        => string.IsNullOrWhiteSpace(districtName) ? null : districtName.Trim();
}
