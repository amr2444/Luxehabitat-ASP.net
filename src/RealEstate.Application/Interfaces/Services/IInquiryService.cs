using RealEstate.Application.DTOs.Inquiries;
using RealEstate.Domain.Enums;

namespace RealEstate.Application.Interfaces.Services;

public interface IInquiryService
{
    Task<InquiryDto> SendAsync(string tenantUserId, SendInquiryRequest request, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<InquiryDto>> GetAgentInquiriesAsync(string agentUserId, CancellationToken cancellationToken = default);
    Task<IReadOnlyCollection<InquiryDto>> GetTenantInquiriesAsync(string tenantUserId, CancellationToken cancellationToken = default);
    Task ChangeStatusAsync(string agentUserId, Guid inquiryId, InquiryStatus status, CancellationToken cancellationToken = default);
    Task<InquiryDto> GetAgentInquiryAsync(string agentUserId, Guid inquiryId, CancellationToken cancellationToken = default);
    Task<InquiryDto> GetTenantInquiryAsync(string tenantUserId, Guid inquiryId, CancellationToken cancellationToken = default);
}
