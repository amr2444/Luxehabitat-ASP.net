using Microsoft.AspNetCore.Identity;

namespace RealEstate.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? ProfilePictureUrl { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime? LastLoginAtUtc { get; set; }
    public AgentProfile? AgentProfile { get; set; }
    public TenantProfile? TenantProfile { get; set; }
    public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    public ICollection<Document> Documents { get; set; } = new List<Document>();
    public ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();
}
