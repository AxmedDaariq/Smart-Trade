namespace Integration.Domain.Entities;

public class IntegrationAuditEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }
    
    public Guid? ActorUserId { get; set; }
    public Guid? ActorSystemId { get; set; }
    
    public string Action { get; set; } = string.Empty;
    public string ResourceType { get; set; } = string.Empty;
    public Guid ResourceId { get; set; }
    
    public string? BeforeJson { get; set; } // JSONB
    public string? AfterJson { get; set; } // JSONB
    
    public string? Reason { get; set; }
    
    public string CorrelationId { get; set; } = string.Empty;
    
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    
    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
}
