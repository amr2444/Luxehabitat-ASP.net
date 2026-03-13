using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.Entities;

namespace RealEstate.Infrastructure.Persistence.Configurations;

public sealed class PropertyFeatureConfiguration : IEntityTypeConfiguration<PropertyFeature>
{
    public void Configure(EntityTypeBuilder<PropertyFeature> builder)
    {
        builder.ToTable("PropertyFeatures");
        builder.ConfigureAuditableEntity();
        builder.Property(x => x.FeatureKey).HasMaxLength(100).IsRequired();
        builder.Property(x => x.FeatureValue).HasMaxLength(500).IsRequired();
        builder.HasIndex(x => new { x.PropertyId, x.FeatureKey });
        builder.HasOne(x => x.Property)
            .WithMany(x => x.Features)
            .HasForeignKey(x => x.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
