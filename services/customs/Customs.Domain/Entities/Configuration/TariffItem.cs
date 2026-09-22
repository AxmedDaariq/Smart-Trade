namespace Customs.Domain.Entities.Configuration;

public class TariffItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TariffScheduleVersionId { get; set; }
    
    public string HsCode { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string UnitOfMeasure { get; set; } = string.Empty;
    
    public string RateType { get; set; } = string.Empty; // AdValorem, Specific, Compound, Mixed
    public decimal Rate { get; set; }
    public decimal? SecondaryRate { get; set; }
    
    public decimal? MinimumAmount { get; set; }
    public decimal? MaximumAmount { get; set; }
    
    public string? CurrencyCode { get; set; }
    public string? ProcedureCode { get; set; }
    public string? OriginCountryCode { get; set; }
    
    public DateTime EffectiveFromUtc { get; set; }
    public DateTime? EffectiveToUtc { get; set; }
    
    public string? LegalBasis { get; set; }
    public string Status { get; set; } = "Active"; 
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public TariffScheduleVersion TariffScheduleVersion { get; set; } = null!;
}
