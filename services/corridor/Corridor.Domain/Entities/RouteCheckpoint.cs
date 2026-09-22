namespace Corridor.Domain.Entities;

public class RouteCheckpoint
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid RouteVersionId { get; set; }
    public Guid CheckpointId { get; set; }
    
    public int SequenceNumber { get; set; }
    public bool Mandatory { get; set; } = true;
    
    public double? ExpectedArrivalOffsetMinutes { get; set; }
    public double? ExpectedDwellMinutes { get; set; }
    public double? AllowedDeviationMeters { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public RouteVersion RouteVersion { get; set; } = null!;
    public Checkpoint Checkpoint { get; set; } = null!;
}
