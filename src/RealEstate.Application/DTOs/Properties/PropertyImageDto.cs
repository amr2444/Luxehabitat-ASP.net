namespace RealEstate.Application.DTOs.Properties;

public sealed record PropertyImageDto(
    Guid Id,
    string ImageUrl,
    bool IsCover,
    int DisplayOrder);
