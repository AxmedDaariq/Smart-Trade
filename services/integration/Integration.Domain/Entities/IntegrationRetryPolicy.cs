namespace Integration.Domain.Entities;

public class IntegrationRetryPolicy
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }
    
    public string PolicyReference { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    
    public int MaxAttempts { get; set; }
    public int InitialDelayMs { get; set; }
    public int MaxDelayMs { get; set; }
    
    public string BackoffType { get; set; } = "Exponential"; // Fixed, Linear, Exponential
    public bool JitterEnabled { get; set; }
    
    public string RetryableStatusCodesJson { get; set; } = "[]"; // JSONB
    public string RetryableErrorCodesJson { get; set; } = "[]"; // JSONB
    
    public string Status { get; set; } = "Active";
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
