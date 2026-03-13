using RealEstate.Domain.Common;

namespace RealEstate.Domain.Entities;

public class City : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? PostalCode { get; set; }
    public ICollection<District> Districts { get; set; } = new List<District>();
    public ICollection<Property> Properties { get; set; } = new List<Property>();
    public ICollection<TenantProfile> TenantPreferences { get; set; } = new List<TenantProfile>();
}
