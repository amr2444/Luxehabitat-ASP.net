using RealEstate.Domain.Enums;

namespace RealEstate.Application.DTOs.Reviews;

public sealed record ReviewDto(
    Guid Id,
    Guid PropertyId,
    int Rating,
    string? Comment,
    string TenantDisplayName,
    ReviewStatus Status,
    DateTime CreatedAtUtc);
