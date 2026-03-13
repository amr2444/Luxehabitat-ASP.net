using RealEstate.Domain.Common;
using RealEstate.Domain.Enums;

namespace RealEstate.Domain.Entities;

public class VisitAppointment : AuditableEntity
{
    public Guid PropertyId { get; set; }
    public Guid TenantProfileId { get; set; }
    public Guid AgentProfileId { get; set; }
    public DateTime ScheduledAtUtc { get; set; }
    public VisitAppointmentStatus Status { get; set; } = VisitAppointmentStatus.Pending;
    public string? Notes { get; set; }
    public DateTime? ConfirmedAtUtc { get; set; }
    public Property Property { get; set; } = null!;
    public TenantProfile TenantProfile { get; set; } = null!;
    public AgentProfile AgentProfile { get; set; } = null!;
}
