namespace Notification.Domain.Entities;

public class NotificationAttachment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid NotificationId { get; set; }
    
    public string FileName { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public long Size { get; set; }
    
    public string StorageKey { get; set; } = string.Empty;
    public string Sha256 { get; set; } = string.Empty;
    
    public string MalwareScanStatus { get; set; } = "Pending"; // Pending, Clean, Infected, Skipped
    
    public string Status { get; set; } = "Active"; // Active, Deleted, Quarantined
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public Notification Notification { get; set; } = null!;
}
