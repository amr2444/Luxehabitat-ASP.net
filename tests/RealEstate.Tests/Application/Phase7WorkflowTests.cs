using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using RealEstate.Application.DTOs.Files;
using RealEstate.Application.DTOs.RentalApplications;
using RealEstate.Application.DTOs.Visits;
using RealEstate.Application.Enums;
using RealEstate.Application.Exceptions;
using RealEstate.Application.Interfaces.Services;
using RealEstate.Application.Services;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;
using RealEstate.Tests.Helpers;
using RealEstate.Web.Policies;

namespace RealEstate.Tests.Application;

public sealed class Phase7WorkflowTests
{
    [Fact]
    public async Task RentalApplicationService_ShouldMarkPropertyAsRented_WhenApplicationIsAccepted()
    {
        await using var context = TestDataBuilder.CreateContext();
        var seeded = await TestDataBuilder.SeedBasicGraphAsync(context);
        var notificationService = new NotificationService(context);
        var propertyService = new PropertyService(context);
        var service = new RentalApplicationService(context, notificationService, propertyService);

        var application = await service.SubmitAsync(seeded.TenantUser.Id, new SubmitRentalApplicationRequest(
            seeded.Property.Id,
            "Candidat serieux",
            DateTime.UtcNow.AddDays(30)));

        await service.ChangeStatusAsync(seeded.AgentUser.Id, application.Id, RentalApplicationStatus.Accepted);

        Assert.Equal(PropertyStatus.Rented, context.Properties.Single().Status);
    }

    [Fact]
    public async Task VisitAppointmentService_ShouldCompleteConfirmedVisit()
    {
        await using var context = TestDataBuilder.CreateContext();
        var seeded = await TestDataBuilder.SeedBasicGraphAsync(context);
        var notificationService = new NotificationService(context);
        var service = new VisitAppointmentService(context, notificationService);

        var visit = await service.RequestAsync(seeded.TenantUser.Id, new RequestVisitAppointmentRequest(
            seeded.Property.Id,
            DateTime.UtcNow.AddDays(2),
            "Visite du matin"));

        await service.ConfirmAsync(seeded.AgentUser.Id, visit.Id);
        await service.CompleteAsync(seeded.AgentUser.Id, visit.Id);

        Assert.Equal(VisitAppointmentStatus.Completed, context.VisitAppointments.Single().Status);
    }

    [Fact]
    public async Task PropertyImageService_ShouldUploadAndDeleteImage()
    {
        await using var context = TestDataBuilder.CreateContext();
        var seeded = await TestDataBuilder.SeedBasicGraphAsync(context);
        var storage = new FakeFileStorageService();
        var service = new PropertyImageService(context, storage);

        await service.UploadAsync(seeded.AgentUser.Id, seeded.Property.Id, [CreatePng("cover.png")]);
        var uploadedImage = context.PropertyImages.OrderByDescending(x => x.CreatedAtUtc).First();

        Assert.True(storage.SavedPaths.Count > 0);

        await service.DeleteAsync(seeded.AgentUser.Id, seeded.Property.Id, uploadedImage.Id);

        Assert.DoesNotContain(context.PropertyImages, x => x.Id == uploadedImage.Id);
        Assert.Single(storage.DeletedPaths);
    }

    [Fact]
    public async Task PropertyImageService_ShouldRejectInvalidImageSignature()
    {
        await using var context = TestDataBuilder.CreateContext();
        var seeded = await TestDataBuilder.SeedBasicGraphAsync(context);
        var storage = new FakeFileStorageService();
        var service = new PropertyImageService(context, storage);

        var invalid = new FileUploadRequest("fake.png", "image/png", 4, new MemoryStream([0x01, 0x02, 0x03, 0x04]));

        await Assert.ThrowsAsync<ValidationAppException>(() => service.UploadAsync(seeded.AgentUser.Id, seeded.Property.Id, [invalid]));
    }

    [Fact]
    public async Task OwnershipAccessService_ShouldPreventTenantFromViewingAnotherTenantApplication()
    {
        await using var context = TestDataBuilder.CreateContext();
        var seeded = await TestDataBuilder.SeedBasicGraphAsync(context);
        var secondTenantUser = new ApplicationUser
        {
            Id = "tenant-2",
            UserName = "tenant2@test.local",
            Email = "tenant2@test.local",
            FirstName = "Tina",
            LastName = "Tenant"
        };
        var secondTenantProfile = new TenantProfile
        {
            UserId = secondTenantUser.Id,
            User = secondTenantUser,
            Occupation = "Designer"
        };

        await context.Users.AddAsync(secondTenantUser);
        await context.TenantProfiles.AddAsync(secondTenantProfile);
        await context.SaveChangesAsync();

        var notificationService = new NotificationService(context);
        var propertyService = new PropertyService(context);
        var appService = new RentalApplicationService(context, notificationService, propertyService);
        var application = await appService.SubmitAsync(seeded.TenantUser.Id, new SubmitRentalApplicationRequest(seeded.Property.Id, null, null));
        var ownershipService = new OwnershipAccessService(context);

        var canView = await ownershipService.CanViewAsync(secondTenantUser.Id, ProtectedResourceType.ViewRentalApplication, application.Id);

        Assert.False(canView);
    }

    [Fact]
    public async Task ApplicationDbContext_ShouldFilterSoftDeletedProperties()
    {
        await using var context = TestDataBuilder.CreateContext();
        var seeded = await TestDataBuilder.SeedBasicGraphAsync(context);

        seeded.Property.IsDeleted = true;
        seeded.Property.DeletedAtUtc = DateTime.UtcNow;
        await context.SaveChangesAsync();

        Assert.Empty(context.Properties.ToList());
    }

    [Fact]
    public async Task ResourceOwnershipHandler_ShouldSucceedForOwner()
    {
        await using var context = TestDataBuilder.CreateContext();
        var seeded = await TestDataBuilder.SeedBasicGraphAsync(context);
        var ownershipService = new OwnershipAccessService(context);
        var handler = new ResourceOwnershipHandler(ownershipService);
        var requirement = new ResourceOwnershipRequirement(ProtectedResourceType.Property);
        var principal = new ClaimsPrincipal(new ClaimsIdentity(
        [
            new Claim(ClaimTypes.NameIdentifier, seeded.AgentUser.Id)
        ], "Test"));
        var authorizationContext = new AuthorizationHandlerContext([requirement], principal, seeded.Property.Id);

        await handler.HandleAsync(authorizationContext);

        Assert.True(authorizationContext.HasSucceeded);
    }

    private static FileUploadRequest CreatePng(string fileName)
    {
        byte[] bytes = [0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A];
        return new FileUploadRequest(fileName, "image/png", bytes.Length, new MemoryStream(bytes));
    }

    private sealed class FakeFileStorageService : IFileStorageService
    {
        public List<string> SavedPaths { get; } = [];
        public List<string> DeletedPaths { get; } = [];

        public Task<string> SavePropertyImageAsync(Guid propertyId, FileUploadRequest file, CancellationToken cancellationToken = default)
        {
            var path = $"/uploads/properties/{propertyId:N}/{file.FileName}";
            SavedPaths.Add(path);
            return Task.FromResult(path);
        }

        public Task DeletePropertyImageAsync(string imageUrl, CancellationToken cancellationToken = default)
        {
            DeletedPaths.Add(imageUrl);
            return Task.CompletedTask;
        }
    }
}
