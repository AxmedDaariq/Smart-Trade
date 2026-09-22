namespace Identity.Domain.Entities;

public class SecurityEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid? UserId { get; set; }
    
    public string EventType { get; set; } = string.Empty;
    public string Severity { get; set; } = "Info"; // Info, Warning, High, Critical
    
    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
    
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    
    public string ResourceType { get; set; } = string.Empty;
    public string ResourceId { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    // Maps to PostgreSQL JSONB
    public string MetadataJson { get; set; } = "{}";
}
