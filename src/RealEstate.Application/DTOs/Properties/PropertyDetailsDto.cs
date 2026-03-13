using RealEstate.Domain.Enums;

namespace RealEstate.Application.DTOs.Properties;

public sealed record PropertyDetailsDto(
    Guid Id,
    Guid CityId,
    Guid? DistrictId,
    string? DistrictName,
    string Title,
    string Slug,
    string Description,
    string City,
    string? District,
    decimal Price,
    decimal? SecurityDeposit,
    decimal AreaSqm,
    int Bedrooms,
    int Bathrooms,
    int? Floor,
    string AddressLine,
    string PostalCode,
    PropertyType PropertyType,
    ListingType ListingType,
    PropertyStatus Status,
    DateTime? AvailableFromUtc,
    DateTime? PublishedAtUtc,
    string AgentDisplayName,
    IReadOnlyCollection<PropertyImageDto> Images,
    IReadOnlyCollection<string> Amenities);
