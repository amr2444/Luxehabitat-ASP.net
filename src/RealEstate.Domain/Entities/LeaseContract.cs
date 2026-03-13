using RealEstate.Domain.Common;
using RealEstate.Domain.Enums;

namespace RealEstate.Domain.Entities;

public class LeaseContract : AuditableEntity
{
    public Guid PropertyId { get; set; }
    public Guid TenantProfileId { get; set; }
    public Guid AgentProfileId { get; set; }
    public DateTime StartDateUtc { get; set; }
    public DateTime EndDateUtc { get; set; }
    public decimal MonthlyRent { get; set; }
    public decimal DepositAmount { get; set; }
    public Guid? DocumentId { get; set; }
    public LeaseContractStatus Status { get; set; } = LeaseContractStatus.Draft;
    public Property Property { get; set; } = null!;
    public TenantProfile TenantProfile { get; set; } = null!;
    public AgentProfile AgentProfile { get; set; } = null!;
    public Document? Document { get; set; }
}
