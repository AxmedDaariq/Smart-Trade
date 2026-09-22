namespace Customs.Domain.Entities;

public class CustomsValuationComponent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CustomsValuationId { get; set; }
    
    public string ComponentType { get; set; } = string.Empty; // Invoice, Freight, Insurance, Addition, Deduction, Other
    public string Description { get; set; } = string.Empty;
    
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    
    public string SourceReference { get; set; } = string.Empty;
    public int CalculationOrder { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public CustomsValuation CustomsValuation { get; set; } = null!;
}
