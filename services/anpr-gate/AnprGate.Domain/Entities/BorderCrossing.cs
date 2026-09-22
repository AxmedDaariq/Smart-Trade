namespace AnprGate.Domain.Entities;

public class BorderCrossing
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public string CrossingReference { get; set; } = string.Empty;
    
    public Guid BorderCheckpointId { get; set; }
    public Guid? GateId { get; set; }
    
    public string VehicleReference { get; set; } = string.Empty; // External to Fleet
    public string? DriverReference { get; set; } // External to Fleet
    public string? TripReference { get; set; } // External to Corridor
    
    public string? EntryRecognitionReference { get; set; }
    public string? ExitRecognitionReference { get; set; }
    
    public DateTime? EntryAtUtc { get; set; }
    public DateTime? ExitAtUtc { get; set; }
    
    public string Status { get; set; } = "Approaching"; // Approaching, EntryPending, Entered, Processing, ExitPending, Exited, Completed, Denied, Cancelled, ManualReview
    public string Direction { get; set; } = string.Empty; // Inbound, Outbound
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public BorderCheckpoint BorderCheckpoint { get; set; } = null!;
    public Gate? Gate { get; set; }
    
    public ICollection<BorderCrossingEvent> Events { get; set; } = new List<BorderCrossingEvent>();
}
