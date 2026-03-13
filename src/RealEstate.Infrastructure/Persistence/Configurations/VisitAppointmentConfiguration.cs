using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RealEstate.Domain.Entities;

namespace RealEstate.Infrastructure.Persistence.Configurations;

public sealed class VisitAppointmentConfiguration : IEntityTypeConfiguration<VisitAppointment>
{
    public void Configure(EntityTypeBuilder<VisitAppointment> builder)
    {
        builder.ToTable("VisitAppointments");
        builder.ConfigureAuditableEntity();
        builder.Property(x => x.Notes).HasMaxLength(2000);
        builder.HasIndex(x => new { x.AgentProfileId, x.ScheduledAtUtc });
        builder.HasIndex(x => new { x.TenantProfileId, x.ScheduledAtUtc });
        builder.HasOne(x => x.Property)
            .WithMany(x => x.VisitAppointments)
            .HasForeignKey(x => x.PropertyId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.TenantProfile)
            .WithMany(x => x.VisitAppointments)
            .HasForeignKey(x => x.TenantProfileId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.AgentProfile)
            .WithMany(x => x.VisitAppointments)
            .HasForeignKey(x => x.AgentProfileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
