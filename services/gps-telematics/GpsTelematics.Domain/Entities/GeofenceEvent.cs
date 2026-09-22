using NetTopologySuite.Geometries;

namespace GpsTelematics.Domain.Entities;

public class GeofenceEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid GeofenceId { get; set; }
    
    public Guid GpsDeviceId { get; set; }
    public string VehicleReference { get; set; } = string.Empty;
    public Guid? TrackingSessionId { get; set; }
    
    public string EventType { get; set; } = string.Empty; // Entered, Exited, DwellStarted, DwellExceeded, SpeedViolation, RestrictedArea, RouteDeviation
    
    public Point Position { get; set; } = null!; // geography(Point,4326)
    
    public DateTime OccurredAtUtc { get; set; }
    public DateTime DetectedAtUtc { get; set; } = DateTime.UtcNow;
    
    public long? SequenceNumber { get; set; }
    
    public string Status { get; set; } = "Active";
    public string CorrelationId { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public Geofence Geofence { get; set; } = null!;
    public GpsDevice GpsDevice { get; set; } = null!;
}
