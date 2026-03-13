using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.Entities;

namespace RealEstate.Infrastructure.Persistence.Configurations;

public sealed class FavoriteConfiguration : IEntityTypeConfiguration<Favorite>
{
    public void Configure(EntityTypeBuilder<Favorite> builder)
    {
        builder.ToTable("Favorites");
        builder.ConfigureAuditableEntity();
        builder.HasIndex(x => new { x.TenantProfileId, x.PropertyId }).IsUnique();
        builder.HasOne(x => x.TenantProfile)
            .WithMany(x => x.Favorites)
            .HasForeignKey(x => x.TenantProfileId)
            .OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Property)
            .WithMany(x => x.Favorites)
            .HasForeignKey(x => x.PropertyId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
