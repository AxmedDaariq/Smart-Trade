namespace GpsTelematics.Domain.Entities;

public class GeofenceRule
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid GeofenceId { get; set; }
    
    public string RuleType { get; set; } = string.Empty; // Entry, Exit, Dwell, Speed, Restricted, RouteDeviation, Other
    public string EventType { get; set; } = string.Empty;
    
    public bool Enabled { get; set; } = true;
    
    public double? SpeedThresholdKph { get; set; }
    public int? DwellThresholdSeconds { get; set; }
    public string? Direction { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public Geofence Geofence { get; set; } = null!;
}
