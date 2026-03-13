using System.ComponentModel.DataAnnotations;
using RealEstate.Domain.Enums;

namespace RealEstate.Web.ViewModels.Tenant;

public sealed class TenantProfileViewModel
{
    public Guid Id { get; set; }
    public string? Occupation { get; set; }
    public decimal? MonthlyIncome { get; set; }
    public Guid? PreferredCityId { get; set; }
    public decimal? PreferredBudgetMin { get; set; }
    public decimal? PreferredBudgetMax { get; set; }
}

public sealed class EditTenantProfileViewModel
{
    [StringLength(200)]
    public string? Occupation { get; set; }
    [Range(0, 100000000)]
    public decimal? MonthlyIncome { get; set; }
    public Guid? PreferredCityId { get; set; }
    [Range(0, 100000000)]
    public decimal? PreferredBudgetMin { get; set; }
    [Range(0, 100000000)]
    public decimal? PreferredBudgetMax { get; set; }
}

public sealed class FavoriteListItemViewModel
{
    public Guid FavoriteId { get; set; }
    public Guid PropertyId { get; set; }
    public string PropertyTitle { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string City { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
}

public sealed class TenantVisitListItemViewModel
{
    public Guid Id { get; set; }
    public string PropertyTitle { get; set; } = string.Empty;
    public DateTime ScheduledAtUtc { get; set; }
    public VisitAppointmentStatus Status { get; set; }
    public string CounterpartyName { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public sealed class TenantApplicationListItemViewModel
{
    public Guid Id { get; set; }
    public string PropertyTitle { get; set; } = string.Empty;
    public RentalApplicationStatus Status { get; set; }
    public DateTime? RequestedMoveInDateUtc { get; set; }
    public DateTime? SubmittedAtUtc { get; set; }
}

public sealed class TenantInquiryListItemViewModel
{
    public Guid Id { get; set; }
    public string PropertyTitle { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public InquiryStatus Status { get; set; }
    public DateTime CreatedAtUtc { get; set; }
}
