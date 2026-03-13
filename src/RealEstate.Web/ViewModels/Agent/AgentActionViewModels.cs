using System.ComponentModel.DataAnnotations;
using RealEstate.Domain.Enums;

namespace RealEstate.Web.ViewModels.Agent;

public sealed class UpdateInquiryStatusViewModel
{
    public Guid InquiryId { get; set; }
    [Required]
    public InquiryStatus Status { get; set; }
}

public sealed class RescheduleVisitViewModel
{
    public Guid VisitId { get; set; }
    [Required]
    public DateTime ScheduledAtUtc { get; set; }
}

public sealed class UpdateApplicationStatusViewModel
{
    public Guid ApplicationId { get; set; }
    [Required]
    public RentalApplicationStatus Status { get; set; }
}
