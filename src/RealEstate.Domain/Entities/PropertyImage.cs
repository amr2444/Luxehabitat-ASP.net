using RealEstate.Domain.Common;

namespace RealEstate.Domain.Entities;

public class PropertyImage : AuditableEntity
{
    public Guid PropertyId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsCover { get; set; }
    public Property Property { get; set; } = null!;
}
