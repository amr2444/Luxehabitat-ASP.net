namespace RealEstate.Application.DTOs.Agents;

public sealed record UpdateAgentProfileRequest(
    string AgencyName,
    string LicenseNumber,
    string? Bio,
    int YearsOfExperience);
