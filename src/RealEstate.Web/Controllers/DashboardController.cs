using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.Interfaces.Services;
using RealEstate.Web.Extensions;
using RealEstate.Web.Mappings;
using RealEstate.Web.ViewModels.Dashboard;

namespace RealEstate.Web.Controllers;

[Authorize]
public sealed class DashboardController(
    IAgentService agentService,
    ITenantService tenantService,
    IInquiryService inquiryService,
    IVisitAppointmentService visitAppointmentService) : Controller
{
    [HttpGet]
    public IActionResult Index()
    {
        if (User.IsInRole("Admin"))
        {
            return RedirectToAction("Dashboard", "Admin", new { area = "Admin" });
        }

        if (User.IsInRole("Agent"))
        {
            return RedirectToAction(nameof(Agent));
        }

        if (User.IsInRole("Tenant"))
        {
            return RedirectToAction(nameof(Tenant));
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> Agent(CancellationToken cancellationToken)
    {
        if (!User.IsInRole("Agent"))
        {
            return Forbid();
        }

        var userId = User.GetRequiredUserId();
        var dashboard = await agentService.GetDashboardAsync(userId, cancellationToken);
        var model = new AgentDashboardViewModel
        {
            TotalProperties = dashboard.TotalProperties,
            PublishedProperties = dashboard.PublishedProperties,
            PendingInquiries = dashboard.PendingInquiries,
            UpcomingVisits = dashboard.UpcomingVisits,
            ActiveApplications = dashboard.ActiveApplications,
            RecentInquiries = (await inquiryService.GetAgentInquiriesAsync(userId, cancellationToken)).Take(5).Select(x => x.ToAgentViewModel()).ToArray(),
            ScheduledVisits = (await visitAppointmentService.GetAgentVisitsAsync(userId, cancellationToken)).Take(5).Select(x => x.ToAgentViewModel()).ToArray()
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Tenant(CancellationToken cancellationToken)
    {
        if (!User.IsInRole("Tenant"))
        {
            return Forbid();
        }

        var userId = User.GetRequiredUserId();
        var dashboard = await tenantService.GetDashboardAsync(userId, cancellationToken);
        var model = new TenantDashboardViewModel
        {
            FavoritesCount = dashboard.FavoritesCount,
            PendingVisitsCount = dashboard.PendingVisitsCount,
            ActiveApplicationsCount = dashboard.ActiveApplicationsCount,
            UnreadNotificationsCount = dashboard.UnreadNotificationsCount,
            Favorites = (await tenantService.GetFavoritesAsync(userId, cancellationToken)).Take(4).Select(x => x.ToViewModel()).ToArray(),
            Visits = (await visitAppointmentService.GetTenantVisitsAsync(userId, cancellationToken)).Take(4).Select(x => x.ToTenantViewModel()).ToArray()
        };

        return View(model);
    }

    [HttpGet]
    public IActionResult Admin()
    {
        if (!User.IsInRole("Admin"))
        {
            return Forbid();
        }

        return RedirectToAction("Dashboard", "Admin", new { area = "Admin" });
    }
}
