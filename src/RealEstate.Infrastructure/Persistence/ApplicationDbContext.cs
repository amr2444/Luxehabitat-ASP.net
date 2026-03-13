using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RealEstate.Application.Interfaces.Persistence;
using RealEstate.Domain.Entities;
using RealEstate.Infrastructure.Persistence.Configurations;

namespace RealEstate.Infrastructure.Persistence;

public sealed class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>, IApplicationDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<AgentProfile> AgentProfiles => Set<AgentProfile>();
    public DbSet<TenantProfile> TenantProfiles => Set<TenantProfile>();
    public DbSet<City> Cities => Set<City>();
    public DbSet<District> Districts => Set<District>();
    public DbSet<Property> Properties => Set<Property>();
    public DbSet<PropertyImage> PropertyImages => Set<PropertyImage>();
    public DbSet<PropertyAmenity> PropertyAmenities => Set<PropertyAmenity>();
    public DbSet<PropertyFeature> PropertyFeatures => Set<PropertyFeature>();
    public DbSet<Favorite> Favorites => Set<Favorite>();
    public DbSet<Inquiry> Inquiries => Set<Inquiry>();
    public DbSet<VisitAppointment> VisitAppointments => Set<VisitAppointment>();
    public DbSet<RentalApplication> RentalApplications => Set<RentalApplication>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<LeaseContract> LeaseContracts => Set<LeaseContract>();
    public DbSet<Document> Documents => Set<Document>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        builder.ApplySoftDeleteQueryFilters();
    }
}
