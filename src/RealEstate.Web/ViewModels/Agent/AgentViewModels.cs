using System.ComponentModel.DataAnnotations;
using RealEstate.Domain.Enums;

namespace RealEstate.Web.ViewModels.Agent;

public sealed class AgentProfileViewModel
{
    public Guid Id { get; set; }
    public string AgencyName { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public int YearsOfExperience { get; set; }
    public bool IsApproved { get; set; }
    public DateTime? ApprovalDateUtc { get; set; }
}

public sealed class EditAgentProfileViewModel
{
    [Required, StringLength(200)]
    public string AgencyName { get; set; } = string.Empty;
    [Required, StringLength(100)]
    public string LicenseNumber { get; set; } = string.Empty;
    [StringLength(2000)]
    public string? Bio { get; set; }
    [Range(0, 60)]
    public int YearsOfExperience { get; set; }
}

public sealed class AgentInquiryListItemViewModel
{
    public Guid Id { get; set; }
    public string PropertyTitle { get; set; } = string.Empty;
    public string TenantDisplayName { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public InquiryStatus Status { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}

public sealed class AgentVisitListItemViewModel
{
    public Guid Id { get; set; }
    public string PropertyTitle { get; set; } = string.Empty;
    public DateTime ScheduledAtUtc { get; set; }
    public VisitAppointmentStatus Status { get; set; }
    public string CounterpartyName { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public sealed class AgentApplicationListItemViewModel
{
    public Guid Id { get; set; }
    public string PropertyTitle { get; set; } = string.Empty;
    public RentalApplicationStatus Status { get; set; }
    public DateTime? RequestedMoveInDateUtc { get; set; }
    public DateTime? SubmittedAtUtc { get; set; }
}
