namespace RealEstate.Application.DTOs.Agents;

public sealed record AgentDashboardDto(
    int TotalProperties,
    int PublishedProperties,
    int PendingInquiries,
    int UpcomingVisits,
    int ActiveApplications);
