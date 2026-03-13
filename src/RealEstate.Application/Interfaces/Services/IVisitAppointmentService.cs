using RealEstate.Application.DTOs.Visits;

namespace RealEstate.Application.Interfaces.Services;

public interface IVisitAppointmentService
{
    Task<VisitAppointmentDto> RequestAsync(string tenantUserId, RequestVisitAppointmentRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<VisitAppointmentDto>> GetAgentVisitsAsync(string agentUserId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<VisitAppointmentDto>> GetTenantVisitsAsync(string tenantUserId, CancellationToken cancellationToken = default);
    Task ConfirmAsync(string agentUserId, Guid visitId, CancellationToken cancellationToken = default);
    Task RejectAsync(string agentUserId, Guid visitId, CancellationToken cancellationToken = default);
    Task RescheduleAsync(string agentUserId, Guid visitId, DateTime scheduledAtUtc, CancellationToken cancellationToken = default);
    Task CompleteAsync(string agentUserId, Guid visitId, CancellationToken cancellationToken = default);
    Task<VisitAppointmentDto> GetAgentVisitAsync(string agentUserId, Guid visitId, CancellationToken cancellationToken = default);
    Task<VisitAppointmentDto> GetTenantVisitAsync(string tenantUserId, Guid visitId, CancellationToken cancellationToken = default);
}
