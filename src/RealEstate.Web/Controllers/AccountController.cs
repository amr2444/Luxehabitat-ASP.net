using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.DTOs.Agents;
using RealEstate.Application.DTOs.Tenants;
using RealEstate.Application.Interfaces.Services;
using RealEstate.Domain.Entities;
using RealEstate.Web.Extensions;
using RealEstate.Web.ViewModels.Account;

namespace RealEstate.Web.Controllers;

public sealed class AccountController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IAgentService agentService,
    ITenantService tenantService) : Controller
{
    [HttpGet]
    public IActionResult Login(string? returnUrl = null) => View(new LoginViewModel { ReturnUrl = returnUrl });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Identifiants invalides.");
            return View(model);
        }

        if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
        {
            return Redirect(model.ReturnUrl);
        }

        return RedirectToAction("Index", "Dashboard");
    }

    [HttpGet]
    public IActionResult Register() => View(new RegisterViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = new ApplicationUser
        {
            UserName = model.Email,
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        var role = model.Role is "Agent" ? "Agent" : "Tenant";
        await userManager.AddToRoleAsync(user, role);

        if (role == "Agent")
        {
            await agentService.UpdateProfileAsync(user.Id, new UpdateAgentProfileRequest(
                model.AgencyName ?? "Nouvelle agence",
                model.LicenseNumber ?? $"LIC-{Guid.NewGuid():N}"[..12],
                null,
                0), cancellationToken);
        }
        else
        {
            await tenantService.UpdateProfileAsync(user.Id, new UpdateTenantProfileRequest(null, null, null, null, null), cancellationToken);
        }

        await signInManager.SignInAsync(user, false);
        TempData.PutSuccess("Votre compte a ete cree avec succes.");
        return RedirectToAction("Index", "Dashboard");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }
}
