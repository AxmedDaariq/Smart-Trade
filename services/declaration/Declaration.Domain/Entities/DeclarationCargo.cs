namespace Declaration.Domain.Entities;

public class DeclarationCargo
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DeclarationVersionId { get; set; }
    
    public string CargoReference { get; set; } = string.Empty;
    public string CargoDescription { get; set; } = string.Empty;
    
    public string CommodityCode { get; set; } = string.Empty;
    
    public string CountryOfOrigin { get; set; } = string.Empty;
    public string CountryOfExport { get; set; } = string.Empty;
    
    public int PackageCount { get; set; }
    public string PackageType { get; set; } = string.Empty;
    
    public decimal GrossWeight { get; set; }
    public decimal NetWeight { get; set; }
    public string WeightUnit { get; set; } = "KGM";
    
    public decimal Volume { get; set; }
    public string VolumeUnit { get; set; } = "MTQ";
    
    public decimal DeclaredValue { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    
    public string MarksAndNumbers { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public DeclarationVersion DeclarationVersion { get; set; } = null!;
    public ICollection<DeclarationCargoItem> Items { get; set; } = new List<DeclarationCargoItem>();
}
