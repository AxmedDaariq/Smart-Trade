namespace Payment.Domain.Entities;

public class PaymentIntent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid? CustomerAccountId { get; set; }
    
    public string IntentReference { get; set; } = string.Empty;
    public string SourceSystem { get; set; } = string.Empty; // Declaration, Customs, ECTN
    public string SourceReferenceId { get; set; } = string.Empty; // e.g. CustomsAssessmentId
    
    public decimal AmountRequested { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    
    public string Status { get; set; } = "Created"; // Created, RequiresPaymentMethod, Processing, Succeeded, Canceled, Failed
    
    public Guid? GatewayConfigurationId { get; set; }
    public string GatewayIntentId { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? ExpiresAtUtc { get; set; }
    
    public string MetadataJson { get; set; } = "{}"; // JSONB
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public ICollection<PaymentTransaction> Transactions { get; set; } = new List<PaymentTransaction>();
}
