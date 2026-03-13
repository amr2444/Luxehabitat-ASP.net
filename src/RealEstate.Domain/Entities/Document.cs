using RealEstate.Domain.Common;
using RealEstate.Domain.Enums;

namespace RealEstate.Domain.Entities;

public class Document : AuditableEntity
{
    public string? UserId { get; set; }
    public Guid? PropertyId { get; set; }
    public Guid? RentalApplicationId { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string StoragePath { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long SizeInBytes { get; set; }
    public DocumentType DocumentType { get; set; }
    public ApplicationUser? User { get; set; }
    public Property? Property { get; set; }
    public RentalApplication? RentalApplication { get; set; }
    public LeaseContract? LeaseContract { get; set; }
}
