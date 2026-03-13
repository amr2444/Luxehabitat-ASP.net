using RealEstate.Domain.Enums;

namespace RealEstate.Application.DTOs.Notifications;

public sealed record CreateNotificationRequest(
    string UserId,
    NotificationType Type,
    string Title,
    string Message,
    string? LinkUrl);
