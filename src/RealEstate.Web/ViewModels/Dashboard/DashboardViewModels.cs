using RealEstate.Web.ViewModels.Agent;
using RealEstate.Web.ViewModels.Tenant;

namespace RealEstate.Web.ViewModels.Dashboard;

public sealed class AgentDashboardViewModel
{
    public int TotalProperties { get; set; }
    public int PublishedProperties { get; set; }
    public int PendingInquiries { get; set; }
    public int UpcomingVisits { get; set; }
    public int ActiveApplications { get; set; }
    public IReadOnlyCollection<AgentInquiryListItemViewModel> RecentInquiries { get; set; } = Array.Empty<AgentInquiryListItemViewModel>();
    public IReadOnlyCollection<AgentVisitListItemViewModel> ScheduledVisits { get; set; } = Array.Empty<AgentVisitListItemViewModel>();
}

public sealed class TenantDashboardViewModel
{
    public int FavoritesCount { get; set; }
    public int PendingVisitsCount { get; set; }
    public int ActiveApplicationsCount { get; set; }
    public int UnreadNotificationsCount { get; set; }
    public IReadOnlyCollection<FavoriteListItemViewModel> Favorites { get; set; } = Array.Empty<FavoriteListItemViewModel>();
    public IReadOnlyCollection<TenantVisitListItemViewModel> Visits { get; set; } = Array.Empty<TenantVisitListItemViewModel>();
}

public sealed class AdminDashboardViewModel
{
    public int UsersCount { get; set; }
    public int AgentsCount { get; set; }
    public int ActiveAgentsCount { get; set; }
    public int TenantCount { get; set; }
    public int PropertiesCount { get; set; }
    public int RentalsCount { get; set; }
    public int ReviewsCount { get; set; }
    public IReadOnlyCollection<AdminAgentListItemViewModel> PendingAgents { get; set; } = Array.Empty<AdminAgentListItemViewModel>();
}

public sealed class AdminUserListItemViewModel
{
    public string UserId { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsAgent { get; set; }
    public bool IsTenant { get; set; }
}

public sealed class AdminAgentListItemViewModel
{
    public Guid AgentProfileId { get; set; }
    public string AgencyName { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public bool IsApproved { get; set; }
    public string StatusLabel { get; set; } = string.Empty;
    public int PropertiesCount { get; set; }
    public int ReviewsCount { get; set; }
}

public sealed class AdminPropertyListItemViewModel
{
    public Guid PropertyId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string AgentName { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public bool IsDeleted { get; set; }
}

public sealed class AdminReviewListItemViewModel
{
    public Guid ReviewId { get; set; }
    public string PropertyTitle { get; set; } = string.Empty;
    public string AgentName { get; set; } = string.Empty;
    public int Rating { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Comment { get; set; }
}
