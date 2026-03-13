using System.Security.Claims;

namespace RealEstate.Web.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static string GetRequiredUserId(this ClaimsPrincipal user)
    {
        return user.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("Utilisateur authentifie introuvable.");
    }
}
