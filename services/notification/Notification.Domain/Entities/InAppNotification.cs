namespace Notification.Domain.Entities;

public class InAppNotification
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid NotificationId { get; set; }
    public Guid RecipientId { get; set; }
    
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    
    public string? ActionType { get; set; }
    public string? ActionReference { get; set; }
    
    public bool IsRead { get; set; }
    public DateTime? ReadAtUtc { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAtUtc { get; set; }

    public Notification Notification { get; set; } = null!;
}
