namespace Notification.Domain.Entities;

public class NotificationProviderReference
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }
    
    public string ChannelType { get; set; } = string.Empty;
    public string ProviderName { get; set; } = string.Empty;
    
    public string ProviderReference { get; set; } = string.Empty;
    public string ConfigurationReference { get; set; } = string.Empty;
    
    public string Status { get; set; } = "Active";
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
