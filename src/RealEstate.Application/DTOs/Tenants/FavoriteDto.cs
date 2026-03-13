namespace RealEstate.Application.DTOs.Tenants;

public sealed record FavoriteDto(
    Guid FavoriteId,
    Guid PropertyId,
    string PropertyTitle,
    decimal Price,
    string City,
    string? CoverImageUrl);
