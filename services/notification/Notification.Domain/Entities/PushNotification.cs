namespace Notification.Domain.Entities;

public class PushNotification
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid NotificationId { get; set; }
    public Guid RecipientId { get; set; }
    
    public string? DeviceReference { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    
    public string? PayloadJson { get; set; } // JSONB
    
    public string? ProviderReference { get; set; }
    
    public string Status { get; set; } = "Created";
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? SentAtUtc { get; set; }
    public DateTime? DeliveredAtUtc { get; set; }

    public Notification Notification { get; set; } = null!;
}
