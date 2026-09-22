namespace Notification.Domain.Entities;

public class NotificationPreference
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public string UserReference { get; set; } = string.Empty;
    public string NotificationType { get; set; } = string.Empty;
    
    public bool Enabled { get; set; } = true;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();
}
