namespace AnprGate.Domain.Entities;

public class AnprIntegrationRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public string ProviderName { get; set; } = string.Empty;
    public string? ProviderEventId { get; set; }
    
    public string ResourceType { get; set; } = string.Empty;
    public Guid ResourceId { get; set; }
    
    public string Operation { get; set; } = string.Empty;
    public string Direction { get; set; } = string.Empty;
    
    public string Status { get; set; } = "Pending";
    
    public string? PayloadHash { get; set; }
    
    public DateTime ReceivedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ProcessedAtUtc { get; set; }
    
    public int AttemptCount { get; set; }
    public string? LastError { get; set; }
    public string CorrelationId { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
