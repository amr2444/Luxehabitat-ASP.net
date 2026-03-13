namespace RealEstate.Application.DTOs.RentalApplications;

public sealed record SubmitRentalApplicationRequest(
    Guid PropertyId,
    string? CoverLetter,
    DateTime? RequestedMoveInDateUtc);
