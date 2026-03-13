using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.Entities;

namespace RealEstate.Infrastructure.Persistence.Configurations;

public sealed class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> builder)
    {
        builder.ToTable("Reviews");
        builder.ConfigureAuditableEntity();
        builder.Property(x => x.Comment).HasMaxLength(2500);
        builder.HasIndex(x => x.PropertyId);
        builder.HasIndex(x => x.AgentProfileId);
        builder.HasOne(x => x.Property)
            .WithMany(x => x.Reviews)
            .HasForeignKey(x => x.PropertyId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.TenantProfile)
            .WithMany(x => x.ReviewsWritten)
            .HasForeignKey(x => x.TenantProfileId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.AgentProfile)
            .WithMany(x => x.ReviewsReceived)
            .HasForeignKey(x => x.AgentProfileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
