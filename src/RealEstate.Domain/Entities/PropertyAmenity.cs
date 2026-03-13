using RealEstate.Domain.Common;

namespace RealEstate.Domain.Entities;

public class PropertyAmenity : AuditableEntity
{
    public Guid PropertyId { get; set; }
    public string AmenityCode { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public Property Property { get; set; } = null!;
}
