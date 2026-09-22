namespace Customs.Domain.Entities;

public class CustomsAuditEntry
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid? ActorUserId { get; set; }
    
    public string Action { get; set; } = string.Empty;
    public string ResourceType { get; set; } = string.Empty;
    public string ResourceId { get; set; } = string.Empty;
    
    public string BeforeJson { get; set; } = "{}"; // JSONB
    public string AfterJson { get; set; } = "{}"; // JSONB
    
    public string Reason { get; set; } = string.Empty;
    public string? LegalBasis { get; set; }
    
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    
    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
}
