using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.Entities;

namespace RealEstate.Infrastructure.Persistence.Configurations;

public sealed class LeaseContractConfiguration : IEntityTypeConfiguration<LeaseContract>
{
    public void Configure(EntityTypeBuilder<LeaseContract> builder)
    {
        builder.ToTable("LeaseContracts");
        builder.ConfigureAuditableEntity();
        builder.Property(x => x.MonthlyRent).HasColumnType("decimal(18,2)");
        builder.Property(x => x.DepositAmount).HasColumnType("decimal(18,2)");
        builder.HasIndex(x => x.PropertyId);
        builder.HasIndex(x => x.TenantProfileId);
        builder.HasOne(x => x.Property)
            .WithMany(x => x.LeaseContracts)
            .HasForeignKey(x => x.PropertyId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.TenantProfile)
            .WithMany(x => x.LeaseContracts)
            .HasForeignKey(x => x.TenantProfileId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.AgentProfile)
            .WithMany(x => x.LeaseContracts)
            .HasForeignKey(x => x.AgentProfileId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Document)
            .WithOne(x => x.LeaseContract)
            .HasForeignKey<LeaseContract>(x => x.DocumentId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
