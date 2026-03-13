using RealEstate.Domain.Enums;

namespace RealEstate.Application.DTOs.Visits;

public sealed record VisitAppointmentDto(
    Guid Id,
    Guid PropertyId,
    string PropertyTitle,
    DateTime ScheduledAtUtc,
    VisitAppointmentStatus Status,
    string? Notes,
    string CounterpartyName);
