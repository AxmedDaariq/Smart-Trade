namespace Notification.Domain.Entities;

public class NotificationReadState
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid NotificationId { get; set; }
    
    public string UserReference { get; set; } = string.Empty;
    
    public bool IsRead { get; set; }
    public DateTime? ReadAtUtc { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public Notification Notification { get; set; } = null!;
}
