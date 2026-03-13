using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.Entities;

namespace RealEstate.Infrastructure.Persistence.Configurations;

public sealed class PropertyAmenityConfiguration : IEntityTypeConfiguration<PropertyAmenity>
{
    public void Configure(EntityTypeBuilder<PropertyAmenity> builder)
    {
        builder.ToTable("PropertyAmenities");
        builder.ConfigureAuditableEntity();
        builder.Property(x => x.AmenityCode).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Label).HasMaxLength(150).IsRequired();
        builder.HasIndex(x => new { x.PropertyId, x.AmenityCode }).IsUnique();
        builder.HasOne(x => x.Property)
            .WithMany(x => x.Amenities)
            .HasForeignKey(x => x.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
