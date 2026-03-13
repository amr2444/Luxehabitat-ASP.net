using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using RealEstate.Application.DTOs.Inquiries;
using RealEstate.Application.DTOs.Properties;
using RealEstate.Application.DTOs.RentalApplications;
using RealEstate.Application.DTOs.Visits;
using RealEstate.Application.Exceptions;
using RealEstate.Application.Services;
using RealEstate.Domain.Enums;
using RealEstate.Infrastructure.Persistence.Seed;
using RealEstate.Tests.Helpers;

namespace RealEstate.Tests.Application;

public sealed class ServiceWorkflowTests
{
    [Fact]
    public async Task PropertyService_ShouldCreatePropertyForAgent()
    {
        await using var context = TestDataBuilder.CreateContext();
        var seeded = await TestDataBuilder.SeedBasicGraphAsync(context);
        var service = new PropertyService(context);

        var result = await service.CreateAsync(seeded.AgentUser.Id, new PropertyUpsertRequest(
            seeded.City.Id,
            seeded.District.Id,
            "Triangle d'or",
            "Maison familiale",
            "Grande maison avec jardin",
            PropertyType.House,
            ListingType.Rent,
            2800,
            2800,
            145,
            4,
            2,
            null,
            "20 avenue Victor Hugo",
            "75016",
            DateTime.UtcNow.AddDays(20)));

        Assert.Equal("Maison familiale", result.Title);
        Assert.Equal(PropertyStatus.Draft, result.Status);
        Assert.Equal(2, context.Properties.Count());
    }

    [Fact]
    public async Task PropertyService_ShouldRejectUpdateFromAnotherAgent()
    {
        await using var context = TestDataBuilder.CreateContext();
        var seeded = await TestDataBuilder.SeedBasicGraphAsync(context);
        var service = new PropertyService(context);

        var action = () => service.UpdateAsync(seeded.SecondAgentUser.Id, seeded.Property.Id, new PropertyUpsertRequest(
            seeded.City.Id,
            seeded.District.Id,
            "Quartier pirate",
            "Titre pirate",
            "Description pirate",
            PropertyType.Apartment,
            ListingType.Rent,
            1900,
            1900,
            90,
            3,
            2,
            null,
            "1 rue interdite",
            "75000",
            DateTime.UtcNow.AddDays(10)));

        await Assert.ThrowsAsync<ForbiddenAppException>(action);
    }

    [Fact]
    public async Task TenantService_ShouldAddAndRemoveFavorite()
    {
        await using var context = TestDataBuilder.CreateContext();
        var seeded = await TestDataBuilder.SeedBasicGraphAsync(context);
        var service = new TenantService(context);

        await service.AddFavoriteAsync(seeded.TenantUser.Id, seeded.Property.Id);
        Assert.Single(context.Favorites);

        await service.RemoveFavoriteAsync(seeded.TenantUser.Id, seeded.Property.Id);
        Assert.Empty(context.Favorites);
    }

    [Fact]
    public async Task InquiryService_ShouldSubmitInquiry()
    {
        await using var context = TestDataBuilder.CreateContext();
        var seeded = await TestDataBuilder.SeedBasicGraphAsync(context);
        var notificationService = new NotificationService(context);
        var service = new InquiryService(context, notificationService);

        var inquiry = await service.SendAsync(seeded.TenantUser.Id, new SendInquiryRequest(seeded.Property.Id, "Disponibilite", "Le bien est-il toujours disponible ?"));

        Assert.Equal(seeded.Property.Id, inquiry.PropertyId);
        Assert.Single(context.Inquiries);
        Assert.Single(context.Notifications);
    }

    [Fact]
    public async Task VisitAppointmentService_ShouldSubmitVisit()
    {
        await using var context = TestDataBuilder.CreateContext();
        var seeded = await TestDataBuilder.SeedBasicGraphAsync(context);
        var notificationService = new NotificationService(context);
        var service = new VisitAppointmentService(context, notificationService);

        var visit = await service.RequestAsync(seeded.TenantUser.Id, new RequestVisitAppointmentRequest(
            seeded.Property.Id,
            DateTime.UtcNow.AddDays(2),
            "Disponible en fin de journee"));

        Assert.Equal(VisitAppointmentStatus.Pending, visit.Status);
        Assert.Single(context.VisitAppointments);
    }

    [Fact]
    public async Task RentalApplicationService_ShouldSubmitApplication()
    {
        await using var context = TestDataBuilder.CreateContext();
        var seeded = await TestDataBuilder.SeedBasicGraphAsync(context);
        var notificationService = new NotificationService(context);
        var propertyService = new PropertyService(context);
        var service = new RentalApplicationService(context, notificationService, propertyService);

        var application = await service.SubmitAsync(seeded.TenantUser.Id, new SubmitRentalApplicationRequest(
            seeded.Property.Id,
            "Je suis tres interesse par ce bien.",
            DateTime.UtcNow.AddMonths(1)));

        Assert.Equal(RentalApplicationStatus.Submitted, application.Status);
        Assert.Single(context.RentalApplications);
    }

    [Fact]
    public async Task IdentitySeeder_ShouldCreateRoles()
    {
        await using var context = TestDataBuilder.CreateContext();
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDbContext<RealEstate.Infrastructure.Persistence.ApplicationDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        services.AddIdentity<RealEstate.Domain.Entities.ApplicationUser, Microsoft.AspNetCore.Identity.IdentityRole>()
            .AddEntityFrameworkStores<RealEstate.Infrastructure.Persistence.ApplicationDbContext>()
            .AddDefaultTokenProviders();
        services.Configure<SeedOptions>(options =>
        {
            options.AdminEmail = "admin@test.local";
            options.AdminPassword = "Admin1234!";
        });
        services.AddScoped<IdentitySeeder>();

        await using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<IdentitySeeder>();

        await seeder.SeedAsync();

        var roleManager = scope.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.RoleManager<Microsoft.AspNetCore.Identity.IdentityRole>>();
        Assert.True(await roleManager.RoleExistsAsync("Admin"));
        Assert.True(await roleManager.RoleExistsAsync("Agent"));
        Assert.True(await roleManager.RoleExistsAsync("Tenant"));
    }

    [Fact]
    public async Task IdentitySeeder_ShouldCreateDefaultAdminUser()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDbContext<RealEstate.Infrastructure.Persistence.ApplicationDbContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
        services.AddIdentity<RealEstate.Domain.Entities.ApplicationUser, Microsoft.AspNetCore.Identity.IdentityRole>()
            .AddEntityFrameworkStores<RealEstate.Infrastructure.Persistence.ApplicationDbContext>()
            .AddDefaultTokenProviders();
        services.Configure<SeedOptions>(options =>
        {
            options.AdminEmail = "admin@test.local";
            options.AdminPassword = "Admin1234!";
            options.AdminFirstName = "Platform";
            options.AdminLastName = "Admin";
        });
        services.AddScoped<IdentitySeeder>();

        await using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<IdentitySeeder>();
        var userManager = scope.ServiceProvider.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<RealEstate.Domain.Entities.ApplicationUser>>();

        await seeder.SeedAsync();

        var admin = await userManager.FindByEmailAsync("admin@test.local");
        Assert.NotNull(admin);
        Assert.True(await userManager.IsInRoleAsync(admin!, "Admin"));
    }
}
