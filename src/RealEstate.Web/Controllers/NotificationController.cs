using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.Interfaces.Services;
using RealEstate.Web.Extensions;

namespace RealEstate.Web.Controllers;

[Authorize]
public sealed class NotificationController(INotificationService notificationService) : Controller
{
    [HttpGet]
    public IActionResult Panel()
        => ViewComponent("NotificationPanel");

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsRead(Guid notificationId, string? returnUrl = null, CancellationToken cancellationToken = default)
    {
        await notificationService.MarkAsReadAsync(User.GetRequiredUserId(), notificationId, cancellationToken);
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Dashboard");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAllAsRead(string? returnUrl = null, CancellationToken cancellationToken = default)
    {
        await notificationService.MarkAllAsReadAsync(User.GetRequiredUserId(), cancellationToken);

        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Dashboard");
    }
}
