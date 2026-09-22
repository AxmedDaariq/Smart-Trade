namespace Payment.Domain.Entities.Billing;

public class Receipt
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid InvoiceId { get; set; }
    public Guid PaymentTransactionId { get; set; }
    
    public string ReceiptNumber { get; set; } = string.Empty;
    
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    
    public string PaymentMethodType { get; set; } = string.Empty;
    public string PayerName { get; set; } = string.Empty;
    public string PayerReference { get; set; } = string.Empty;
    
    public DateTime IssuedAtUtc { get; set; } = DateTime.UtcNow;
    
    public string MetadataJson { get; set; } = "{}"; // JSONB
    
    public Invoice Invoice { get; set; } = null!;
}
