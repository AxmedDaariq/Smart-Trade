namespace Audit.Domain.Entities;

public class AuditEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }
    
    public string AuditReference { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Severity { get; set; } = "Informational";
    
    public string ActorType { get; set; } = string.Empty;
    public Guid? ActorId { get; set; }
    public string? ActorService { get; set; }
    public string? ActorIpAddress { get; set; }
    public string? ActorUserAgent { get; set; }
    
    public string? SessionReference { get; set; }
    public string? TenantContextSource { get; set; }
    
    public string ResourceType { get; set; } = string.Empty;
    public Guid? ResourceId { get; set; }
    public string? ResourceReference { get; set; }
    
    public string Action { get; set; } = string.Empty;
    public string Outcome { get; set; } = string.Empty;
    public string? Reason { get; set; }
    
    public string SourceService { get; set; } = string.Empty;
    public string SourceEnvironment { get; set; } = string.Empty;
    
    public string CorrelationId { get; set; } = string.Empty;
    public string? CausationId { get; set; }
    public string? RequestId { get; set; }
    public string? TraceId { get; set; }
    
    public DateTime OccurredAtUtc { get; set; }
    public DateTime ReceivedAtUtc { get; set; } = DateTime.UtcNow;
    
    public string DataClassification { get; set; } = "Internal";
    
    public string IntegrityHash { get; set; } = string.Empty;
    public string? PreviousIntegrityHash { get; set; }
    
    public string SchemaVersion { get; set; } = "1.0";
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
