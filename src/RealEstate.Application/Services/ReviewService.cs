using Microsoft.EntityFrameworkCore;
using RealEstate.Application.DTOs.Reviews;
using RealEstate.Application.Exceptions;
using RealEstate.Application.Interfaces.Persistence;
using RealEstate.Application.Interfaces.Services;
using RealEstate.Domain.Entities;

namespace RealEstate.Application.Services;

public sealed class ReviewService(IApplicationDbContext context) : IReviewService
{
    public async Task<ReviewDto> CreateAsync(string tenantUserId, CreateReviewRequest request, CancellationToken cancellationToken = default)
    {
        if (request.Rating is < 1 or > 5)
        {
            throw new ValidationAppException("La note doit être comprise entre 1 et 5.");
        }

        var tenant = await context.TenantProfiles.FirstOrDefaultAsync(x => x.UserId == tenantUserId, cancellationToken)
            ?? throw new NotFoundAppException("Profil locataire introuvable.");

        var property = await context.Properties.FirstOrDefaultAsync(x => x.Id == request.PropertyId, cancellationToken)
            ?? throw new NotFoundAppException("Bien introuvable.");

        var hasInteraction = await context.Inquiries.AnyAsync(x => x.PropertyId == property.Id && x.TenantProfileId == tenant.Id, cancellationToken)
            || await context.VisitAppointments.AnyAsync(x => x.PropertyId == property.Id && x.TenantProfileId == tenant.Id, cancellationToken)
            || await context.RentalApplications.AnyAsync(x => x.PropertyId == property.Id && x.TenantProfileId == tenant.Id, cancellationToken);

        if (!hasInteraction)
        {
            throw new ForbiddenAppException("Un avis nécessite une interaction préalable sur le bien.");
        }

        var review = new Review
        {
            PropertyId = property.Id,
            TenantProfileId = tenant.Id,
            AgentProfileId = property.AgentProfileId,
            Rating = request.Rating,
            Comment = request.Comment?.Trim(),
            CreatedByUserId = tenantUserId
        };

        await context.Reviews.AddAsync(review, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        return await context.Reviews
            .AsNoTracking()
            .Where(x => x.Id == review.Id)
            .Select(x => new ReviewDto(
                x.Id,
                x.PropertyId,
                x.Rating,
                x.Comment,
                $"{x.TenantProfile.User.FirstName} {x.TenantProfile.User.LastName}".Trim(),
                x.Status,
                x.CreatedAtUtc))
            .FirstAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<ReviewDto>> GetPropertyReviewsAsync(Guid propertyId, CancellationToken cancellationToken = default)
    {
        return await context.Reviews
            .AsNoTracking()
            .Where(x => x.PropertyId == propertyId)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new ReviewDto(
                x.Id,
                x.PropertyId,
                x.Rating,
                x.Comment,
                $"{x.TenantProfile.User.FirstName} {x.TenantProfile.User.LastName}".Trim(),
                x.Status,
                x.CreatedAtUtc))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<ReviewDto>> GetAgentReviewsAsync(string agentUserId, CancellationToken cancellationToken = default)
    {
        var agent = await context.AgentProfiles.FirstOrDefaultAsync(x => x.UserId == agentUserId, cancellationToken)
            ?? throw new NotFoundAppException("Profil agent introuvable.");

        return await context.Reviews
            .AsNoTracking()
            .Where(x => x.AgentProfileId == agent.Id)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Select(x => new ReviewDto(
                x.Id,
                x.PropertyId,
                x.Rating,
                x.Comment,
                $"{x.TenantProfile.User.FirstName} {x.TenantProfile.User.LastName}".Trim(),
                x.Status,
                x.CreatedAtUtc))
            .ToListAsync(cancellationToken);
    }
}
