using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.DTOs.Agents;
using RealEstate.Application.Interfaces.Services;
using RealEstate.Domain.Enums;
using RealEstate.Web.Extensions;
using RealEstate.Web.Mappings;
using RealEstate.Web.Policies;
using RealEstate.Web.ViewModels.Agent;

namespace RealEstate.Web.Controllers;

[Authorize(Policy = AuthorizationPolicies.RequireAgent)]
public sealed class AgentController(
    IAgentService agentService,
    IInquiryService inquiryService,
    IVisitAppointmentService visitAppointmentService,
    IRentalApplicationService rentalApplicationService,
    IAuthorizationService authorizationService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Profile(CancellationToken cancellationToken)
    {
        var profile = await agentService.GetProfileAsync(User.GetRequiredUserId(), cancellationToken);
        return View(profile.ToEditViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(EditAgentProfileViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await agentService.UpdateProfileAsync(User.GetRequiredUserId(), new UpdateAgentProfileRequest(model.AgencyName, model.LicenseNumber, model.Bio, model.YearsOfExperience), cancellationToken);
        TempData.PutSuccess("Le profil agent a ete mis a jour.");
        return RedirectToAction(nameof(Profile));
    }

    [HttpGet]
    public async Task<IActionResult> Inquiries(CancellationToken cancellationToken)
    {
        var items = await inquiryService.GetAgentInquiriesAsync(User.GetRequiredUserId(), cancellationToken);
        return View(items.Select(x => x.ToAgentViewModel()).ToArray());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateInquiryStatus(UpdateInquiryStatusViewModel model, CancellationToken cancellationToken)
    {
        if (!(await authorizationService.AuthorizeAsync(User, model.InquiryId, AuthorizationPolicies.CanManageInquiry)).Succeeded)
        {
            return Forbid();
        }

        await inquiryService.ChangeStatusAsync(User.GetRequiredUserId(), model.InquiryId, model.Status, cancellationToken);
        TempData.PutSuccess("Le statut de la demande a ete mis a jour.");
        return RedirectToAction(nameof(Inquiries));
    }

    [HttpGet]
    public async Task<IActionResult> Visits(CancellationToken cancellationToken)
    {
        var items = await visitAppointmentService.GetAgentVisitsAsync(User.GetRequiredUserId(), cancellationToken);
        return View(items.Select(x => x.ToAgentViewModel()).ToArray());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmVisit(Guid visitId, CancellationToken cancellationToken)
    {
        if (!(await authorizationService.AuthorizeAsync(User, visitId, AuthorizationPolicies.CanManageVisit)).Succeeded)
        {
            return Forbid();
        }

        await visitAppointmentService.ConfirmAsync(User.GetRequiredUserId(), visitId, cancellationToken);
        TempData.PutSuccess("La visite a ete confirmee.");
        return RedirectToAction(nameof(Visits));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RejectVisit(Guid visitId, CancellationToken cancellationToken)
    {
        if (!(await authorizationService.AuthorizeAsync(User, visitId, AuthorizationPolicies.CanManageVisit)).Succeeded)
        {
            return Forbid();
        }

        await visitAppointmentService.RejectAsync(User.GetRequiredUserId(), visitId, cancellationToken);
        TempData.PutSuccess("La visite a ete refusee.");
        return RedirectToAction(nameof(Visits));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RescheduleVisit(RescheduleVisitViewModel model, CancellationToken cancellationToken)
    {
        if (!(await authorizationService.AuthorizeAsync(User, model.VisitId, AuthorizationPolicies.CanManageVisit)).Succeeded)
        {
            return Forbid();
        }

        await visitAppointmentService.RescheduleAsync(User.GetRequiredUserId(), model.VisitId, model.ScheduledAtUtc, cancellationToken);
        TempData.PutSuccess("La visite a ete reprogrammee.");
        return RedirectToAction(nameof(Visits));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CompleteVisit(Guid visitId, CancellationToken cancellationToken)
    {
        if (!(await authorizationService.AuthorizeAsync(User, visitId, AuthorizationPolicies.CanManageVisit)).Succeeded)
        {
            return Forbid();
        }

        await visitAppointmentService.CompleteAsync(User.GetRequiredUserId(), visitId, cancellationToken);
        TempData.PutSuccess("La visite a ete marquee comme terminee.");
        return RedirectToAction(nameof(Visits));
    }

    [HttpGet]
    public async Task<IActionResult> Applications(CancellationToken cancellationToken)
    {
        var items = await rentalApplicationService.GetAgentApplicationsAsync(User.GetRequiredUserId(), cancellationToken);
        return View(items.Select(x => x.ToAgentViewModel()).ToArray());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateApplicationStatus(UpdateApplicationStatusViewModel model, CancellationToken cancellationToken)
    {
        if (!(await authorizationService.AuthorizeAsync(User, model.ApplicationId, AuthorizationPolicies.CanManageRentalApplication)).Succeeded)
        {
            return Forbid();
        }

        await rentalApplicationService.ChangeStatusAsync(User.GetRequiredUserId(), model.ApplicationId, model.Status, cancellationToken);
        TempData.PutSuccess("Le statut de la candidature a ete mis a jour.");
        return RedirectToAction(nameof(Applications));
    }
}
