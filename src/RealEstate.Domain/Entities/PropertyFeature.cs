using RealEstate.Domain.Common;

namespace RealEstate.Domain.Entities;

public class PropertyFeature : AuditableEntity
{
    public Guid PropertyId { get; set; }
    public string FeatureKey { get; set; } = string.Empty;
    public string FeatureValue { get; set; } = string.Empty;
    public Property Property { get; set; } = null!;
}
