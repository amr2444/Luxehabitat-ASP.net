using Microsoft.AspNetCore.Authorization;
using RealEstate.Application.Enums;
using RealEstate.Web.Policies;

namespace RealEstate.Web.Extensions;

public static class AuthorizationExtensions
{
    public static IServiceCollection AddWebAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy(AuthorizationPolicies.RequireAdmin, policy => policy.RequireRole("Admin"));
            options.AddPolicy(AuthorizationPolicies.RequireAgent, policy => policy.RequireRole("Agent"));
            options.AddPolicy(AuthorizationPolicies.RequireTenant, policy => policy.RequireRole("Tenant"));
            options.AddPolicy(AuthorizationPolicies.CanManageProperty, policy => policy.Requirements.Add(new ResourceOwnershipRequirement(ProtectedResourceType.Property)));
            options.AddPolicy(AuthorizationPolicies.CanManageInquiry, policy => policy.Requirements.Add(new ResourceOwnershipRequirement(ProtectedResourceType.Inquiry)));
            options.AddPolicy(AuthorizationPolicies.CanManageVisit, policy => policy.Requirements.Add(new ResourceOwnershipRequirement(ProtectedResourceType.Visit)));
            options.AddPolicy(AuthorizationPolicies.CanManageRentalApplication, policy => policy.Requirements.Add(new ResourceOwnershipRequirement(ProtectedResourceType.RentalApplication)));
            options.AddPolicy(AuthorizationPolicies.CanManageFavorite, policy => policy.Requirements.Add(new ResourceOwnershipRequirement(ProtectedResourceType.Favorite)));
            options.AddPolicy(AuthorizationPolicies.CanViewInquiry, policy => policy.Requirements.Add(new ResourceOwnershipRequirement(ProtectedResourceType.ViewInquiry)));
            options.AddPolicy(AuthorizationPolicies.CanViewVisit, policy => policy.Requirements.Add(new ResourceOwnershipRequirement(ProtectedResourceType.ViewVisit)));
            options.AddPolicy(AuthorizationPolicies.CanViewApplication, policy => policy.Requirements.Add(new ResourceOwnershipRequirement(ProtectedResourceType.ViewRentalApplication)));
            options.AddPolicy(AuthorizationPolicies.CanViewPropertyDetailsPrivate, policy => policy.Requirements.Add(new ResourceOwnershipRequirement(ProtectedResourceType.PropertyPrivateDetails)));
        });

        services.AddScoped<IAuthorizationHandler, ResourceOwnershipHandler>();

        return services;
    }
}
