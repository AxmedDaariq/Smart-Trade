namespace Notification.Domain.Entities;

public class NotificationDelivery
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid NotificationId { get; set; }
    public Guid RecipientId { get; set; }
    public Guid ChannelId { get; set; }
    
    public string? ProviderReference { get; set; }
    
    public string Status { get; set; } = "Pending"; // Pending, Queued, Sending, Delivered, Failed, Cancelled
    
    public int AttemptCount { get; set; }
    
    public DateTime? LastAttemptAtUtc { get; set; }
    public DateTime? DeliveredAtUtc { get; set; }
    public DateTime? FailedAtUtc { get; set; }
    
    public string? LastErrorCode { get; set; }
    public string? LastErrorMessage { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public Notification Notification { get; set; } = null!;
    public NotificationRecipient Recipient { get; set; } = null!;
    public NotificationChannel Channel { get; set; } = null!;
    public ICollection<NotificationDeliveryAttempt> Attempts { get; set; } = new List<NotificationDeliveryAttempt>();
}
