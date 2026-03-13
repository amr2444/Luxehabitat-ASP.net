using Microsoft.EntityFrameworkCore;
using RealEstate.Application.DTOs.Admin;
using RealEstate.Application.Exceptions;
using RealEstate.Application.Interfaces.Persistence;
using RealEstate.Application.Interfaces.Services;
using RealEstate.Domain.Enums;

namespace RealEstate.Application.Services;

public sealed class AdminService(
    IApplicationDbContext context,
    IPropertyService propertyService) : IAdminService
{
    public async Task<AdminDashboardDto> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var pendingAgents = await context.AgentProfiles
            .AsNoTracking()
            .Where(x => !x.IsApproved)
            .OrderBy(x => x.CreatedAtUtc)
            .Take(5)
            .Select(x => new AdminAgentListItemDto(
                x.Id,
                x.AgencyName,
                x.LicenseNumber,
                x.IsApproved,
                x.IsApproved ? "Actif" : "En attente",
                x.Properties.Count,
                x.ReviewsReceived.Count))
            .ToListAsync(cancellationToken);

        return new AdminDashboardDto(
            await context.Users.CountAsync(cancellationToken),
            await context.AgentProfiles.CountAsync(cancellationToken),
            await context.AgentProfiles.CountAsync(x => x.IsApproved, cancellationToken),
            await context.TenantProfiles.CountAsync(cancellationToken),
            await context.Properties.CountAsync(cancellationToken),
            await context.Properties.CountAsync(x => x.Status == PropertyStatus.Rented, cancellationToken),
            await context.Reviews.CountAsync(cancellationToken),
            pendingAgents);
    }

    public async Task<IReadOnlyCollection<AdminUserListItemDto>> GetUsersAsync(CancellationToken cancellationToken = default)
    {
        return await context.Users
            .AsNoTracking()
            .OrderBy(x => x.Email)
            .Take(100)
            .Select(x => new AdminUserListItemDto(
                x.Id,
                $"{x.FirstName} {x.LastName}".Trim(),
                x.Email ?? x.UserName ?? string.Empty,
                x.IsActive,
                x.AgentProfile != null,
                x.TenantProfile != null))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<AdminAgentListItemDto>> GetAgentsAsync(CancellationToken cancellationToken = default)
    {
        return await context.AgentProfiles
            .AsNoTracking()
            .OrderBy(x => x.AgencyName)
            .Take(100)
            .Select(x => new AdminAgentListItemDto(
                x.Id,
                x.AgencyName,
                x.LicenseNumber,
                x.IsApproved,
                x.IsApproved ? "Actif" : "Suspendu",
                x.Properties.Count,
                x.ReviewsReceived.Count))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<AdminPropertyListItemDto>> GetPropertiesAsync(CancellationToken cancellationToken = default)
    {
        return await context.Properties
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(100)
            .Select(x => new AdminPropertyListItemDto(
                x.Id,
                x.Title,
                x.AgentProfile.AgencyName,
                x.City.Name,
                x.Status.ToString(),
                x.IsDeleted))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<AdminReviewListItemDto>> GetReviewsAsync(CancellationToken cancellationToken = default)
    {
        return await context.Reviews
            .AsNoTracking()
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(100)
            .Select(x => new AdminReviewListItemDto(
                x.Id,
                x.Property.Title,
                x.AgentProfile.AgencyName,
                x.Rating,
                x.Status.ToString(),
                x.Comment))
            .ToListAsync(cancellationToken);
    }

    public async Task ApproveAgentAsync(Guid agentProfileId, string adminUserId, CancellationToken cancellationToken = default)
    {
        var agent = await context.AgentProfiles.FirstOrDefaultAsync(x => x.Id == agentProfileId, cancellationToken)
            ?? throw new NotFoundAppException("Agent introuvable.");

        agent.IsApproved = true;
        agent.ApprovalDateUtc = DateTime.UtcNow;
        agent.UpdatedAtUtc = DateTime.UtcNow;
        agent.UpdatedByUserId = adminUserId;
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task SuspendAgentAsync(Guid agentProfileId, string adminUserId, CancellationToken cancellationToken = default)
    {
        var agent = await context.AgentProfiles
            .Include(x => x.User)
            .FirstOrDefaultAsync(x => x.Id == agentProfileId, cancellationToken)
            ?? throw new NotFoundAppException("Agent introuvable.");

        agent.IsApproved = false;
        agent.User.IsActive = false;
        agent.UpdatedAtUtc = DateTime.UtcNow;
        agent.UpdatedByUserId = adminUserId;
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task SuspendPropertyAsync(Guid propertyId, string adminUserId, CancellationToken cancellationToken = default)
        => propertyService.SuspendAsync(propertyId, adminUserId, cancellationToken);

    public Task DeletePropertyAsync(Guid propertyId, string adminUserId, CancellationToken cancellationToken = default)
        => propertyService.SoftDeleteAsync(propertyId, adminUserId, cancellationToken);

    public async Task HideReviewAsync(Guid reviewId, string adminUserId, CancellationToken cancellationToken = default)
    {
        var review = await context.Reviews.FirstOrDefaultAsync(x => x.Id == reviewId, cancellationToken)
            ?? throw new NotFoundAppException("Avis introuvable.");

        review.Status = ReviewStatus.Hidden;
        review.UpdatedAtUtc = DateTime.UtcNow;
        review.UpdatedByUserId = adminUserId;
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteReviewAsync(Guid reviewId, string adminUserId, CancellationToken cancellationToken = default)
    {
        var review = await context.Reviews.FirstOrDefaultAsync(x => x.Id == reviewId, cancellationToken)
            ?? throw new NotFoundAppException("Avis introuvable.");

        review.IsDeleted = true;
        review.DeletedAtUtc = DateTime.UtcNow;
        review.UpdatedAtUtc = DateTime.UtcNow;
        review.UpdatedByUserId = adminUserId;
        await context.SaveChangesAsync(cancellationToken);
    }
}
