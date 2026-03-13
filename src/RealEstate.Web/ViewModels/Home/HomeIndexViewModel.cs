using RealEstate.Web.ViewModels.Property;

namespace RealEstate.Web.ViewModels.Home;

public sealed class HomeIndexViewModel
{
    public PropertyFilterViewModel QuickSearch { get; set; } = new();
    public IReadOnlyCollection<PropertyListItemViewModel> FeaturedProperties { get; set; } = Array.Empty<PropertyListItemViewModel>();
}
