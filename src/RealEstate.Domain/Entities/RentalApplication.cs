using RealEstate.Domain.Common;
using RealEstate.Domain.Enums;

namespace RealEstate.Domain.Entities;

public class RentalApplication : AuditableEntity
{
    public Guid PropertyId { get; set; }
    public Guid TenantProfileId { get; set; }
    public Guid AgentProfileId { get; set; }
    public RentalApplicationStatus Status { get; set; } = RentalApplicationStatus.Draft;
    public string? CoverLetter { get; set; }
    public DateTime? RequestedMoveInDateUtc { get; set; }
    public DateTime? SubmittedAtUtc { get; set; }
    public DateTime? ReviewedAtUtc { get; set; }
    public Property Property { get; set; } = null!;
    public TenantProfile TenantProfile { get; set; } = null!;
    public AgentProfile AgentProfile { get; set; } = null!;
    public ICollection<Document> Documents { get; set; } = new List<Document>();
}
