namespace Customs.Domain.Entities;

public class CustomsAssessmentLine
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CustomsAssessmentId { get; set; }
    
    public int LineNumber { get; set; }
    public string Description { get; set; } = string.Empty;
    
    public Guid ChargeTypeId { get; set; }
    
    public decimal TaxableBase { get; set; }
    public decimal Rate { get; set; }
    public string RateType { get; set; } = string.Empty;
    
    public decimal Quantity { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    
    public decimal Amount { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    
    public Guid? TariffItemId { get; set; }
    public Guid? RateVersionId { get; set; }
    
    public string CalculationTraceJson { get; set; } = "{}"; // JSONB
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public CustomsAssessment CustomsAssessment { get; set; } = null!;
}
