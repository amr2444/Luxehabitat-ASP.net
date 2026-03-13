using RealEstate.Application.DTOs.RentalApplications;
using RealEstate.Domain.Enums;

namespace RealEstate.Application.Interfaces.Services;

public interface IRentalApplicationService
{
    Task<RentalApplicationDto> SubmitAsync(string tenantUserId, SubmitRentalApplicationRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<RentalApplicationDto>> GetAgentApplicationsAsync(string agentUserId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<RentalApplicationDto>> GetTenantApplicationsAsync(string tenantUserId, CancellationToken cancellationToken = default);
    Task ChangeStatusAsync(string agentUserId, Guid applicationId, RentalApplicationStatus status, CancellationToken cancellationToken = default);
    Task<RentalApplicationDto> GetAgentApplicationAsync(string agentUserId, Guid applicationId, CancellationToken cancellationToken = default);
    Task<RentalApplicationDto> GetTenantApplicationAsync(string tenantUserId, Guid applicationId, CancellationToken cancellationToken = default);
}
