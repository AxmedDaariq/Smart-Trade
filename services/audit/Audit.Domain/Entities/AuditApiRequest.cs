namespace Audit.Domain.Entities;

public class AuditApiRequest
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }
    
    public Guid AuditEventId { get; set; }
    
    public string HttpMethod { get; set; } = string.Empty;
    public string Route { get; set; } = string.Empty;
    public string? EndpointCategory { get; set; }
    
    public int StatusCode { get; set; }
    
    public long? RequestSizeBytes { get; set; }
    public long? ResponseSizeBytes { get; set; }
    
    public long? DurationMs { get; set; }
    
    public string? ClientType { get; set; }
    public string? ApiVersion { get; set; }
    
    public string? RateLimitResult { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public AuditEvent AuditEvent { get; set; } = null!;
}
