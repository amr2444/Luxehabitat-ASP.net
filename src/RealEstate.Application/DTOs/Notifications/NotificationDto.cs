using RealEstate.Domain.Enums;

namespace RealEstate.Application.DTOs.Notifications;

public sealed record NotificationDto(
    Guid Id,
    NotificationType Type,
    string Title,
    string Message,
    string? LinkUrl,
    bool IsRead,
    DateTime CreatedAtUtc);
