namespace Customs.Domain.Entities;

public class CustomsValuation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CustomsCaseId { get; set; }
    public Guid DeclarationId { get; set; }
    
    public int AssessmentVersionNumber { get; set; }
    
    public string ValuationMethod { get; set; } = string.Empty; // TransactionValue, IdenticalGoods, SimilarGoods, Deductive, Computed, Fallback
    
    public decimal InvoiceValue { get; set; }
    public decimal FreightValue { get; set; }
    public decimal InsuranceValue { get; set; }
    public decimal Additions { get; set; }
    public decimal Deductions { get; set; }
    
    public decimal ExchangeRate { get; set; }
    public Guid? ExchangeRateId { get; set; }
    
    public decimal ConvertedValue { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public string BaseCurrencyCode { get; set; } = string.Empty;
    
    public string? LegalBasis { get; set; }
    public string Status { get; set; } = "Calculated";
    
    public DateTime CalculatedAtUtc { get; set; } = DateTime.UtcNow;
    public Guid CalculatedBy { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public CustomsCase CustomsCase { get; set; } = null!;
    public ICollection<CustomsValuationComponent> Components { get; set; } = new List<CustomsValuationComponent>();
}
