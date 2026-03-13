using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RealEstate.Domain.Entities;

namespace RealEstate.Infrastructure.Persistence.Seed;

public sealed class IdentitySeeder(
    RoleManager<IdentityRole> roleManager,
    UserManager<ApplicationUser> userManager,
    IOptions<SeedOptions> options,
    ILogger<IdentitySeeder> logger)
{
    private static readonly string[] Roles = ["Admin", "Agent", "Tenant"];

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        foreach (var role in Roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                logger.LogInformation("Creation du role {Role}.", role);
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var adminOptions = options.Value;
        var adminUser = await userManager.FindByEmailAsync(adminOptions.AdminEmail);
        if (adminUser is null)
        {
            logger.LogInformation("Creation de l'utilisateur administrateur par defaut.");

            adminUser = new ApplicationUser
            {
                UserName = adminOptions.AdminEmail,
                Email = adminOptions.AdminEmail,
                EmailConfirmed = true,
                FirstName = adminOptions.AdminFirstName,
                LastName = adminOptions.AdminLastName,
                IsActive = true
            };

            var result = await userManager.CreateAsync(adminUser, adminOptions.AdminPassword);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Impossible de creer l'utilisateur admin: {string.Join(", ", result.Errors.Select(x => x.Description))}");
            }
        }

        if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
        {
            logger.LogInformation("Affectation du role Admin a l'utilisateur par defaut.");
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }
}
