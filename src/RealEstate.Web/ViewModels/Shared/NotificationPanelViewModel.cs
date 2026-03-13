namespace RealEstate.Web.ViewModels.Shared;

public sealed class NotificationPanelViewModel
{
    public int UnreadCount { get; set; }
    public IReadOnlyCollection<NotificationListItemViewModel> Items { get; set; } = Array.Empty<NotificationListItemViewModel>();
}
