using NetTopologySuite.Geometries;

namespace Corridor.Domain.Entities;

public class TripEvent
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid TripId { get; set; }
    
    public string EventType { get; set; } = string.Empty;
    /* 
       TripCreated, TripApproved, TripReady, TripStarted, 
       CheckpointReached, BorderApproached, BorderReached, 
       PortReached, DryPortReached, DestinationReached, 
       DeliveryCompleted, TripDelayed, RouteDeviation, 
       TripHeld, TripResumed, TripCompleted, TripCancelled 
    */
    
    public DateTime OccurredAtUtc { get; set; }
    public DateTime DetectedAtUtc { get; set; } = DateTime.UtcNow;
    
    public Point? Position { get; set; } // geography(Point,4326)
    
    public string Source { get; set; } = string.Empty;
    public string? ExternalReference { get; set; }
    public string? Description { get; set; }
    public string CorrelationId { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public Trip Trip { get; set; } = null!;
}
