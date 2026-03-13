using Microsoft.EntityFrameworkCore;
using RealEstate.Application.DTOs.Agents;
using RealEstate.Application.DTOs.Inquiries;
using RealEstate.Application.DTOs.Visits;
using RealEstate.Application.Exceptions;
using RealEstate.Application.Interfaces.Persistence;
using RealEstate.Application.Interfaces.Services;
using RealEstate.Domain.Enums;

namespace RealEstate.Application.Services;

public sealed class AgentService(IApplicationDbContext context) : IAgentService
{
    public async Task<AgentDashboardDto> GetDashboardAsync(string agentUserId, CancellationToken cancellationToken = default)
    {
        var profile = await GetAgentProfileEntityAsync(agentUserId, cancellationToken);

        var totalProperties = await context.Properties.CountAsync(x => x.AgentProfileId == profile.Id, cancellationToken);
        var publishedProperties = await context.Properties.CountAsync(x => x.AgentProfileId == profile.Id && x.Status == PropertyStatus.Published, cancellationToken);
        var pendingInquiries = await context.Inquiries.CountAsync(x => x.AgentProfileId == profile.Id && x.Status == InquiryStatus.New, cancellationToken);
        var upcomingVisits = await context.VisitAppointments.CountAsync(x => x.AgentProfileId == profile.Id && x.ScheduledAtUtc >= DateTime.UtcNow, cancellationToken);
        var activeApplications = await context.RentalApplications.CountAsync(x => x.AgentProfileId == profile.Id && x.Status == RentalApplicationStatus.Submitted, cancellationToken);

        return new AgentDashboardDto(totalProperties, publishedProperties, pendingInquiries, upcomingVisits, activeApplications);
    }

    public async Task<AgentProfileDto> GetProfileAsync(string agentUserId, CancellationToken cancellationToken = default)
    {
        var profile = await GetAgentProfileEntityAsync(agentUserId, cancellationToken);
        return Map(profile);
    }

    public async Task<AgentProfileDto> UpdateProfileAsync(string agentUserId, UpdateAgentProfileRequest request, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(request.AgencyName) || string.IsNullOrWhiteSpace(request.LicenseNumber))
        {
            throw new ValidationAppException("Le nom d’agence et le numéro de licence sont requis.");
        }

        var userExists = await context.Users.AnyAsync(x => x.Id == agentUserId, cancellationToken);
        if (!userExists)
        {
            throw new NotFoundAppException("Utilisateur agent introuvable.");
        }

        var profile = await context.AgentProfiles.FirstOrDefaultAsync(x => x.UserId == agentUserId, cancellationToken);
        if (profile is null)
        {
            profile = new Domain.Entities.AgentProfile
            {
                UserId = agentUserId,
                AgencyName = request.AgencyName.Trim(),
                LicenseNumber = request.LicenseNumber.Trim(),
                Bio = request.Bio?.Trim(),
                YearsOfExperience = request.YearsOfExperience,
                CreatedByUserId = agentUserId
            };

            await context.AgentProfiles.AddAsync(profile, cancellationToken);
        }
        else
        {
            profile.AgencyName = request.AgencyName.Trim();
            profile.LicenseNumber = request.LicenseNumber.Trim();
            profile.Bio = request.Bio?.Trim();
            profile.YearsOfExperience = request.YearsOfExperience;
            profile.UpdatedAtUtc = DateTime.UtcNow;
            profile.UpdatedByUserId = agentUserId;
        }

        await context.SaveChangesAsync(cancellationToken);
        return Map(profile);
    }

    public async Task<IReadOnlyCollection<InquiryDto>> GetRecentInquiriesAsync(string agentUserId, int take = 5, CancellationToken cancellationToken = default)
    {
        var profile = await GetAgentProfileEntityAsync(agentUserId, cancellationToken);

        return await context.Inquiries
            .AsNoTracking()
            .Where(x => x.AgentProfileId == profile.Id)
            .OrderByDescending(x => x.CreatedAtUtc)
            .Take(take)
            .Select(x => new InquiryDto(
                x.Id,
                x.PropertyId,
                x.Property.Title,
                x.TenantProfileId,
                $"{x.TenantProfile.User.FirstName} {x.TenantProfile.User.LastName}".Trim(),
                x.Subject,
                x.Message,
                x.Status,
                x.CreatedAtUtc))
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<VisitAppointmentDto>> GetScheduledVisitsAsync(string agentUserId, CancellationToken cancellationToken = default)
    {
        var profile = await GetAgentProfileEntityAsync(agentUserId, cancellationToken);

        return await context.VisitAppointments
            .AsNoTracking()
            .Where(x => x.AgentProfileId == profile.Id)
            .OrderBy(x => x.ScheduledAtUtc)
            .Select(x => new VisitAppointmentDto(
                x.Id,
                x.PropertyId,
                x.Property.Title,
                x.ScheduledAtUtc,
                x.Status,
                x.Notes,
                $"{x.TenantProfile.User.FirstName} {x.TenantProfile.User.LastName}".Trim()))
            .ToListAsync(cancellationToken);
    }

    private async Task<Domain.Entities.AgentProfile> GetAgentProfileEntityAsync(string agentUserId, CancellationToken cancellationToken)
    {
        return await context.AgentProfiles
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.UserId == agentUserId, cancellationToken)
            ?? throw new NotFoundAppException("Profil agent introuvable.");
    }

    private static AgentProfileDto Map(Domain.Entities.AgentProfile profile) =>
        new(profile.Id, profile.UserId, profile.AgencyName, profile.LicenseNumber, profile.Bio, profile.YearsOfExperience, profile.IsApproved, profile.ApprovalDateUtc);
}
