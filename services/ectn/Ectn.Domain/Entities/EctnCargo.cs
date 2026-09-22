namespace Ectn.Domain.Entities;

public class EctnCargo
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EctnShipmentId { get; set; }
    
    public string CargoReference { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    public string CommodityCode { get; set; } = string.Empty;
    public string? HsCode { get; set; }
    
    public int PackageCount { get; set; }
    public string PackageType { get; set; } = string.Empty;
    
    public decimal GrossWeight { get; set; }
    public decimal NetWeight { get; set; }
    public string WeightUnit { get; set; } = "KGM";
    
    public decimal Volume { get; set; }
    public string VolumeUnit { get; set; } = "MTQ";
    
    public decimal DeclaredValue { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    
    public string CountryOfOrigin { get; set; } = string.Empty;
    public string MarksAndNumbers { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public EctnShipment EctnShipment { get; set; } = null!;
    public ICollection<EctnCargoItem> Items { get; set; } = new List<EctnCargoItem>();
}
