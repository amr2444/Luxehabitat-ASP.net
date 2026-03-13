using RealEstate.Domain.Common;

namespace RealEstate.Domain.Entities;

public class AgentProfile : AuditableEntity
{
    public string UserId { get; set; } = string.Empty;
    public string AgencyName { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public int YearsOfExperience { get; set; }
    public bool IsApproved { get; set; }
    public DateTime? ApprovalDateUtc { get; set; }
    public ApplicationUser User { get; set; } = null!;
    public ICollection<Property> Properties { get; set; } = new List<Property>();
    public ICollection<Inquiry> Inquiries { get; set; } = new List<Inquiry>();
    public ICollection<VisitAppointment> VisitAppointments { get; set; } = new List<VisitAppointment>();
    public ICollection<RentalApplication> RentalApplications { get; set; } = new List<RentalApplication>();
    public ICollection<Review> ReviewsReceived { get; set; } = new List<Review>();
    public ICollection<LeaseContract> LeaseContracts { get; set; } = new List<LeaseContract>();
}
