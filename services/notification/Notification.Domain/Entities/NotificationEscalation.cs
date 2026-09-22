namespace Notification.Domain.Entities;

public class NotificationEscalation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid NotificationId { get; set; }
    
    public int Level { get; set; }
    
    public string TargetType { get; set; } = string.Empty; // User, Role, Organization, ExternalContact
    public string TargetReference { get; set; } = string.Empty;
    
    public int DelayMinutes { get; set; }
    
    public string Status { get; set; } = "Pending"; // Pending, Triggered, Completed, Cancelled
    
    public DateTime? TriggeredAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public Notification Notification { get; set; } = null!;
}
