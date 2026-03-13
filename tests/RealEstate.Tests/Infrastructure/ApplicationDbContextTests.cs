using Microsoft.EntityFrameworkCore;
using RealEstate.Infrastructure.Persistence;

namespace RealEstate.Tests.Infrastructure;

public sealed class ApplicationDbContextTests
{
    [Fact]
    public void ApplicationDbContext_ShouldExposeCoreDbSets()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        using var context = new ApplicationDbContext(options);

        Assert.NotNull(context.Properties);
        Assert.NotNull(context.AgentProfiles);
        Assert.NotNull(context.TenantProfiles);
        Assert.NotNull(context.RentalApplications);
    }
}
