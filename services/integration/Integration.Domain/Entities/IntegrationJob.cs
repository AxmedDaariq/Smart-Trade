namespace Integration.Domain.Entities;

public class IntegrationJob
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }
    
    public string JobReference { get; set; } = string.Empty;
    public string JobType { get; set; } = string.Empty; // Synchronization, BatchImport, BatchExport, Reconciliation, HealthCheck, Retry, Cleanup, Other
    
    public Guid? SystemId { get; set; }
    
    public DateTime ScheduledAtUtc { get; set; }
    public DateTime? StartedAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    
    public string Status { get; set; } = "Pending"; // Pending, Running, Completed, Failed, Cancelled
    
    public int AttemptCount { get; set; }
    
    public string? LastError { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public IntegrationSystem? System { get; set; }
}
