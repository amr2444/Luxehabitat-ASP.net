using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.DTOs.Files;
using RealEstate.Application.DTOs.Inquiries;
using RealEstate.Application.DTOs.Properties;
using RealEstate.Application.DTOs.RentalApplications;
using RealEstate.Application.DTOs.Visits;
using RealEstate.Application.Exceptions;
using RealEstate.Application.Interfaces.Services;
using RealEstate.Web.Extensions;
using RealEstate.Web.Mappings;
using RealEstate.Web.Policies;
using RealEstate.Web.ViewModels.Property;
using RealEstate.Web.ViewModels.Shared;

namespace RealEstate.Web.Controllers;

public sealed class PropertyController(
    IPropertyService propertyService,
    ILookupService lookupService,
    ITenantService tenantService,
    IInquiryService inquiryService,
    IVisitAppointmentService visitAppointmentService,
    IRentalApplicationService rentalApplicationService,
    IPropertyImageService propertyImageService,
    IAuthorizationService authorizationService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index([FromQuery] PropertyFilterViewModel filter, CancellationToken cancellationToken)
    {
        var cities = await lookupService.GetCitiesAsync(cancellationToken);
        var districts = await lookupService.GetDistrictsAsync(filter.CityId, cancellationToken);
        var pagedProperties = await propertyService.GetPublishedAsync(
            new PropertyFilterRequest(filter.CityId, filter.DistrictId, filter.MinPrice, filter.MaxPrice, filter.PropertyType, filter.ListingType, filter.Bedrooms, filter.PageNumber, 9),
            cancellationToken);

        var model = new PropertySearchPageViewModel
        {
            Filter = filter,
            Properties = pagedProperties.Items.Select(x => x.ToViewModel()).ToArray(),
            Pagination = new PaginationViewModel
            {
                CurrentPage = pagedProperties.PageNumber,
                TotalPages = pagedProperties.TotalPages
            }
        };

        model.Filter.Cities = cities.ToViewModels();
        model.Filter.Districts = districts.ToViewModels();

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var property = await propertyService.GetPublishedDetailsAsync(id, cancellationToken);
            return View(property.ToViewModel());
        }
        catch (NotFoundAppException)
        {
            return NotFound();
        }
    }

    [Authorize(Policy = AuthorizationPolicies.RequireAgent)]
    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken cancellationToken)
    {
        var model = new CreatePropertyViewModel();
        await PopulateLocationsAsync(model, cancellationToken);
        return View(model);
    }

    [Authorize(Policy = AuthorizationPolicies.RequireAgent)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreatePropertyViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            await PopulateLocationsAsync(model, cancellationToken);
            return View(model);
        }

        try
        {
            var property = await propertyService.CreateAsync(User.GetRequiredUserId(), BuildRequest(model), cancellationToken);
            await UploadImagesIfAnyAsync(property.Id, model.NewImages, cancellationToken);
            TempData.PutSuccess("Le bien a ete cree avec succes.");
            return RedirectToAction(nameof(MyProperties));
        }
        catch (AppException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            await PopulateLocationsAsync(model, cancellationToken);
            return View(model);
        }
    }

    [Authorize(Policy = AuthorizationPolicies.RequireAgent)]
    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken cancellationToken)
    {
        if (!(await authorizationService.AuthorizeAsync(User, id, AuthorizationPolicies.CanManageProperty)).Succeeded)
        {
            return Forbid();
        }

        try
        {
            var property = await propertyService.GetAgentPropertyDetailsAsync(User.GetRequiredUserId(), id, cancellationToken);
            var model = property.ToEditViewModel();
            await PopulateLocationsAsync(model, cancellationToken);
            return View(model);
        }
        catch (AppException)
        {
            return Forbid();
        }
    }

    [Authorize(Policy = AuthorizationPolicies.RequireAgent)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Guid id, EditPropertyViewModel model, CancellationToken cancellationToken)
    {
        if (!(await authorizationService.AuthorizeAsync(User, id, AuthorizationPolicies.CanManageProperty)).Succeeded)
        {
            return Forbid();
        }

        if (!ModelState.IsValid)
        {
            await PopulateLocationsAsync(model, cancellationToken);
            return View(model);
        }

        try
        {
            var property = await propertyService.UpdateAsync(User.GetRequiredUserId(), id, BuildRequest(model), cancellationToken);
            await UploadImagesIfAnyAsync(id, model.NewImages, cancellationToken);
            TempData.PutSuccess("Le bien a ete mis a jour.");
            return RedirectToAction(nameof(Edit), new { id = property.Id });
        }
        catch (AppException exception)
        {
            ModelState.AddModelError(string.Empty, exception.Message);
            await PopulateLocationsAsync(model, cancellationToken);
            return View(model);
        }
    }

    [Authorize(Policy = AuthorizationPolicies.RequireAgent)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Publish(Guid id, CancellationToken cancellationToken)
    {
        if (!(await authorizationService.AuthorizeAsync(User, id, AuthorizationPolicies.CanManageProperty)).Succeeded)
        {
            return Forbid();
        }

        try
        {
            await propertyService.PublishAsync(User.GetRequiredUserId(), id, cancellationToken);
            TempData.PutSuccess("Le bien a ete publie.");
        }
        catch (AppException exception)
        {
            TempData.PutError(exception.Message);
        }

        return RedirectToAction(nameof(MyProperties));
    }

    [Authorize(Policy = AuthorizationPolicies.RequireAgent)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Archive(Guid id, CancellationToken cancellationToken)
    {
        if (!(await authorizationService.AuthorizeAsync(User, id, AuthorizationPolicies.CanManageProperty)).Succeeded)
        {
            return Forbid();
        }

        try
        {
            await propertyService.ArchiveAsync(User.GetRequiredUserId(), id, cancellationToken);
            TempData.PutSuccess("Le bien a ete archive.");
        }
        catch (AppException exception)
        {
            TempData.PutError(exception.Message);
        }

        return RedirectToAction(nameof(MyProperties));
    }

    [Authorize(Policy = AuthorizationPolicies.RequireAgent)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAsRented(Guid id, CancellationToken cancellationToken)
    {
        if (!(await authorizationService.AuthorizeAsync(User, id, AuthorizationPolicies.CanManageProperty)).Succeeded)
        {
            return Forbid();
        }

        try
        {
            await propertyService.MarkAsRentedAsync(User.GetRequiredUserId(), id, cancellationToken);
            TempData.PutSuccess("Le bien a ete marque comme loue.");
        }
        catch (AppException exception)
        {
            TempData.PutError(exception.Message);
        }

        return RedirectToAction(nameof(MyProperties));
    }

    [Authorize(Policy = AuthorizationPolicies.RequireAgent)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Restore(Guid id, CancellationToken cancellationToken)
    {
        if (!(await authorizationService.AuthorizeAsync(User, id, AuthorizationPolicies.CanManageProperty)).Succeeded)
        {
            return Forbid();
        }

        try
        {
            await propertyService.RestoreAsync(id, User.GetRequiredUserId(), false, cancellationToken);
            TempData.PutSuccess("Le bien a ete restaure en brouillon.");
        }
        catch (AppException exception)
        {
            TempData.PutError(exception.Message);
        }

        return RedirectToAction(nameof(MyProperties));
    }

    [Authorize(Policy = AuthorizationPolicies.RequireAgent)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteImage(Guid propertyId, Guid imageId, CancellationToken cancellationToken)
    {
        if (!(await authorizationService.AuthorizeAsync(User, propertyId, AuthorizationPolicies.CanManageProperty)).Succeeded)
        {
            return Forbid();
        }

        try
        {
            await propertyImageService.DeleteAsync(User.GetRequiredUserId(), propertyId, imageId, cancellationToken);
            TempData.PutSuccess("L'image a ete supprimee.");
        }
        catch (AppException exception)
        {
            TempData.PutError(exception.Message);
        }

        return RedirectToAction(nameof(Edit), new { id = propertyId });
    }

    [Authorize(Policy = AuthorizationPolicies.RequireAgent)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetCoverImage(Guid propertyId, Guid imageId, CancellationToken cancellationToken)
    {
        if (!(await authorizationService.AuthorizeAsync(User, propertyId, AuthorizationPolicies.CanManageProperty)).Succeeded)
        {
            return Forbid();
        }

        try
        {
            await propertyImageService.SetCoverAsync(User.GetRequiredUserId(), propertyId, imageId, cancellationToken);
            TempData.PutSuccess("L'image principale a ete mise a jour.");
        }
        catch (AppException exception)
        {
            TempData.PutError(exception.Message);
        }

        return RedirectToAction(nameof(Edit), new { id = propertyId });
    }

    [Authorize(Policy = AuthorizationPolicies.RequireAgent)]
    [HttpGet]
    public async Task<IActionResult> MyProperties(CancellationToken cancellationToken)
    {
        var properties = await propertyService.GetAgentPropertiesAsync(User.GetRequiredUserId(), cancellationToken);
        return View(properties.Select(x => x.ToViewModel()).ToArray());
    }

    [Authorize(Policy = AuthorizationPolicies.RequireTenant)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddFavorite(Guid propertyId, string? returnUrl, CancellationToken cancellationToken)
    {
        try
        {
            await tenantService.AddFavoriteAsync(User.GetRequiredUserId(), propertyId, cancellationToken);
            TempData.PutSuccess("Le bien a ete ajoute a vos favoris.");
        }
        catch (AppException exception)
        {
            TempData.PutError(exception.Message);
        }

        return RedirectToLocalOrAction(returnUrl, nameof(Details), new { id = propertyId });
    }

    [Authorize(Policy = AuthorizationPolicies.RequireTenant)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SendInquiry(SendInquiryViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            TempData.PutError("Merci de verifier votre demande d'information.");
            return RedirectToAction(nameof(Details), new { id = model.PropertyId });
        }

        try
        {
            await inquiryService.SendAsync(User.GetRequiredUserId(), new SendInquiryRequest(model.PropertyId, model.Subject, model.Message), cancellationToken);
            TempData.PutSuccess("Votre demande d'information a ete envoyee.");
        }
        catch (AppException exception)
        {
            TempData.PutError(exception.Message);
        }

        return RedirectToAction(nameof(Details), new { id = model.PropertyId });
    }

    [Authorize(Policy = AuthorizationPolicies.RequireTenant)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RequestVisit(RequestVisitViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            TempData.PutError("Merci de verifier la date de visite demandee.");
            return RedirectToAction(nameof(Details), new { id = model.PropertyId });
        }

        try
        {
            await visitAppointmentService.RequestAsync(User.GetRequiredUserId(), new RequestVisitAppointmentRequest(model.PropertyId, model.ScheduledAtUtc, model.Notes), cancellationToken);
            TempData.PutSuccess("Votre demande de visite a ete envoyee.");
        }
        catch (AppException exception)
        {
            TempData.PutError(exception.Message);
        }

        return RedirectToAction(nameof(Details), new { id = model.PropertyId });
    }

    [Authorize(Policy = AuthorizationPolicies.RequireTenant)]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Apply(SubmitRentalApplicationViewModel model, CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid)
        {
            TempData.PutError("Merci de verifier votre candidature.");
            return RedirectToAction(nameof(Details), new { id = model.PropertyId });
        }

        try
        {
            await rentalApplicationService.SubmitAsync(User.GetRequiredUserId(), new SubmitRentalApplicationRequest(model.PropertyId, model.CoverLetter, model.RequestedMoveInDateUtc), cancellationToken);
            TempData.PutSuccess("Votre candidature a ete soumise.");
        }
        catch (AppException exception)
        {
            TempData.PutError(exception.Message);
        }

        return RedirectToAction(nameof(Details), new { id = model.PropertyId });
    }

    private async Task PopulateLocationsAsync(PropertyFormViewModel model, CancellationToken cancellationToken)
    {
        model.Cities = (await lookupService.GetCitiesAsync(cancellationToken)).ToViewModels();
        model.Districts = [];
    }

    private async Task UploadImagesIfAnyAsync(Guid propertyId, IReadOnlyCollection<IFormFile> files, CancellationToken cancellationToken)
    {
        if (files.Count == 0)
        {
            return;
        }

        var uploads = files
            .Where(x => x.Length > 0)
            .Select(x => new FileUploadRequest(x.FileName, x.ContentType, x.Length, x.OpenReadStream()))
            .ToArray();

        try
        {
            await propertyImageService.UploadAsync(User.GetRequiredUserId(), propertyId, uploads, cancellationToken);
        }
        finally
        {
            foreach (var upload in uploads)
            {
                await upload.Content.DisposeAsync();
            }
        }
    }

    private IActionResult RedirectToLocalOrAction(string? returnUrl, string action, object routeValues)
    {
        if (!string.IsNullOrWhiteSpace(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction(action, routeValues);
    }

    private static PropertyUpsertRequest BuildRequest(PropertyFormViewModel model) =>
        new(model.CityId, null, model.DistrictName, model.Title, model.Description, model.PropertyType, model.ListingType, model.Price, model.SecurityDeposit, model.AreaSqm, model.Bedrooms, model.Bathrooms, model.Floor, model.AddressLine, model.PostalCode, model.AvailableFromUtc);
}
