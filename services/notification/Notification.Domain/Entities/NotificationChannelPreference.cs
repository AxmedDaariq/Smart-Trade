namespace Notification.Domain.Entities;

public class NotificationChannelPreference
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public string UserReference { get; set; } = string.Empty;
    public string NotificationType { get; set; } = string.Empty;
    public string ChannelType { get; set; } = string.Empty; // SMS, Email, Push, InApp
    
    public bool Enabled { get; set; } = true;
    public string Priority { get; set; } = "Normal";
    
    public TimeSpan? QuietHoursStart { get; set; }
    public TimeSpan? QuietHoursEnd { get; set; }
    public string Timezone { get; set; } = "UTC";
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();
}
