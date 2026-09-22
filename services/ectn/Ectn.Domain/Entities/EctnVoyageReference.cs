namespace Ectn.Domain.Entities;

public class EctnVoyageReference
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EctnShipmentId { get; set; }
    
    public string VoyageNumber { get; set; } = string.Empty;
    public Guid? VesselReferenceId { get; set; }
    
    public DateTime EstimatedArrivalUtc { get; set; }
    public DateTime EstimatedDepartureUtc { get; set; }
    public DateTime? ActualArrivalUtc { get; set; }
    public DateTime? ActualDepartureUtc { get; set; }
    
    public string OriginPortCode { get; set; } = string.Empty;
    public string DestinationPortCode { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public EctnShipment EctnShipment { get; set; } = null!;
}
