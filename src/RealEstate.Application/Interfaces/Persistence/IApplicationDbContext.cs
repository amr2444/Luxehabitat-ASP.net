using Microsoft.EntityFrameworkCore;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Interfaces.Persistence;

public interface IApplicationDbContext
{
    DbSet<ApplicationUser> Users { get; }
    DbSet<AgentProfile> AgentProfiles { get; }
    DbSet<TenantProfile> TenantProfiles { get; }
    DbSet<City> Cities { get; }
    DbSet<District> Districts { get; }
    DbSet<Property> Properties { get; }
    DbSet<PropertyImage> PropertyImages { get; }
    DbSet<PropertyAmenity> PropertyAmenities { get; }
    DbSet<PropertyFeature> PropertyFeatures { get; }
    DbSet<Favorite> Favorites { get; }
    DbSet<Inquiry> Inquiries { get; }
    DbSet<VisitAppointment> VisitAppointments { get; }
    DbSet<RentalApplication> RentalApplications { get; }
    DbSet<Review> Reviews { get; }
    DbSet<Notification> Notifications { get; }
    DbSet<LeaseContract> LeaseContracts { get; }
    DbSet<Document> Documents { get; }
    DbSet<AuditLog> AuditLogs { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
