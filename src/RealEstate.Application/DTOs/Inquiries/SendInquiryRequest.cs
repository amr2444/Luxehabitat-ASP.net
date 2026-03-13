namespace RealEstate.Application.DTOs.Inquiries;

public sealed record SendInquiryRequest(
    Guid PropertyId,
    string Subject,
    string Message);
