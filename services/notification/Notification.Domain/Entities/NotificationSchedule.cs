namespace Notification.Domain.Entities;

public class NotificationSchedule
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid NotificationId { get; set; }
    
    public string ScheduleType { get; set; } = "OneTime"; // OneTime, Recurring
    
    public DateTime? ScheduledAtUtc { get; set; }
    public string? CronExpression { get; set; }
    public string? Timezone { get; set; }
    
    public string Status { get; set; } = "Scheduled"; // Scheduled, Processing, Completed, Cancelled, Expired, Failed
    
    public DateTime? NextRunAtUtc { get; set; }
    public DateTime? LastRunAtUtc { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public Notification Notification { get; set; } = null!;
}
