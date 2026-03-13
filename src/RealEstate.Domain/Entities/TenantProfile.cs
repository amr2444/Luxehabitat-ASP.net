using RealEstate.Domain.Common;

namespace RealEstate.Domain.Entities;

public class TenantProfile : AuditableEntity
{
    public string UserId { get; set; } = string.Empty;
    public string? Occupation { get; set; }
    public decimal? MonthlyIncome { get; set; }
    public Guid? PreferredCityId { get; set; }
    public decimal? PreferredBudgetMin { get; set; }
    public decimal? PreferredBudgetMax { get; set; }
    public ApplicationUser User { get; set; } = null!;
    public City? PreferredCity { get; set; }
    public ICollection<Favorite> Favorites { get; set; } = new List<Favorite>();
    public ICollection<Inquiry> Inquiries { get; set; } = new List<Inquiry>();
    public ICollection<VisitAppointment> VisitAppointments { get; set; } = new List<VisitAppointment>();
    public ICollection<RentalApplication> RentalApplications { get; set; } = new List<RentalApplication>();
    public ICollection<Review> ReviewsWritten { get; set; } = new List<Review>();
    public ICollection<LeaseContract> LeaseContracts { get; set; } = new List<LeaseContract>();
}
