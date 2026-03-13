using Microsoft.EntityFrameworkCore;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;
using RealEstate.Infrastructure.Persistence;

namespace RealEstate.Tests.Helpers;

internal static class TestDataBuilder
{
    public static ApplicationDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationDbContext(options);
    }

    public static async Task<SeededContext> SeedBasicGraphAsync(ApplicationDbContext context)
    {
        var agentUser = new ApplicationUser
        {
            Id = "agent-1",
            UserName = "agent1@test.local",
            Email = "agent1@test.local",
            FirstName = "Alice",
            LastName = "Agent"
        };

        var secondAgentUser = new ApplicationUser
        {
            Id = "agent-2",
            UserName = "agent2@test.local",
            Email = "agent2@test.local",
            FirstName = "Bob",
            LastName = "Broker"
        };

        var tenantUser = new ApplicationUser
        {
            Id = "tenant-1",
            UserName = "tenant1@test.local",
            Email = "tenant1@test.local",
            FirstName = "Tom",
            LastName = "Tenant"
        };

        var city = new City { Name = "Paris", Slug = "paris", PostalCode = "75000" };
        var district = new District { City = city, Name = "Le Marais", Slug = "le-marais" };

        var agentProfile = new AgentProfile
        {
            UserId = agentUser.Id,
            User = agentUser,
            AgencyName = "Premium Realty",
            LicenseNumber = "LIC-001",
            IsApproved = true
        };

        var secondAgentProfile = new AgentProfile
        {
            UserId = secondAgentUser.Id,
            User = secondAgentUser,
            AgencyName = "Urban Realty",
            LicenseNumber = "LIC-002",
            IsApproved = true
        };

        var tenantProfile = new TenantProfile
        {
            UserId = tenantUser.Id,
            User = tenantUser,
            Occupation = "Engineer"
        };

        var property = new Property
        {
            AgentProfile = agentProfile,
            City = city,
            District = district,
            Title = "Appartement lumineux",
            Slug = "appartement-lumineux",
            Description = "Bien de test",
            PropertyType = PropertyType.Apartment,
            ListingType = ListingType.Rent,
            Status = PropertyStatus.Published,
            Price = 1500,
            AreaSqm = 72,
            Bedrooms = 2,
            Bathrooms = 1,
            AddressLine = "10 rue de Rivoli",
            PostalCode = "75004",
            PublishedAtUtc = DateTime.UtcNow
        };

        property.Images.Add(new PropertyImage
        {
            ImageUrl = "/img/property-1.jpg",
            IsCover = true,
            DisplayOrder = 1
        });

        await context.Users.AddRangeAsync(agentUser, secondAgentUser, tenantUser);
        await context.Cities.AddAsync(city);
        await context.Districts.AddAsync(district);
        await context.AgentProfiles.AddRangeAsync(agentProfile, secondAgentProfile);
        await context.TenantProfiles.AddAsync(tenantProfile);
        await context.Properties.AddAsync(property);
        await context.SaveChangesAsync();

        return new SeededContext(agentUser, secondAgentUser, tenantUser, agentProfile, secondAgentProfile, tenantProfile, city, district, property);
    }
}

internal sealed record SeededContext(
    ApplicationUser AgentUser,
    ApplicationUser SecondAgentUser,
    ApplicationUser TenantUser,
    AgentProfile AgentProfile,
    AgentProfile SecondAgentProfile,
    TenantProfile TenantProfile,
    City City,
    District District,
    Property Property);
