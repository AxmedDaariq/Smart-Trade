using NetTopologySuite.Geometries;

namespace AisVessel.Domain.Entities;

public class AisPosition
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid VesselId { get; set; }
    public Guid? AisDeviceId { get; set; }
    public string? MmsiNumber { get; set; }
    
    public Point Position { get; set; } = null!; // geography(Point,4326)
    
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    
    public double? SogKnots { get; set; }
    public double? CogDegrees { get; set; }
    public double? HeadingDegrees { get; set; }
    public double? RateOfTurn { get; set; }
    public string? NavigationalStatus { get; set; }
    public double? DraughtMeters { get; set; }
    
    public DateTime TimestampUtc { get; set; }
    public DateTime ReceivedAtUtc { get; set; } = DateTime.UtcNow;
    public long? SequenceNumber { get; set; }
    
    public string? ProviderReference { get; set; }
    
    public string ValidationStatus { get; set; } = "Accepted"; // Accepted, Rejected, Quarantined, Duplicate, OutOfOrder
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public Vessel Vessel { get; set; } = null!;
    public AisDevice? AisDevice { get; set; }
}
