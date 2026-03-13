using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.Interfaces.Services;
using RealEstate.Web.Extensions;
using RealEstate.Web.Mappings;
using RealEstate.Web.ViewModels.Shared;

namespace RealEstate.Web.ViewComponents;

public sealed class NotificationPanelViewComponent(INotificationService notificationService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        if (User.Identity?.IsAuthenticated != true)
        {
            return View(new NotificationPanelViewModel());
        }

        var userId = UserClaimsPrincipal.GetRequiredUserId();
        var notifications = await notificationService.GetUserNotificationsAsync(userId);
        var items = notifications.Take(5).Select(x => x.ToViewModel()).ToArray();

        return View(new NotificationPanelViewModel
        {
            UnreadCount = notifications.Count(x => !x.IsRead),
            Items = items
        });
    }
}
