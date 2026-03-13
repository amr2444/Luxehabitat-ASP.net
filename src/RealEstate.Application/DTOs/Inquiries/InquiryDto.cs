using RealEstate.Domain.Enums;

namespace RealEstate.Application.DTOs.Inquiries;

public sealed record InquiryDto(
    Guid Id,
    Guid PropertyId,
    string PropertyTitle,
    Guid TenantProfileId,
    string TenantDisplayName,
    string Subject,
    string Message,
    InquiryStatus Status,
    DateTime CreatedAtUtc);
