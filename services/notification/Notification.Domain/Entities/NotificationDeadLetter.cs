namespace Notification.Domain.Entities;

public class NotificationDeadLetter
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid NotificationId { get; set; }
    public Guid? DeliveryId { get; set; }
    
    public string? FailureCode { get; set; }
    public string FailureReason { get; set; } = string.Empty;
    
    public int AttemptCount { get; set; }
    
    public DateTime FirstFailedAtUtc { get; set; }
    public DateTime LastFailedAtUtc { get; set; }
    
    public string Status { get; set; } = "Open"; // Open, Investigating, Replayed, Resolved, Discarded
    
    public Guid? ResolvedBy { get; set; }
    public DateTime? ResolvedAtUtc { get; set; }
    public string? ResolutionReason { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
