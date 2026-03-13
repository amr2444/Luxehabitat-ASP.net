using RealEstate.Domain.Common;
using RealEstate.Domain.Enums;

namespace RealEstate.Domain.Entities;

public class Inquiry : AuditableEntity
{
    public Guid PropertyId { get; set; }
    public Guid TenantProfileId { get; set; }
    public Guid AgentProfileId { get; set; }
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public InquiryStatus Status { get; set; } = InquiryStatus.New;
    public DateTime? RespondedAtUtc { get; set; }
    public Property Property { get; set; } = null!;
    public TenantProfile TenantProfile { get; set; } = null!;
    public AgentProfile AgentProfile { get; set; } = null!;
}
