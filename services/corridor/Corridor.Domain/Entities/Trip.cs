using NetTopologySuite.Geometries;

namespace Corridor.Domain.Entities;

public class Trip
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public string TripReference { get; set; } = string.Empty;
    public string DispatchReference { get; set; } = string.Empty; // External to Fleet
    public string VehicleReference { get; set; } = string.Empty; // External to Fleet
    public string? TrailerReference { get; set; } // External to Fleet
    public string DriverReference { get; set; } = string.Empty; // External to Fleet
    
    public Guid? CorridorId { get; set; }
    public Guid? CorridorVersionId { get; set; }
    
    public Guid? RouteId { get; set; }
    public Guid? RouteVersionId { get; set; }
    
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    
    public DateTime PlannedStartAtUtc { get; set; }
    public DateTime? PlannedArrivalAtUtc { get; set; }
    
    public DateTime? ActualStartAtUtc { get; set; }
    public DateTime? ActualArrivalAtUtc { get; set; }
    
    public string Status { get; set; } = "Planned"; // Planned, Approved, Ready, Dispatched, Started, InTransit, AtCheckpoint, Delayed, Held, Completed, Cancelled, Failed
    public string? CurrentMilestone { get; set; }
    
    public double? CurrentLatitude { get; set; }
    public double? CurrentLongitude { get; set; }
    public Point? CurrentPosition { get; set; } // geography(Point,4326)
    
    public DateTime? LastPositionAtUtc { get; set; }
    public DateTime? CurrentEtaAtUtc { get; set; }
    
    public double? DelayMinutes { get; set; }
    public string DeviationStatus { get; set; } = "Normal"; // Normal, Warning, Deviated, Unknown
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public ICollection<TripVersion> Versions { get; set; } = new List<TripVersion>();
    public ICollection<TripMilestone> Milestones { get; set; } = new List<TripMilestone>();
    public ICollection<TripCheckpointEvent> CheckpointEvents { get; set; } = new List<TripCheckpointEvent>();
    public ICollection<TripEvent> TripEvents { get; set; } = new List<TripEvent>();
    public ICollection<TripEtaRecord> EtaRecords { get; set; } = new List<TripEtaRecord>();
    public ICollection<TripDelay> Delays { get; set; } = new List<TripDelay>();
    public ICollection<RouteDeviation> Deviations { get; set; } = new List<RouteDeviation>();
    public ICollection<TripIncident> Incidents { get; set; } = new List<TripIncident>();
    public ICollection<CorridorAlert> Alerts { get; set; } = new List<CorridorAlert>();
    public ICollection<TripHold> Holds { get; set; } = new List<TripHold>();
    public ICollection<TripStatusHistory> StatusHistory { get; set; } = new List<TripStatusHistory>();
}
