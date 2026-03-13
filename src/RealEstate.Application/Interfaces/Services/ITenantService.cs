using RealEstate.Application.DTOs.Tenants;

namespace RealEstate.Application.Interfaces.Services;

public interface ITenantService
{
    Task<TenantDashboardDto> GetDashboardAsync(string tenantUserId, CancellationToken cancellationToken = default);
    Task<TenantProfileDto> GetProfileAsync(string tenantUserId, CancellationToken cancellationToken = default);
    Task<TenantProfileDto> UpdateProfileAsync(string tenantUserId, UpdateTenantProfileRequest request, CancellationToken cancellationToken = default);
    Task AddFavoriteAsync(string tenantUserId, Guid propertyId, CancellationToken cancellationToken = default);
    Task RemoveFavoriteAsync(string tenantUserId, Guid propertyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<FavoriteDto>> GetFavoritesAsync(string tenantUserId, CancellationToken cancellationToken = default);
}
