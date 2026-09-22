namespace AnprGate.Domain.Entities;

public class AnprAuditEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid? ActorUserId { get; set; }
    public string? ActorDeviceId { get; set; }
    
    public string Action { get; set; } = string.Empty;
    public string ResourceType { get; set; } = string.Empty;
    public Guid ResourceId { get; set; }
    
    public string BeforeJson { get; set; } = "{}"; // JSONB
    public string AfterJson { get; set; } = "{}"; // JSONB
    
    public string Reason { get; set; } = string.Empty;
    public string? LegalBasis { get; set; }
    
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
    public string CorrelationId { get; set; } = string.Empty;
    
    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
}
