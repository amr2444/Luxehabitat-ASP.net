using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.Entities;

namespace RealEstate.Infrastructure.Persistence.Configurations;

public sealed class InquiryConfiguration : IEntityTypeConfiguration<Inquiry>
{
    public void Configure(EntityTypeBuilder<Inquiry> builder)
    {
        builder.ToTable("Inquiries");
        builder.ConfigureAuditableEntity();
        builder.Property(x => x.Subject).HasMaxLength(200).IsRequired();
        builder.Property(x => x.Message).HasMaxLength(3000).IsRequired();
        builder.HasIndex(x => x.PropertyId);
        builder.HasIndex(x => x.AgentProfileId);
        builder.HasIndex(x => x.Status);
        builder.HasOne(x => x.Property)
            .WithMany(x => x.Inquiries)
            .HasForeignKey(x => x.PropertyId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.TenantProfile)
            .WithMany(x => x.Inquiries)
            .HasForeignKey(x => x.TenantProfileId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.AgentProfile)
            .WithMany(x => x.Inquiries)
            .HasForeignKey(x => x.AgentProfileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
