using NetTopologySuite.Geometries;

namespace Corridor.Domain.Entities;

public class CorridorSegment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid CorridorVersionId { get; set; }
    
    public string SegmentReference { get; set; } = string.Empty;
    public int SequenceNumber { get; set; }
    public string Name { get; set; } = string.Empty;
    
    public string OriginPoint { get; set; } = string.Empty;
    public string DestinationPoint { get; set; } = string.Empty;
    
    public LineString Geometry { get; set; } = null!; // LINESTRING 4326
    
    public double? DistanceKm { get; set; }
    public double? ExpectedDurationMinutes { get; set; }
    
    public string Status { get; set; } = "Active";
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public CorridorVersion CorridorVersion { get; set; } = null!;
    public ICollection<CorridorSegmentRule> Rules { get; set; } = new List<CorridorSegmentRule>();
}
