namespace Notification.Domain.Entities;

public class Notification
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public string NotificationReference { get; set; } = string.Empty;
    public string NotificationType { get; set; } = string.Empty;
    public string Priority { get; set; } = "Normal"; // Low, Normal, High, Critical
    
    public string Subject { get; set; } = string.Empty;
    public string? BodyPreview { get; set; }
    public string Category { get; set; } = string.Empty;
    
    public string SourceService { get; set; } = string.Empty;
    public string SourceEventType { get; set; } = string.Empty;
    public string? SourceEventId { get; set; }
    
    public string CorrelationId { get; set; } = string.Empty;
    public string? CausationId { get; set; }
    
    public string Status { get; set; } = "Created"; // Created, Queued, Processing, PartiallyDelivered, Delivered, Failed, Cancelled, Expired, DeadLettered
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ScheduledAtUtc { get; set; }
    public DateTime? ExpiresAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public ICollection<NotificationRecipient> Recipients { get; set; } = new List<NotificationRecipient>();
}
