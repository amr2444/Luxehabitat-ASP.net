using RealEstate.Domain.Common;

namespace RealEstate.Domain.Entities;

public class AuditLog : AuditableEntity
{
    public string? UserId { get; set; }
    public string EntityName { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string? OldValuesJson { get; set; }
    public string? NewValuesJson { get; set; }
    public string? IpAddress { get; set; }
    public ApplicationUser? User { get; set; }
}
