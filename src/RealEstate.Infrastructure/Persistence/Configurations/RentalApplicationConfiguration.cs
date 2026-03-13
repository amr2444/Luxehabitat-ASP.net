using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.Entities;

namespace RealEstate.Infrastructure.Persistence.Configurations;

public sealed class RentalApplicationConfiguration : IEntityTypeConfiguration<RentalApplication>
{
    public void Configure(EntityTypeBuilder<RentalApplication> builder)
    {
        builder.ToTable("RentalApplications");
        builder.ConfigureAuditableEntity();
        builder.Property(x => x.CoverLetter).HasMaxLength(4000);
        builder.HasIndex(x => x.PropertyId);
        builder.HasIndex(x => x.AgentProfileId);
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => new { x.PropertyId, x.TenantProfileId });
        builder.HasOne(x => x.Property)
            .WithMany(x => x.RentalApplications)
            .HasForeignKey(x => x.PropertyId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.TenantProfile)
            .WithMany(x => x.RentalApplications)
            .HasForeignKey(x => x.TenantProfileId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.AgentProfile)
            .WithMany(x => x.RentalApplications)
            .HasForeignKey(x => x.AgentProfileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
