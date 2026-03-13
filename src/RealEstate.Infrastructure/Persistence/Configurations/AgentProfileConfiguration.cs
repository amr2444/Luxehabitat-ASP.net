using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.Entities;

namespace RealEstate.Infrastructure.Persistence.Configurations;

public sealed class AgentProfileConfiguration : IEntityTypeConfiguration<AgentProfile>
{
    public void Configure(EntityTypeBuilder<AgentProfile> builder)
    {
        builder.ToTable("AgentProfiles");
        builder.ConfigureAuditableEntity();
        builder.Property(x => x.AgencyName).HasMaxLength(200).IsRequired();
        builder.Property(x => x.LicenseNumber).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Bio).HasMaxLength(2000);
        builder.HasIndex(x => x.UserId).IsUnique();
        builder.HasIndex(x => x.LicenseNumber).IsUnique();
        builder.HasOne(x => x.User)
            .WithOne(x => x.AgentProfile)
            .HasForeignKey<AgentProfile>(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
