using RealEstate.Domain.Enums;

namespace RealEstate.Application.DTOs.Properties;

public sealed record PropertyFilterRequest(
    Guid? CityId,
    Guid? DistrictId,
    decimal? MinPrice,
    decimal? MaxPrice,
    PropertyType? PropertyType,
    ListingType? ListingType,
    int? Bedrooms,
    int PageNumber = 1,
    int PageSize = 9);
