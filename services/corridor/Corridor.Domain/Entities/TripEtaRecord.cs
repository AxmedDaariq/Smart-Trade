namespace Corridor.Domain.Entities;

public class TripEtaRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid TripId { get; set; }
    
    public DateTime CalculatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime EstimatedArrivalAtUtc { get; set; }
    public DateTime? EstimatedCheckpointArrivalAtUtc { get; set; }
    
    public string Source { get; set; } = string.Empty; // GPS, CorridorEngine, Manual, Integration, Other
    public string CalculationVersion { get; set; } = string.Empty;
    
    public double? Confidence { get; set; }
    public double? DistanceRemainingKm { get; set; }
    public string? Reason { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public Trip Trip { get; set; } = null!;
}
