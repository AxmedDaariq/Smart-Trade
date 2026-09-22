namespace AisVessel.Domain.Entities;

public class VesselDelay
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid VesselId { get; set; }
    public Guid? VoyageId { get; set; }
    public Guid? PortCallId { get; set; }
    
    public string DelayType { get; set; } = string.Empty; // Arrival, Berthing, CargoOperation, Departure, Weather, PortCongestion, Technical, Documentation, Other
    
    public DateTime? PlannedAtUtc { get; set; }
    public DateTime? ActualAtUtc { get; set; }
    
    public double DurationMinutes { get; set; }
    
    public string Reason { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public Vessel Vessel { get; set; } = null!;
}
