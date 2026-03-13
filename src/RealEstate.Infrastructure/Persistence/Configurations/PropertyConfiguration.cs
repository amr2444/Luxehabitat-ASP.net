using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.Entities;

namespace RealEstate.Infrastructure.Persistence.Configurations;

public sealed class PropertyConfiguration : IEntityTypeConfiguration<Property>
{
    public void Configure(EntityTypeBuilder<Property> builder)
    {
        builder.ToTable("Properties");
        builder.ConfigureAuditableEntity();
        builder.Property(x => x.Title).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Slug).HasMaxLength(220).IsRequired();
        builder.Property(x => x.Description).HasMaxLength(5000).IsRequired();
        builder.Property(x => x.DistrictName).HasMaxLength(150);
        builder.Property(x => x.Price).HasColumnType("decimal(18,2)");
        builder.Property(x => x.SecurityDeposit).HasColumnType("decimal(18,2)");
        builder.Property(x => x.AreaSqm).HasColumnType("decimal(9,2)");
        builder.Property(x => x.AddressLine).HasMaxLength(250).IsRequired();
        builder.Property(x => x.PostalCode).HasMaxLength(20).IsRequired();
        builder.HasIndex(x => x.Slug).IsUnique();
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.CityId);
        builder.HasIndex(x => x.DistrictId);
        builder.HasIndex(x => x.Price);
        builder.HasIndex(x => new { x.CityId, x.PropertyType, x.ListingType, x.Status });
        builder.HasOne(x => x.AgentProfile)
            .WithMany(x => x.Properties)
            .HasForeignKey(x => x.AgentProfileId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.City)
            .WithMany(x => x.Properties)
            .HasForeignKey(x => x.CityId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.District)
            .WithMany(x => x.Properties)
            .HasForeignKey(x => x.DistrictId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
