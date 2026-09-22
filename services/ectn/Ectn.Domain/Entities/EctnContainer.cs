namespace Ectn.Domain.Entities;

public class EctnContainer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EctnShipmentId { get; set; }
    
    public string ContainerNumber { get; set; } = string.Empty;
    public string NormalizedContainerNumber { get; set; } = string.Empty;
    public string ContainerType { get; set; } = string.Empty;
    public string SealNumber { get; set; } = string.Empty;
    
    public decimal GrossWeight { get; set; }
    public string WeightUnit { get; set; } = "KGM";
    
    public string Status { get; set; } = "Declared"; // Declared, Verified, Loaded, InTransit, Discharged, Released, Cancelled
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public EctnShipment EctnShipment { get; set; } = null!;
}
