namespace Customs.Domain.Entities;

public class DutyAssessment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CustomsAssessmentId { get; set; }
    
    public Guid TariffItemId { get; set; }
    public Guid TariffScheduleVersionId { get; set; }
    
    public string HsCode { get; set; } = string.Empty;
    
    public string RateType { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    
    public decimal TaxableBase { get; set; }
    public decimal Quantity { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    
    public decimal CalculatedAmount { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    
    public string CalculationFormula { get; set; } = string.Empty;
    public string CalculationTraceJson { get; set; } = "{}"; // JSONB
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public CustomsAssessment CustomsAssessment { get; set; } = null!;
}
