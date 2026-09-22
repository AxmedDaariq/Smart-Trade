namespace Customs.Domain.Entities.Configuration;

public class CustomsRateVersion
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid CustomsRateId { get; set; }
    
    public int VersionNumber { get; set; }
    
    public string RateType { get; set; } = string.Empty;
    public decimal Rate { get; set; }
    public decimal? MinimumAmount { get; set; }
    public decimal? MaximumAmount { get; set; }
    public string? UnitOfMeasure { get; set; }
    public string? CurrencyCode { get; set; }
    public string? ProcedureCode { get; set; }
    public string? HsCodePattern { get; set; }
    public string? OriginCountryCode { get; set; }
    
    public DateTime EffectiveFromUtc { get; set; }
    public DateTime? EffectiveToUtc { get; set; }
    
    public string? LegalBasis { get; set; }
    public string ChangeReason { get; set; } = string.Empty;
    public string Status { get; set; } = "Draft"; // Draft, UnderReview, Approved, Published, Superseded, Archived
    
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public Guid? ApprovedBy { get; set; }
    public DateTime? ApprovedAtUtc { get; set; }
    
    public Guid? PublishedBy { get; set; }
    public DateTime? PublishedAtUtc { get; set; }
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public CustomsRate CustomsRate { get; set; } = null!;
}
