using Microsoft.AspNetCore.Authorization;
using RealEstate.Application.Interfaces.Services;
using RealEstate.Web.Extensions;

namespace RealEstate.Web.Policies;

public sealed class ResourceOwnershipHandler(IOwnershipAccessService ownershipAccessService)
    : AuthorizationHandler<ResourceOwnershipRequirement, Guid>
{
    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        ResourceOwnershipRequirement requirement,
        Guid resource)
    {
        if (context.User.IsInRole("Admin"))
        {
            context.Succeed(requirement);
            return;
        }

        var userId = context.User.GetRequiredUserId();
        if (await ownershipAccessService.CanManageAsync(userId, requirement.ResourceType, resource))
        {
            context.Succeed(requirement);
        }
    }
}
