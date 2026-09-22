namespace Declaration.Domain.Entities;

public class DeclarationCargoItem
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DeclarationCargoId { get; set; }
    
    public int LineNumber { get; set; }
    public string Description { get; set; } = string.Empty;
    
    public string HsCodeDeclared { get; set; } = string.Empty;
    public string CommodityCode { get; set; } = string.Empty;
    public string CountryOfOrigin { get; set; } = string.Empty;
    
    public decimal Quantity { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    
    public decimal GrossWeight { get; set; }
    public decimal NetWeight { get; set; }
    public string WeightUnit { get; set; } = "KGM";
    
    public decimal UnitPrice { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    
    public decimal TotalValue { get; set; }
    
    public int PackageCount { get; set; }
    public string PackageType { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public DeclarationCargo DeclarationCargo { get; set; } = null!;
}
