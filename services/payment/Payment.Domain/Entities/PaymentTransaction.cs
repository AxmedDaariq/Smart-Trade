namespace Payment.Domain.Entities;

public class PaymentTransaction
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PaymentIntentId { get; set; }
    
    public string TransactionType { get; set; } = string.Empty; // Authorization, Capture, Refund, Void, Chargeback
    public string Status { get; set; } = "Pending"; // Pending, Success, Failed
    
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    
    public string GatewayTransactionId { get; set; } = string.Empty;
    public string GatewayResponseCode { get; set; } = string.Empty;
    public string GatewayResponseMessage { get; set; } = string.Empty;
    
    public string PaymentMethodType { get; set; } = string.Empty; // Card, BankTransfer, MobileMoney, Cash, Wallet
    public string PaymentMethodDetailsJson { get; set; } = "{}"; // JSONB
    
    public DateTime ProcessedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public PaymentIntent PaymentIntent { get; set; } = null!;
}
