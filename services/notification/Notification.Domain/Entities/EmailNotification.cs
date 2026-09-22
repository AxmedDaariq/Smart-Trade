namespace Notification.Domain.Entities;

public class EmailNotification
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid NotificationId { get; set; }
    public Guid RecipientId { get; set; }
    
    public string FromReference { get; set; } = string.Empty;
    public string ToReference { get; set; } = string.Empty;
    public string? ReplyToReference { get; set; }
    
    public string Subject { get; set; } = string.Empty;
    public string? BodyHtml { get; set; }
    public string? BodyText { get; set; }
    
    public Guid? TemplateVersionId { get; set; }
    public string? ProviderReference { get; set; }
    
    public string Status { get; set; } = "Created";
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public Notification Notification { get; set; } = null!;
}
