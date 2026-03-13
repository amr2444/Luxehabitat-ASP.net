using RealEstate.Domain.Common;

namespace RealEstate.Domain.Entities;

public class Favorite : AuditableEntity
{
    public Guid TenantProfileId { get; set; }
    public Guid PropertyId { get; set; }
    public TenantProfile TenantProfile { get; set; } = null!;
    public Property Property { get; set; } = null!;
}
