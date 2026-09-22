namespace Corridor.Domain.Entities;

public class TripDelay
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid TripId { get; set; }
    public Guid? MilestoneId { get; set; }
    
    public string DelayType { get; set; } = string.Empty; // Departure, Checkpoint, Border, Port, DryPort, Route, Vehicle, Driver, Weather, Congestion, Documentation, Operational, Other
    
    public DateTime StartedAtUtc { get; set; }
    public DateTime? EndedAtUtc { get; set; }
    
    public double? DurationMinutes { get; set; }
    
    public string Reason { get; set; } = string.Empty;
    public string Source { get; set; } = string.Empty;
    
    public string Status { get; set; } = "Open"; // Open, Resolved, Cancelled
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public Trip Trip { get; set; } = null!;
}
