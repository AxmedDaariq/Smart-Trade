namespace Payment.Domain.Entities.Configuration;

public class PaymentMethodConfiguration
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid PaymentGatewayConfigurationId { get; set; }
    
    public string MethodType { get; set; } = string.Empty; // Card, BankTransfer, MobileMoney
    public string MethodName { get; set; } = string.Empty;
    
    public bool IsActive { get; set; } = true;
    
    public decimal ProcessingFeePercentage { get; set; }
    public decimal FixedProcessingFee { get; set; }
    public string FeeCurrencyCode { get; set; } = string.Empty;
    
    public decimal? MinimumAmount { get; set; }
    public decimal? MaximumAmount { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public PaymentGatewayConfiguration Gateway { get; set; } = null!;
}
