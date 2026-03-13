namespace RealEstate.Web.Policies;

public static class AuthorizationPolicies
{
    public const string RequireAdmin = "RequireAdmin";
    public const string RequireAgent = "RequireAgent";
    public const string RequireTenant = "RequireTenant";
    public const string CanManageProperty = "CanManageProperty";
    public const string CanManageInquiry = "CanManageInquiry";
    public const string CanManageVisit = "CanManageVisit";
    public const string CanManageRentalApplication = "CanManageRentalApplication";
    public const string CanManageFavorite = "CanManageFavorite";
    public const string CanViewInquiry = "CanViewInquiry";
    public const string CanViewVisit = "CanViewVisit";
    public const string CanViewApplication = "CanViewApplication";
    public const string CanViewPropertyDetailsPrivate = "CanViewPropertyDetailsPrivate";
}
