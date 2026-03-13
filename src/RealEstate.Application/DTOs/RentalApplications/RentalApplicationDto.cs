using RealEstate.Domain.Enums;

namespace RealEstate.Application.DTOs.RentalApplications;

public sealed record RentalApplicationDto(
    Guid Id,
    Guid PropertyId,
    string PropertyTitle,
    RentalApplicationStatus Status,
    DateTime? RequestedMoveInDateUtc,
    DateTime? SubmittedAtUtc);
