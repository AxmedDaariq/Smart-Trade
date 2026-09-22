namespace AisVessel.Domain.Entities;

public class PortCall
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid PortId { get; set; }
    public Guid VesselId { get; set; }
    public Guid? VoyageId { get; set; }
    
    public string PortCallReference { get; set; } = string.Empty;
    public int SequenceNumber { get; set; }
    public string Purpose { get; set; } = string.Empty; // Loading, Discharging, Bunkering, Transshipment, CrewChange, Inspection, Other
    
    public DateTime? PlannedArrivalAtUtc { get; set; }
    public DateTime? EstimatedArrivalAtUtc { get; set; }
    public DateTime? ActualArrivalAtUtc { get; set; }
    
    public DateTime? PlannedBerthAtUtc { get; set; }
    public DateTime? ActualBerthAtUtc { get; set; }
    
    public DateTime? PlannedDepartureAtUtc { get; set; }
    public DateTime? EstimatedDepartureAtUtc { get; set; }
    public DateTime? ActualDepartureAtUtc { get; set; }
    
    public string Status { get; set; } = "Planned"; // Planned, Approaching, Arrived, Anchored, Berthing, Berthed, Operations, DeparturePending, Departed, Completed, Cancelled
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public Port Port { get; set; } = null!;
    public Vessel Vessel { get; set; } = null!;
    public Voyage? Voyage { get; set; }
}
