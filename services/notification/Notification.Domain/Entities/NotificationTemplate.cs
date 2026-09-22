namespace Notification.Domain.Entities;

public class NotificationTemplate
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; } // NULL for global templates
    
    public string TemplateReference { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    
    public string NotificationType { get; set; } = string.Empty;
    public string ChannelType { get; set; } = string.Empty; // Email, SMS, Push, InApp
    
    public string LanguageCode { get; set; } = "en"; // en, so, am
    
    public string? Description { get; set; }
    
    public string Status { get; set; } = "Draft"; // Draft, Active, Inactive, Archived
    
    public int CurrentVersionNumber { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public ICollection<NotificationTemplateVersion> Versions { get; set; } = new List<NotificationTemplateVersion>();
}
