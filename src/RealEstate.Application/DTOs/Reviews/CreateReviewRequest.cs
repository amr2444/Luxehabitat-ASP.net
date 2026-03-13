namespace RealEstate.Application.DTOs.Reviews;

public sealed record CreateReviewRequest(
    Guid PropertyId,
    int Rating,
    string? Comment);
