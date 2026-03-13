namespace RealEstate.Application.DTOs.Agents;

public sealed record AgentProfileDto(
    Guid Id,
    string UserId,
    string AgencyName,
    string LicenseNumber,
    string? Bio,
    int YearsOfExperience,
    bool IsApproved,
    DateTime? ApprovalDateUtc);
