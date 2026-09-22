namespace Customs.Domain.Entities.Configuration;

public class CustomsConfigurationAudit
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public string ConfigurationType { get; set; } = string.Empty;
    public Guid ConfigurationId { get; set; }
    public int VersionNumber { get; set; }
    
    public string Action { get; set; } = string.Empty; // Created, Updated, Submitted, Approved, Rejected, Published, Superseded, Archived
    
    public string BeforeJson { get; set; } = "{}"; // JSONB
    public string AfterJson { get; set; } = "{}"; // JSONB
    
    public Guid ActorUserId { get; set; }
    
    public string Reason { get; set; } = string.Empty;
    public string? LegalBasis { get; set; }
    
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    
    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
}
