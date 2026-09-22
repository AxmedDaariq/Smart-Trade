namespace Ectn.Domain.Entities;

public class EctnShipment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EctnApplicationId { get; set; }
    
    public string ShipmentReference { get; set; } = string.Empty;
    public string ShipmentType { get; set; } = string.Empty; // Import, Export, Transit, ReExport, Other
    
    public string OriginCountryCode { get; set; } = string.Empty;
    public string OriginPortCode { get; set; } = string.Empty;
    public string DestinationCountryCode { get; set; } = string.Empty;
    public string DestinationPortCode { get; set; } = string.Empty;
    public string FinalDestination { get; set; } = string.Empty;
    
    public string TransportMode { get; set; } = string.Empty; // Sea, Road, Rail, Air, Multimodal
    public string Incoterm { get; set; } = string.Empty;
    
    public int TotalPackages { get; set; }
    
    public decimal TotalGrossWeight { get; set; }
    public decimal TotalNetWeight { get; set; }
    public string WeightUnit { get; set; } = "KGM";
    
    public decimal TotalVolume { get; set; }
    public string VolumeUnit { get; set; } = "MTQ";
    
    public decimal DeclaredValue { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    
    public decimal FreightValue { get; set; }
    public string FreightCurrency { get; set; } = string.Empty;
    
    public decimal InsuranceValue { get; set; }
    public string InsuranceCurrency { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public EctnApplication EctnApplication { get; set; } = null!;
    public ICollection<EctnCargo> Cargos { get; set; } = new List<EctnCargo>();
    public ICollection<EctnContainer> Containers { get; set; } = new List<EctnContainer>();
    public ICollection<EctnBillOfLading> BillOfLadings { get; set; } = new List<EctnBillOfLading>();
    public ICollection<EctnVesselReference> VesselReferences { get; set; } = new List<EctnVesselReference>();
    public ICollection<EctnVoyageReference> VoyageReferences { get; set; } = new List<EctnVoyageReference>();
}
