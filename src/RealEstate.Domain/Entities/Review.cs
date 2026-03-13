using RealEstate.Domain.Common;
using RealEstate.Domain.Enums;

namespace RealEstate.Domain.Entities;

public class Review : AuditableEntity
{
    public Guid PropertyId { get; set; }
    public Guid TenantProfileId { get; set; }
    public Guid AgentProfileId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public ReviewStatus Status { get; set; } = ReviewStatus.Visible;
    public Property Property { get; set; } = null!;
    public TenantProfile TenantProfile { get; set; } = null!;
    public AgentProfile AgentProfile { get; set; } = null!;
}
