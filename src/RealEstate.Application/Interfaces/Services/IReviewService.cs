using RealEstate.Application.DTOs.Reviews;

namespace RealEstate.Application.Interfaces.Services;

public interface IReviewService
{
    Task<ReviewDto> CreateAsync(string tenantUserId, CreateReviewRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ReviewDto>> GetPropertyReviewsAsync(Guid propertyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<ReviewDto>> GetAgentReviewsAsync(string agentUserId, CancellationToken cancellationToken = default);
}
