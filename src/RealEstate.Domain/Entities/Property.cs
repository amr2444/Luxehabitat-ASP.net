using RealEstate.Domain.Common;
using RealEstate.Domain.Enums;

namespace RealEstate.Domain.Entities;

public class Property : AuditableEntity
{
    public Guid AgentProfileId { get; set; }
    public Guid CityId { get; set; }
    public Guid? DistrictId { get; set; }
    public string? DistrictName { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public PropertyType PropertyType { get; set; }
    public ListingType ListingType { get; set; }
    public PropertyStatus Status { get; set; } = PropertyStatus.Draft;
    public decimal Price { get; set; }
    public decimal? SecurityDeposit { get; set; }
    public decimal AreaSqm { get; set; }
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public int? Floor { get; set; }
    public string AddressLine { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public DateTime? AvailableFromUtc { get; set; }
    public DateTime? PublishedAtUtc { get; set; }
    public AgentProfile AgentProfile { get; set; } = null!;
    public City City { get; set; } = null!;
    public District? District { get; set; }
    public ICollection<PropertyImage> Images { get; set; } = new List<PropertyImage>();
    public ICollection<PropertyAmenity> Amenities { get; set; } = new List<PropertyAmenity>();
    public ICollection<PropertyFeature> Features { get; set; } = new List<PropertyFeature>();
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    public ICollection<Inquiry> Inquiries { get; set; } = new List<Inquiry>();
    public ICollection<VisitAppointment> VisitAppointments { get; set; } = new List<VisitAppointment>();
    public ICollection<RentalApplication> RentalApplications { get; set; } = new List<RentalApplication>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
    public ICollection<Document> Documents { get; set; } = new List<Document>();
    public ICollection<LeaseContract> LeaseContracts { get; set; } = new List<LeaseContract>();
}
