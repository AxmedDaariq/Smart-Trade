namespace Notification.Domain.Entities;

public class NotificationChannel
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid NotificationId { get; set; }
    public Guid RecipientId { get; set; }
    
    public string ChannelType { get; set; } = string.Empty; // InApp, Push, SMS, Email, Webhook
    public string? ProviderReference { get; set; }
    
    public string Status { get; set; } = "Pending"; // Pending, Queued, Sending, Sent, Delivered, Failed, Cancelled, Expired
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public Notification Notification { get; set; } = null!;
    public NotificationRecipient Recipient { get; set; } = null!;
}
