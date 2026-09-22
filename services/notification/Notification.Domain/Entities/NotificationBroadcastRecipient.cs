namespace Notification.Domain.Entities;

public class NotificationBroadcastRecipient
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid BroadcastId { get; set; }
    
    public string UserReference { get; set; } = string.Empty;
    
    public string Status { get; set; } = "Pending";
    
    public Guid? NotificationId { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAtUtc { get; set; }

    public NotificationBroadcast Broadcast { get; set; } = null!;
}
