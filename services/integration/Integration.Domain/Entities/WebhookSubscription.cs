namespace Integration.Domain.Entities;

public class WebhookSubscription
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid PartnerId { get; set; }
    
    public string WebhookReference { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    
    public string TargetUrl { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
    
    public string SecretReference { get; set; } = string.Empty;
    public string SignatureAlgorithm { get; set; } = "HMAC_SHA256";
    
    public string Status { get; set; } = "Active"; // Active, Suspended, Failing, Inactive
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public IntegrationPartner Partner { get; set; } = null!;
    public ICollection<WebhookDelivery> Deliveries { get; set; } = new List<WebhookDelivery>();
}
