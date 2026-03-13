using RealEstate.Application;
using RealEstate.Infrastructure;

namespace RealEstate.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddWebDependencies(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddApplication();
        services.AddInfrastructure(configuration);
        services.AddWebAuthorization();

        return services;
    }
}
