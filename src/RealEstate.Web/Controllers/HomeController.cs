using Microsoft.AspNetCore.Mvc;
using RealEstate.Application.DTOs.Properties;
using RealEstate.Application.Interfaces.Services;
using RealEstate.Web.Mappings;
using RealEstate.Web.ViewModels.Home;

namespace RealEstate.Web.Controllers;

public sealed class HomeController(
    IPropertyService propertyService,
    ILookupService lookupService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken)
    {
        var featured = await propertyService.GetPublishedAsync(new PropertyFilterRequest(null, null, null, null, null, null, null, 1, 6), cancellationToken);
        var cities = await lookupService.GetCitiesAsync(cancellationToken);

        var model = new HomeIndexViewModel
        {
            FeaturedProperties = featured.Items.Select(x => x.ToViewModel()).ToArray(),
            QuickSearch = { Cities = cities.ToViewModels() }
        };

        return View(model);
    }

    [HttpGet]
    public IActionResult About() => View();

    [HttpGet]
    public IActionResult Admins() => View();

    [HttpGet]
    public IActionResult Contact() => View();
}
