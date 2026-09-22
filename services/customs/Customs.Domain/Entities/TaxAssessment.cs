namespace Customs.Domain.Entities;

public class TaxAssessment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CustomsAssessmentId { get; set; }
    
    public Guid ChargeTypeId { get; set; }
    
    public string TaxTypeCode { get; set; } = string.Empty; // e.g. Excise, Levy
    
    public decimal TaxableBase { get; set; }
    public decimal Rate { get; set; }
    public Guid? RateVersionId { get; set; }
    
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    
    public string? LegalBasis { get; set; }
    
    public string CalculationFormula { get; set; } = string.Empty;
    public string CalculationTraceJson { get; set; } = "{}"; // JSONB
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public CustomsAssessment CustomsAssessment { get; set; } = null!;
}
