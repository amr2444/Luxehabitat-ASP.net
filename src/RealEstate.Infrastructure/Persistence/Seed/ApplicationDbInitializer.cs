using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace RealEstate.Infrastructure.Persistence.Seed;

public sealed class ApplicationDbInitializer(
    ApplicationDbContext context,
    IdentitySeeder identitySeeder,
    DomainSeeder domainSeeder,
    ILogger<ApplicationDbInitializer> logger)
{
    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Application des migrations de base de donnees.");
        await context.Database.MigrateAsync(cancellationToken);

        logger.LogInformation("Execution du seed Identity.");
        await identitySeeder.SeedAsync(cancellationToken);

        logger.LogInformation("Execution du seed metier.");
        await domainSeeder.SeedAsync(cancellationToken);
    }
}
