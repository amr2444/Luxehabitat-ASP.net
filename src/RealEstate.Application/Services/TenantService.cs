using Microsoft.EntityFrameworkCore;
using RealEstate.Application.DTOs.Tenants;
using RealEstate.Application.Exceptions;
using RealEstate.Application.Interfaces.Persistence;
using RealEstate.Application.Interfaces.Services;
using RealEstate.Domain.Entities;
using RealEstate.Domain.Enums;

namespace RealEstate.Application.Services;

public sealed class TenantService(IApplicationDbContext context) : ITenantService
{
    public async Task<TenantDashboardDto> GetDashboardAsync(string tenantUserId, CancellationToken cancellationToken = default)
    {
        var profile = await GetTenantProfileEntityAsync(tenantUserId, cancellationToken);

        var favoritesCount = await context.Favorites.CountAsync(x => x.TenantProfileId == profile.Id, cancellationToken);
        var pendingVisitsCount = await context.VisitAppointments.CountAsync(x => x.TenantProfileId == profile.Id && x.Status == VisitAppointmentStatus.Pending, cancellationToken);
        var activeApplicationsCount = await context.RentalApplications.CountAsync(x => x.TenantProfileId == profile.Id && x.Status == RentalApplicationStatus.Submitted, cancellationToken);
        var unreadNotificationsCount = await context.Notifications.CountAsync(x => x.UserId == tenantUserId && !x.IsRead, cancellationToken);

        return new TenantDashboardDto(favoritesCount, pendingVisitsCount, activeApplicationsCount, unreadNotificationsCount);
    }

    public async Task<TenantProfileDto> GetProfileAsync(string tenantUserId, CancellationToken cancellationToken = default)
    {
        var profile = await GetTenantProfileEntityAsync(tenantUserId, cancellationToken);
        return Map(profile);
    }

    public async Task<TenantProfileDto> UpdateProfileAsync(string tenantUserId, UpdateTenantProfileRequest request, CancellationToken cancellationToken = default)
    {
        var userExists = await context.Users.AnyAsync(x => x.Id == tenantUserId, cancellationToken);
        if (!userExists)
        {
            throw new NotFoundAppException("Utilisateur locataire introuvable.");
        }

        if (request.PreferredCityId.HasValue && !await context.Cities.AnyAsync(x => x.Id == request.PreferredCityId.Value, cancellationToken))
        {
            throw new ValidationAppException("La ville préférée est invalide.");
        }

        var profile = await context.TenantProfiles.FirstOrDefaultAsync(x => x.UserId == tenantUserId, cancellationToken);
        if (profile is null)
        {
            profile = new TenantProfile
            {
                UserId = tenantUserId,
                Occupation = request.Occupation?.Trim(),
                MonthlyIncome = request.MonthlyIncome,
                PreferredCityId = request.PreferredCityId,
                PreferredBudgetMin = request.PreferredBudgetMin,
                PreferredBudgetMax = request.PreferredBudgetMax,
                CreatedByUserId = tenantUserId
            };

            await context.TenantProfiles.AddAsync(profile, cancellationToken);
        }
        else
        {
            profile.Occupation = request.Occupation?.Trim();
            profile.MonthlyIncome = request.MonthlyIncome;
            profile.PreferredCityId = request.PreferredCityId;
            profile.PreferredBudgetMin = request.PreferredBudgetMin;
            profile.PreferredBudgetMax = request.PreferredBudgetMax;
            profile.UpdatedAtUtc = DateTime.UtcNow;
            profile.UpdatedByUserId = tenantUserId;
        }

        await context.SaveChangesAsync(cancellationToken);
        return Map(profile);
    }

    public async Task AddFavoriteAsync(string tenantUserId, Guid propertyId, CancellationToken cancellationToken = default)
    {
        var tenant = await GetTenantProfileEntityAsync(tenantUserId, cancellationToken);
        var property = await context.Properties.FirstOrDefaultAsync(x => x.Id == propertyId, cancellationToken)
            ?? throw new NotFoundAppException("Bien introuvable.");

        if (property.Status != PropertyStatus.Published)
        {
            throw new ValidationAppException("Seuls les biens publiés peuvent être ajoutés en favoris.");
        }

        var exists = await context.Favorites.AnyAsync(x => x.TenantProfileId == tenant.Id && x.PropertyId == propertyId, cancellationToken);
        if (exists)
        {
            return;
        }

        await context.Favorites.AddAsync(new Favorite
        {
            TenantProfileId = tenant.Id,
            PropertyId = propertyId,
            CreatedByUserId = tenantUserId
        }, cancellationToken);

        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task RemoveFavoriteAsync(string tenantUserId, Guid propertyId, CancellationToken cancellationToken = default)
    {
        var tenant = await GetTenantProfileEntityAsync(tenantUserId, cancellationToken);
        var favorite = await context.Favorites.FirstOrDefaultAsync(x => x.TenantProfileId == tenant.Id && x.PropertyId == propertyId, cancellationToken);
        if (favorite is null)
        {
            return;
        }

        context.Favorites.Remove(favorite);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<FavoriteDto>> GetFavoritesAsync(string tenantUserId, CancellationToken cancellationToken = default)
    {
        var tenant = await GetTenantProfileEntityAsync(tenantUserId, cancellationToken);

        return await context.Favorites
            .AsNoTracking()
            .Where(x => x.TenantProfileId == tenant.Id)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new FavoriteDto(
                x.Id,
                x.PropertyId,
                x.Property.Title,
                x.Property.Price,
                x.Property.City.Name,
                x.Property.Images.Where(i => i.IsCover).Select(i => i.ImageUrl).FirstOrDefault()))
            .ToListAsync(cancellationToken);
    }

    private async Task<TenantProfile> GetTenantProfileEntityAsync(string tenantUserId, CancellationToken cancellationToken)
    {
        return await context.TenantProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == tenantUserId, cancellationToken)
            ?? throw new NotFoundAppException("Profil locataire introuvable.");
    }

    private static TenantProfileDto Map(TenantProfile profile) =>
        new(profile.Id, profile.UserId, profile.Occupation, profile.MonthlyIncome, profile.PreferredCityId, profile.PreferredBudgetMin, profile.PreferredBudgetMax);
}
