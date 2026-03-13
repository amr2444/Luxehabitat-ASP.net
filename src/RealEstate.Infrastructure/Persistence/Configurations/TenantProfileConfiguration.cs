using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.Entities;

namespace RealEstate.Infrastructure.Persistence.Configurations;

public sealed class TenantProfileConfiguration : IEntityTypeConfiguration<TenantProfile>
{
    public void Configure(EntityTypeBuilder<TenantProfile> builder)
    {
        builder.ToTable("TenantProfiles");
        builder.ConfigureAuditableEntity();
        builder.Property(x => x.Occupation).HasMaxLength(200);
        builder.Property(x => x.MonthlyIncome).HasColumnType("decimal(18,2)");
        builder.Property(x => x.PreferredBudgetMin).HasColumnType("decimal(18,2)");
        builder.Property(x => x.PreferredBudgetMax).HasColumnType("decimal(18,2)");
        builder.HasIndex(x => x.UserId).IsUnique();
        builder.HasOne(x => x.User)
            .WithOne(x => x.TenantProfile)
            .HasForeignKey<TenantProfile>(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.PreferredCity)
            .WithMany(x => x.TenantPreferences)
            .HasForeignKey(x => x.PreferredCityId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
