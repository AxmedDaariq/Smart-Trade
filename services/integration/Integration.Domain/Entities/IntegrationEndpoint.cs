namespace Integration.Domain.Entities;

public class IntegrationEndpoint
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid? TenantId { get; set; }
    
    public Guid SystemId { get; set; }
    
    public string EndpointReference { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    
    public string Direction { get; set; } = string.Empty; // Inbound, Outbound, Bidirectional
    public string Protocol { get; set; } = string.Empty; // REST, SOAP, Webhook, RabbitMQ, AMQP, SFTP, HTTPS, Other
    
    public string? Method { get; set; } // GET, POST, PUT, DELETE, etc.
    public string Path { get; set; } = string.Empty;
    
    public string? ContentType { get; set; }
    
    public string Status { get; set; } = "Active"; // Active, Inactive, Suspended
    
    public int TimeoutMs { get; set; } = 30000;
    public int? RateLimitPerMinute { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public IntegrationSystem System { get; set; } = null!;
}
