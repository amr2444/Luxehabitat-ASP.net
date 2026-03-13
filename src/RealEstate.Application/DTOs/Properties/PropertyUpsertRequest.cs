using RealEstate.Domain.Enums;

namespace RealEstate.Application.DTOs.Properties;

public sealed record PropertyUpsertRequest(
    Guid CityId,
    Guid? DistrictId,
    string? DistrictName,
    string Title,
    string Description,
    PropertyType PropertyType,
    ListingType ListingType,
    decimal Price,
    decimal? SecurityDeposit,
    decimal AreaSqm,
    int Bedrooms,
    int Bathrooms,
    int? Floor,
    string AddressLine,
    string PostalCode,
    DateTime? AvailableFromUtc);
