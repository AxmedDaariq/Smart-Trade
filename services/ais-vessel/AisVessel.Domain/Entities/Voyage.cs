namespace AisVessel.Domain.Entities;

public class Voyage
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public string VoyageReference { get; set; } = string.Empty;
    public Guid VesselId { get; set; }
    public string? VoyageNumber { get; set; }
    
    public string? OriginPortReference { get; set; }
    public string? DestinationPortReference { get; set; }
    public string? PreviousPortReference { get; set; }
    public string? NextPortReference { get; set; }
    
    public DateTime? PlannedDepartureAtUtc { get; set; }
    public DateTime? PlannedArrivalAtUtc { get; set; }
    
    public DateTime? ActualDepartureAtUtc { get; set; }
    public DateTime? ActualArrivalAtUtc { get; set; }
    
    public string Status { get; set; } = "Planned"; // Planned, Scheduled, Approaching, Arrived, InPort, Departed, Completed, Cancelled, Delayed
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public Vessel Vessel { get; set; } = null!;
}
