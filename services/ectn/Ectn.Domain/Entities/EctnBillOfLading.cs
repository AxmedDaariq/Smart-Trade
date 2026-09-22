namespace Ectn.Domain.Entities;

public class EctnBillOfLading
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EctnShipmentId { get; set; }
    
    public string BolNumber { get; set; } = string.Empty;
    public string NormalizedBolNumber { get; set; } = string.Empty;
    public string BolType { get; set; } = string.Empty; // Master, House, Other
    
    public DateTime IssueDateUtc { get; set; }
    public string CarrierName { get; set; } = string.Empty;
    public string OriginalReference { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public EctnShipment EctnShipment { get; set; } = null!;
}
