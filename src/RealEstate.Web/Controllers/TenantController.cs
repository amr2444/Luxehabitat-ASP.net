using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.DTOs.Tenants;
using RealEstate.Application.Interfaces.Services;
using RealEstate.Web.Extensions;
using RealEstate.Web.Mappings;
using RealEstate.Web.Policies;
using RealEstate.Web.ViewModels.Tenant;

namespace RealEstate.Web.Controllers;

[Authorize(Policy = AuthorizationPolicies.RequireTenant)]
public sealed class TenantController(
    ITenantService tenantService,
    IInquiryService inquiryService,
    IVisitAppointmentService visitAppointmentService,
    IRentalApplicationService rentalApplicationService,
    IAuthorizationService authorizationService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Profile(CancellationToken cancellationToken)
    {
        var profile = await tenantService.GetProfileAsync(User.GetRequiredUserId(), cancellationToken);
        return View(profile.ToEditViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(EditTenantProfileViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await tenantService.UpdateProfileAsync(User.GetRequiredUserId(), new UpdateTenantProfileRequest(model.Occupation, model.MonthlyIncome, model.PreferredCityId, model.PreferredBudgetMin, model.PreferredBudgetMax), cancellationToken);
        TempData.PutSuccess("Le profil locataire a ete mis a jour.");
        return RedirectToAction(nameof(Profile));
    }

    [HttpGet]
    public async Task<IActionResult> Favorites(CancellationToken cancellationToken)
    {
        var items = await tenantService.GetFavoritesAsync(User.GetRequiredUserId(), cancellationToken);
        return View(items.Select(x => x.ToViewModel()).ToArray());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RemoveFavorite(Guid propertyId, CancellationToken cancellationToken)
    {
        if (!(await authorizationService.AuthorizeAsync(User, propertyId, AuthorizationPolicies.CanManageFavorite)).Succeeded)
        {
            return Forbid();
        }

        await tenantService.RemoveFavoriteAsync(User.GetRequiredUserId(), propertyId, cancellationToken);
        TempData.PutSuccess("Le bien a ete retire des favoris.");
        return RedirectToAction(nameof(Favorites));
    }

    [HttpGet]
    public async Task<IActionResult> Inquiries(CancellationToken cancellationToken)
    {
        var items = await inquiryService.GetTenantInquiriesAsync(User.GetRequiredUserId(), cancellationToken);
        return View(items.Select(x => x.ToTenantViewModel()).ToArray());
    }

    [HttpGet]
    public async Task<IActionResult> Visits(CancellationToken cancellationToken)
    {
        var items = await visitAppointmentService.GetTenantVisitsAsync(User.GetRequiredUserId(), cancellationToken);
        return View(items.Select(x => x.ToTenantViewModel()).ToArray());
    }

    [HttpGet]
    public async Task<IActionResult> Applications(CancellationToken cancellationToken)
    {
        var items = await rentalApplicationService.GetTenantApplicationsAsync(User.GetRequiredUserId(), cancellationToken);
        return View(items.Select(x => x.ToTenantViewModel()).ToArray());
    }
}
