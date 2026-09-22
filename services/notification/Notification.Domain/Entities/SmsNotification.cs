namespace Notification.Domain.Entities;

public class SmsNotification
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid NotificationId { get; set; }
    public Guid RecipientId { get; set; }
    
    public string PhoneReference { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    
    public string? ProviderReference { get; set; }
    
    public string Status { get; set; } = "Created";
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? SentAtUtc { get; set; }
    public DateTime? DeliveredAtUtc { get; set; }

    public Notification Notification { get; set; } = null!;
}
