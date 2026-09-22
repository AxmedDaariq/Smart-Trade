namespace Notification.Domain.Entities;

public class NotificationStatusHistory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid NotificationId { get; set; }
    
    public string? PreviousStatus { get; set; }
    public string NewStatus { get; set; } = string.Empty;
    
    public string? Reason { get; set; }
    public string? ActorReference { get; set; }
    
    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
    public string CorrelationId { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public Notification Notification { get; set; } = null!;
}
