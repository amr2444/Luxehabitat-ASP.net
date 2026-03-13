namespace RealEstate.Application.DTOs.Admin;

public sealed record AdminDashboardDto(
    int UsersCount,
    int AgentsCount,
    int ActiveAgentsCount,
    int TenantCount,
    int PropertiesCount,
    int RentalsCount,
    int ReviewsCount,
    IReadOnlyCollection<AdminAgentListItemDto> PendingAgents);

public sealed record AdminUserListItemDto(
    string UserId,
    string DisplayName,
    string Email,
    bool IsActive,
    bool IsAgent,
    bool IsTenant);

public sealed record AdminAgentListItemDto(
    Guid AgentProfileId,
    string AgencyName,
    string LicenseNumber,
    bool IsApproved,
    string StatusLabel,
    int PropertiesCount,
    int ReviewsCount);

public sealed record AdminPropertyListItemDto(
    Guid PropertyId,
    string Title,
    string AgentName,
    string City,
    string Status,
    bool IsDeleted);

public sealed record AdminReviewListItemDto(
    Guid ReviewId,
    string PropertyTitle,
    string AgentName,
    int Rating,
    string Status,
    string? Comment);
