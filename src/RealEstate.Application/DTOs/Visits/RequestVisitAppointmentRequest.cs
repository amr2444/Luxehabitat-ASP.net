namespace RealEstate.Application.DTOs.Visits;

public sealed record RequestVisitAppointmentRequest(
    Guid PropertyId,
    DateTime ScheduledAtUtc,
    string? Notes);
