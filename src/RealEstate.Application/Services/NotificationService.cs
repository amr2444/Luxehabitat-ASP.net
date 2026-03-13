using Microsoft.EntityFrameworkCore;
using RealEstate.Application.DTOs.Notifications;
using RealEstate.Application.Exceptions;
using RealEstate.Application.Interfaces.Persistence;
using RealEstate.Application.Interfaces.Services;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Services;

public sealed class NotificationService(IApplicationDbContext context) : INotificationService
{
    public async Task<NotificationDto> CreateAsync(CreateNotificationRequest request, CancellationToken cancellationToken = default)
    {
        var userExists = await context.Users.AnyAsync(x => x.Id == request.UserId, cancellationToken);
        if (!userExists)
        {
            throw new NotFoundAppException("Utilisateur introuvable pour la notification.");
        }

        var notification = new Notification
        {
            UserId = request.UserId,
            Type = request.Type,
            Title = request.Title.Trim(),
            Message = request.Message.Trim(),
            LinkUrl = request.LinkUrl,
            CreatedByUserId = request.UserId
        };

        await context.Notifications.AddAsync(notification, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return Map(notification);
    }

    public async Task<IReadOnlyCollection<NotificationDto>> GetUserNotificationsAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await context.Notifications
            .AsNoTracking()
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => Map(x))
            .ToListAsync(cancellationToken);
    }

    public async Task MarkAsReadAsync(string userId, Guid notificationId, CancellationToken cancellationToken = default)
    {
        var notification = await context.Notifications.FirstOrDefaultAsync(x => x.Id == notificationId, cancellationToken)
            ?? throw new NotFoundAppException("Notification introuvable.");

        if (notification.UserId != userId)
        {
            throw new ForbiddenAppException("Vous ne pouvez pas modifier cette notification.");
        }

        notification.IsRead = true;
        notification.ReadAtUtc = DateTime.UtcNow;
        notification.UpdatedAtUtc = DateTime.UtcNow;
        notification.UpdatedByUserId = userId;

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task MarkAllAsReadAsync(string userId, CancellationToken cancellationToken = default)
    {
        var notifications = await context.Notifications
            .Where(x => x.UserId == userId && !x.IsRead)
            .ToListAsync(cancellationToken);

        if (notifications.Count == 0)
        {
            return;
        }

        var now = DateTime.UtcNow;
        foreach (var notification in notifications)
        {
            notification.IsRead = true;
            notification.ReadAtUtc = now;
            notification.UpdatedAtUtc = now;
            notification.UpdatedByUserId = userId;
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    private static NotificationDto Map(Notification notification) =>
        new(notification.Id, notification.Type, notification.Title, notification.Message, notification.LinkUrl, notification.IsRead, notification.CreatedAtUtc);
}
