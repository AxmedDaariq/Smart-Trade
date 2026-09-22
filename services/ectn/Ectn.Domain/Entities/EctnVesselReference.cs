namespace Ectn.Domain.Entities;

public class EctnVesselReference
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EctnShipmentId { get; set; }
    
    public string VesselName { get; set; } = string.Empty;
    public string? ImoNumber { get; set; }
    public string? Mmsi { get; set; }
    public string FlagCountryCode { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public EctnShipment EctnShipment { get; set; } = null!;
}
