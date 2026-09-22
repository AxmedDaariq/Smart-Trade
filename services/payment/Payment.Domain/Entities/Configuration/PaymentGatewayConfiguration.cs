namespace Payment.Domain.Entities.Configuration;

public class PaymentGatewayConfiguration
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public string GatewayName { get; set; } = string.Empty; // Stripe, PayPal, LocalBank
    public string ProviderType { get; set; } = string.Empty;
    
    public bool IsActive { get; set; } = true;
    public bool IsTestMode { get; set; } = false;
    
    public string SupportedCurrenciesJson { get; set; } = "[]"; // JSONB
    
    public string ApiKeySecretRef { get; set; } = string.Empty; // Reference to KeyVault/SecretManager
    public string WebhookSecretRef { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    
    public string AdditionalSettingsJson { get; set; } = "{}"; // JSONB
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public ICollection<PaymentMethodConfiguration> SupportedMethods { get; set; } = new List<PaymentMethodConfiguration>();
}
