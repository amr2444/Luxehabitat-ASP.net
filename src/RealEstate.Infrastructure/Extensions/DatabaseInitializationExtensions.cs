using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RealEstate.Infrastructure.Persistence.Seed;

namespace RealEstate.Infrastructure.Extensions;

public static class DatabaseInitializationExtensions
{
    public static async Task InitialiseDatabaseAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseInitialization");

        try
        {
            var initializer = scope.ServiceProvider.GetRequiredService<ApplicationDbInitializer>();
            await initializer.InitializeAsync();
            logger.LogInformation("Initialisation de la base terminee.");
        }
        catch (Exception exception)
        {
            logger.LogError(exception, "Echec de l'initialisation de la base.");
            throw;
        }
    }
}
