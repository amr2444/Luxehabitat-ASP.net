using RealEstate.Domain.Common;

namespace RealEstate.Domain.Entities;

public class District : AuditableEntity
{
    public Guid CityId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public City City { get; set; } = null!;
    public ICollection<Property> Properties { get; set; } = new List<Property>();
}
