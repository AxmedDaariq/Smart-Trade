namespace Notification.Domain.Entities;

public class NotificationBroadcast
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public string BroadcastReference { get; set; } = string.Empty;
    
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    
    public string NotificationType { get; set; } = string.Empty;
    public string ChannelType { get; set; } = string.Empty;
    
    public string AudienceType { get; set; } = string.Empty; // Tenant, Organization, Role, UserSegment, ExplicitUsers
    public string? AudienceFilterJson { get; set; } // JSONB
    
    public DateTime? ScheduledAtUtc { get; set; }
    public DateTime? StartedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    
    public string Status { get; set; } = "Draft"; // Draft, Scheduled, Processing, Completed, Cancelled, Failed, Expired
    
    public Guid CreatedBy { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public ICollection<NotificationBroadcastRecipient> Recipients { get; set; } = new List<NotificationBroadcastRecipient>();
}
