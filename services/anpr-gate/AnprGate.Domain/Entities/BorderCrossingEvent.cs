using NetTopologySuite.Geometries;

namespace AnprGate.Domain.Entities;

public class BorderCrossingEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid BorderCrossingId { get; set; }
    
    public string EventType { get; set; } = string.Empty; // Approached, EntryRecognized, EntryAuthorized, Entered, ProcessingStarted, ProcessingCompleted, ExitRecognized, ExitAuthorized, Exited, Completed, Denied, ManualOverride
    
    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
    
    public Point? Position { get; set; } // geography(Point,4326)
    
    public string Source { get; set; } = string.Empty;
    public string? ExternalReference { get; set; }
    public string? EvidenceReference { get; set; }
    
    public string CorrelationId { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public BorderCrossing BorderCrossing { get; set; } = null!;
}
