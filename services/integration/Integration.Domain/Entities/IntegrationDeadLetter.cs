namespace Integration.Domain.Entities;

public class IntegrationDeadLetter
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid OriginalMessageId { get; set; }
    
    public string MessageType { get; set; } = string.Empty;
    public string SourceSystem { get; set; } = string.Empty;
    public string TargetSystem { get; set; } = string.Empty;
    
    public string? FailureCode { get; set; }
    public string FailureReason { get; set; } = string.Empty;
    
    public int AttemptCount { get; set; }
    
    public DateTime FirstFailedAtUtc { get; set; }
    public DateTime LastFailedAtUtc { get; set; }
    
    public string PayloadHash { get; set; } = string.Empty;
    
    public string Status { get; set; } = "Open"; // Open, Investigating, Replayed, Resolved, Discarded
    
    public Guid? ResolvedBy { get; set; }
    public DateTime? ResolvedAtUtc { get; set; }
    public string? ResolutionReason { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
