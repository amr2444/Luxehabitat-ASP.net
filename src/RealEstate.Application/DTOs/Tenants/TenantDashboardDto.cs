namespace RealEstate.Application.DTOs.Tenants;

public sealed record TenantDashboardDto(
    int FavoritesCount,
    int PendingVisitsCount,
    int ActiveApplicationsCount,
    int UnreadNotificationsCount);
