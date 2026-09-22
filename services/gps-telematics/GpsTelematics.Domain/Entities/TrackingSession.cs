using NetTopologySuite.Geometries;

namespace GpsTelematics.Domain.Entities;

public class TrackingSession
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid GpsDeviceId { get; set; }
    
    public string VehicleReference { get; set; } = string.Empty;
    public string? DriverReference { get; set; }
    public string? DispatchReference { get; set; }
    public string? TripReference { get; set; }
    
    public DateTime StartedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? EndedAtUtc { get; set; }
    
    public Point? StartPosition { get; set; }
    public Point? EndPosition { get; set; }
    
    public string Status { get; set; } = "Starting"; // Starting, Active, Paused, Completed, Cancelled, Failed
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public GpsDevice GpsDevice { get; set; } = null!;
}
