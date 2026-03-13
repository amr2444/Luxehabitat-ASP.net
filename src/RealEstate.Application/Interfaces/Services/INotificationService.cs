using RealEstate.Application.DTOs.Notifications;

namespace RealEstate.Application.Interfaces.Services;

public interface INotificationService
{
    Task<NotificationDto> CreateAsync(CreateNotificationRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<NotificationDto>> GetUserNotificationsAsync(string userId, CancellationToken cancellationToken = default);
    Task MarkAsReadAsync(string userId, Guid notificationId, CancellationToken cancellationToken = default);
    Task MarkAllAsReadAsync(string userId, CancellationToken cancellationToken = default);
}
