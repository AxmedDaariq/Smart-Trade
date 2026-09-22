using NetTopologySuite.Geometries;

namespace AisVessel.Domain.Entities;

public class VesselTrackingSession
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid VesselId { get; set; }
    public Guid? AisDeviceId { get; set; }
    public string? ProviderReference { get; set; }
    
    public DateTime StartedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? EndedAtUtc { get; set; }
    
    public Point? StartPosition { get; set; } // geography(Point,4326)
    public Point? EndPosition { get; set; } // geography(Point,4326)
    
    public string Status { get; set; } = "Starting"; // Starting, Active, Paused, Completed, Cancelled, Failed
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public Vessel Vessel { get; set; } = null!;
    public AisDevice? AisDevice { get; set; }
}
