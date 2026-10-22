using System;

namespace Integration.Domain.Entities;

public class WebhookEndpoint
{
    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid IntegrationId { get; private set; }
    
    public string Path { get; private set; } // e.g., "/api/v1/integration/webhooks/bank-xyz"
    public string SignatureHeader { get; private set; } // e.g., "X-Hub-Signature-256"
    public string ExpectedSecretReference { get; private set; } // Vault Path Reference
    
    public bool RequiresTimestamp { get; private set; }
    public string TimestampHeader { get; private set; }
    public int MaxTimestampDriftSeconds { get; private set; }
    
    public bool Active { get; private set; }
    public DateTime CreatedAt { get; private set; }

    protected WebhookEndpoint() { }

    public WebhookEndpoint(Guid tenantId, Guid integrationId, string path, string signatureHeader, string expectedSecretReference)
    {
        Id = Guid.NewGuid();
        TenantId = tenantId;
        IntegrationId = integrationId;
        Path = path;
        SignatureHeader = signatureHeader;
        ExpectedSecretReference = expectedSecretReference;
        RequiresTimestamp = true;
        MaxTimestampDriftSeconds = 300; // 5 mins
        Active = true;
        CreatedAt = DateTime.UtcNow;
    }
}
