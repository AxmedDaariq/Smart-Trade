using NetTopologySuite.Geometries;

namespace GpsTelematics.Domain.Entities;

public class VehicleLocation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid GpsDeviceId { get; set; }
    
    public string VehicleReference { get; set; } = string.Empty;
    public string? DriverReference { get; set; }
    public Guid? TrackingSessionId { get; set; }
    
    public Point Position { get; set; } = null!; // geography(Point,4326)
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    
    public double? AccuracyMeters { get; set; }
    public double? AltitudeMeters { get; set; }
    public double? SpeedKph { get; set; }
    public double? HeadingDegrees { get; set; }
    
    public DateTime TimestampUtc { get; set; }
    public DateTime ReceivedAtUtc { get; set; } = DateTime.UtcNow;
    public long? SequenceNumber { get; set; }
    
    public string Source { get; set; } = string.Empty;
    public string Status { get; set; } = "Valid"; // Valid, Stale, Invalid, Quarantined
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public GpsDevice GpsDevice { get; set; } = null!;
}
