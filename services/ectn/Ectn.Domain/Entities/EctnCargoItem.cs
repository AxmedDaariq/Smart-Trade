namespace Ectn.Domain.Entities;

public class EctnCargoItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EctnCargoId { get; set; }
    
    public int LineNumber { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? HsCode { get; set; }
    
    public decimal Quantity { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    
    public decimal GrossWeight { get; set; }
    public decimal NetWeight { get; set; }
    
    public decimal UnitPrice { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    public decimal TotalValue { get; set; }
    
    public string CountryOfOrigin { get; set; } = string.Empty;
    public string PackageType { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public EctnCargo EctnCargo { get; set; } = null!;
}
