using NetTopologySuite.Geometries;

namespace GpsTelematics.Domain.Entities;

public class TrackingAlert
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public string VehicleReference { get; set; } = string.Empty;
    public Guid GpsDeviceId { get; set; }
    public Guid? TrackingSessionId { get; set; }
    
    public string AlertType { get; set; } = string.Empty; // GPSOffline, RouteDeviation, Overspeed, GeofenceViolation, UnexpectedStop, LongIdle, DeviceFailure, LowBattery, SignalLoss, DataQuality, Other
    public string Severity { get; set; } = string.Empty; // Info, Warning, High, Critical
    
    public DateTime OccurredAtUtc { get; set; }
    public DateTime DetectedAtUtc { get; set; } = DateTime.UtcNow;
    
    public Point? Position { get; set; } // geography(Point,4326)
    
    public string Status { get; set; } = "Open"; // Open, Acknowledged, Resolved, Dismissed
    public string Description { get; set; } = string.Empty;
    
    public Guid? AcknowledgedBy { get; set; }
    public DateTime? AcknowledgedAtUtc { get; set; }
    
    public Guid? ResolvedBy { get; set; }
    public DateTime? ResolvedAtUtc { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public GpsDevice GpsDevice { get; set; } = null!;
}
