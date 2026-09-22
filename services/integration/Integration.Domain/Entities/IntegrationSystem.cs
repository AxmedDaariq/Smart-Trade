namespace Integration.Domain.Entities;

public class IntegrationSystem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }
    
    public Guid? PartnerId { get; set; }
    
    public string SystemReference { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    
    public string SystemType { get; set; } = string.Empty; // API, Webhook, EventBus, FileTransfer, DatabaseAdapter, MessageQueue, Other
    public string Description { get; set; } = string.Empty;
    
    public string Environment { get; set; } = string.Empty; // Development, Testing, Staging, Production, Sandbox
    
    public string? BaseUrl { get; set; }
    
    public string Status { get; set; } = "Active";
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public IntegrationPartner? Partner { get; set; }
    public ICollection<IntegrationEndpoint> Endpoints { get; set; } = new List<IntegrationEndpoint>();
    public ICollection<IntegrationAuthentication> Authentications { get; set; } = new List<IntegrationAuthentication>();
    public ICollection<IntegrationApiVersion> ApiVersions { get; set; } = new List<IntegrationApiVersion>();
}
