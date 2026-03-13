using RealEstate.Domain.Enums;

namespace RealEstate.Application.DTOs.Properties;

public sealed record PropertyListItemDto(
    Guid Id,
    string Title,
    string Slug,
    string City,
    string? District,
    decimal Price,
    decimal AreaSqm,
    int Bedrooms,
    int Bathrooms,
    PropertyType PropertyType,
    ListingType ListingType,
    PropertyStatus Status,
    string? CoverImageUrl,
    DateTime? PublishedAtUtc);
