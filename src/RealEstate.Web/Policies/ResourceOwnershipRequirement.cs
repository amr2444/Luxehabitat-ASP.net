using Microsoft.AspNetCore.Authorization;
using RealEstate.Application.Enums;

namespace RealEstate.Web.Policies;

public sealed class ResourceOwnershipRequirement(ProtectedResourceType resourceType) : IAuthorizationRequirement
{
    public ProtectedResourceType ResourceType { get; } = resourceType;
}
