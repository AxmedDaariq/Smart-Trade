namespace Customs.Domain.Entities.Configuration;

public class ExchangeRateConfiguration
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public string BaseCurrencyCode { get; set; } = string.Empty;
    public string QuoteCurrencyCode { get; set; } = string.Empty;
    
    public decimal Rate { get; set; }
    
    public DateTime EffectiveAtUtc { get; set; }
    
    public string Source { get; set; } = string.Empty;
    public string SourceReference { get; set; } = string.Empty;
    
    public string Status { get; set; } = "Draft"; // Draft, Approved, Published, Superseded, Archived
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public Guid CreatedBy { get; set; }
}
