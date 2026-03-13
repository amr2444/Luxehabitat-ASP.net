using System.ComponentModel.DataAnnotations;
using RealEstate.Domain.Enums;
using RealEstate.Web.ViewModels.Shared;

namespace RealEstate.Web.ViewModels.Property;

public sealed class PropertyListItemViewModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? District { get; set; }
    public decimal Price { get; set; }
    public decimal AreaSqm { get; set; }
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public PropertyType PropertyType { get; set; }
    public ListingType ListingType { get; set; }
    public PropertyStatus Status { get; set; }
    public string? CoverImageUrl { get; set; }
    public DateTime? PublishedAtUtc { get; set; }
}

public sealed class PropertyDetailsViewModel
{
    public Guid Id { get; set; }
    public Guid CityId { get; set; }
    public Guid? DistrictId { get; set; }
    public string? DistrictName { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string? District { get; set; }
    public decimal Price { get; set; }
    public decimal? SecurityDeposit { get; set; }
    public decimal AreaSqm { get; set; }
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public int? Floor { get; set; }
    public string AddressLine { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public PropertyType PropertyType { get; set; }
    public ListingType ListingType { get; set; }
    public PropertyStatus Status { get; set; }
    public DateTime? AvailableFromUtc { get; set; }
    public DateTime? PublishedAtUtc { get; set; }
    public string AgentDisplayName { get; set; } = string.Empty;
    public IReadOnlyCollection<PropertyImageViewModel> Images { get; set; } = Array.Empty<PropertyImageViewModel>();
    public IReadOnlyCollection<string> Amenities { get; set; } = Array.Empty<string>();
    public SendInquiryViewModel InquiryForm { get; set; } = new();
    public RequestVisitViewModel VisitForm { get; set; } = new();
    public SubmitRentalApplicationViewModel ApplicationForm { get; set; } = new();
}

public class PropertyFormViewModel
{
    [Required]
    public Guid CityId { get; set; }
    public Guid? DistrictId { get; set; }
    [StringLength(150)]
    public string? DistrictName { get; set; }
    [Required, StringLength(200)]
    public string Title { get; set; } = string.Empty;
    [Required, StringLength(5000)]
    public string Description { get; set; } = string.Empty;
    [Required]
    public PropertyType PropertyType { get; set; }
    [Required]
    public ListingType ListingType { get; set; }
    [Range(1, 100000000)]
    public decimal Price { get; set; }
    [Range(0, 100000000)]
    public decimal? SecurityDeposit { get; set; }
    [Range(1, 100000)]
    public decimal AreaSqm { get; set; }
    [Range(0, 50)]
    public int Bedrooms { get; set; }
    [Range(0, 50)]
    public int Bathrooms { get; set; }
    [Range(0, 200)]
    public int? Floor { get; set; }
    [Required, StringLength(250)]
    public string AddressLine { get; set; } = string.Empty;
    [Required, StringLength(20)]
    public string PostalCode { get; set; } = string.Empty;
    [DataType(DataType.Date)]
    public DateTime? AvailableFromUtc { get; set; }
    public List<IFormFile> NewImages { get; set; } = [];
    public IReadOnlyCollection<PropertyImageViewModel> ExistingImages { get; set; } = Array.Empty<PropertyImageViewModel>();
    public IReadOnlyCollection<LocationOptionViewModel> Cities { get; set; } = Array.Empty<LocationOptionViewModel>();
    public IReadOnlyCollection<LocationOptionViewModel> Districts { get; set; } = Array.Empty<LocationOptionViewModel>();
}

public sealed class CreatePropertyViewModel : PropertyFormViewModel
{
}

public sealed class EditPropertyViewModel : PropertyFormViewModel
{
    public Guid Id { get; set; }
}

public sealed class PropertyFilterViewModel
{
    public Guid? CityId { get; set; }
    public Guid? DistrictId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public PropertyType? PropertyType { get; set; }
    public ListingType? ListingType { get; set; }
    public int? Bedrooms { get; set; }
    public int PageNumber { get; set; } = 1;
    public IReadOnlyCollection<LocationOptionViewModel> Cities { get; set; } = Array.Empty<LocationOptionViewModel>();
    public IReadOnlyCollection<LocationOptionViewModel> Districts { get; set; } = Array.Empty<LocationOptionViewModel>();
}

public sealed class PropertySearchPageViewModel
{
    public PropertyFilterViewModel Filter { get; set; } = new();
    public IReadOnlyCollection<PropertyListItemViewModel> Properties { get; set; } = Array.Empty<PropertyListItemViewModel>();
    public PaginationViewModel Pagination { get; set; } = new();
}

public sealed class LocationOptionViewModel
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public sealed class PropertyImageViewModel
{
    public Guid Id { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public bool IsCover { get; set; }
    public int DisplayOrder { get; set; }
}

public sealed class SendInquiryViewModel
{
    public Guid PropertyId { get; set; }
    [Required, StringLength(200)]
    public string Subject { get; set; } = string.Empty;
    [Required, StringLength(3000)]
    public string Message { get; set; } = string.Empty;
}

public sealed class RequestVisitViewModel
{
    public Guid PropertyId { get; set; }
    [Required]
    public DateTime ScheduledAtUtc { get; set; } = DateTime.UtcNow.AddDays(1);
    [StringLength(2000)]
    public string? Notes { get; set; }
}

public sealed class SubmitRentalApplicationViewModel
{
    public Guid PropertyId { get; set; }
    [StringLength(4000)]
    public string? CoverLetter { get; set; }
    public DateTime? RequestedMoveInDateUtc { get; set; }
}
