using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.Interfaces.Services;
using RealEstate.Web.Extensions;
using RealEstate.Web.Mappings;
using RealEstate.Web.Policies;

namespace RealEstate.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = AuthorizationPolicies.RequireAdmin)]
public sealed class AdminController(IAdminService adminService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Dashboard(CancellationToken cancellationToken)
        => View((await adminService.GetDashboardAsync(cancellationToken)).ToViewModel());

    [HttpGet]
    public async Task<IActionResult> Users(CancellationToken cancellationToken)
        => View((await adminService.GetUsersAsync(cancellationToken)).Select(x => x.ToViewModel()).ToArray());

    [HttpGet]
    public async Task<IActionResult> Agents(CancellationToken cancellationToken)
        => View((await adminService.GetAgentsAsync(cancellationToken)).Select(x => x.ToViewModel()).ToArray());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveAgent(Guid agentProfileId, CancellationToken cancellationToken)
    {
        await adminService.ApproveAgentAsync(agentProfileId, User.GetRequiredUserId(), cancellationToken);
        TempData.PutSuccess("L'agent a ete approuve.");
        return RedirectToAction(nameof(Agents));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SuspendAgent(Guid agentProfileId, CancellationToken cancellationToken)
    {
        await adminService.SuspendAgentAsync(agentProfileId, User.GetRequiredUserId(), cancellationToken);
        TempData.PutSuccess("L'agent a ete suspendu.");
        return RedirectToAction(nameof(Agents));
    }

    [HttpGet]
    public async Task<IActionResult> Properties(CancellationToken cancellationToken)
        => View((await adminService.GetPropertiesAsync(cancellationToken)).Select(x => x.ToViewModel()).ToArray());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SuspendProperty(Guid propertyId, CancellationToken cancellationToken)
    {
        await adminService.SuspendPropertyAsync(propertyId, User.GetRequiredUserId(), cancellationToken);
        TempData.PutSuccess("L'annonce a ete suspendue.");
        return RedirectToAction(nameof(Properties));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteProperty(Guid propertyId, CancellationToken cancellationToken)
    {
        await adminService.DeletePropertyAsync(propertyId, User.GetRequiredUserId(), cancellationToken);
        TempData.PutSuccess("L'annonce a ete supprimee.");
        return RedirectToAction(nameof(Properties));
    }

    [HttpGet]
    public async Task<IActionResult> Reviews(CancellationToken cancellationToken)
        => View((await adminService.GetReviewsAsync(cancellationToken)).Select(x => x.ToViewModel()).ToArray());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> HideReview(Guid reviewId, CancellationToken cancellationToken)
    {
        await adminService.HideReviewAsync(reviewId, User.GetRequiredUserId(), cancellationToken);
        TempData.PutSuccess("L'avis a ete masque.");
        return RedirectToAction(nameof(Reviews));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteReview(Guid reviewId, CancellationToken cancellationToken)
    {
        await adminService.DeleteReviewAsync(reviewId, User.GetRequiredUserId(), cancellationToken);
        TempData.PutSuccess("L'avis a ete supprime.");
        return RedirectToAction(nameof(Reviews));
    }
}
