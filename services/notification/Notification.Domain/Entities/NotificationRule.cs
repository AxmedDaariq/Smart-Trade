namespace Notification.Domain.Entities;

public class NotificationRule
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }
    
    public string RuleReference { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    
    public string SourceService { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    
    public string NotificationType { get; set; } = string.Empty;
    public string Priority { get; set; } = "Normal";
    
    public string TemplateReference { get; set; } = string.Empty;
    
    public bool Enabled { get; set; } = true;
    
    public string? ConditionsJson { get; set; } // JSONB - safe declarative logic only
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();
}
