using RealEstate.Application.DTOs.Agents;
using RealEstate.Application.DTOs.Inquiries;
using RealEstate.Application.DTOs.Visits;

namespace RealEstate.Application.Interfaces.Services;

public interface IAgentService
{
    Task<AgentDashboardDto> GetDashboardAsync(string agentUserId, CancellationToken cancellationToken = default);
    Task<AgentProfileDto> GetProfileAsync(string agentUserId, CancellationToken cancellationToken = default);
    Task<AgentProfileDto> UpdateProfileAsync(string agentUserId, UpdateAgentProfileRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<InquiryDto>> GetRecentInquiriesAsync(string agentUserId, int take = 5, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<VisitAppointmentDto>> GetScheduledVisitsAsync(string agentUserId, CancellationToken cancellationToken = default);
}
