namespace Fleet.Domain.Entities;

public class VehicleMaintenanceRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid VehicleId { get; set; }
    
    public string MaintenanceType { get; set; } = string.Empty; // Preventive, Corrective, Emergency, Inspection, Other
    public string ReferenceNumber { get; set; } = string.Empty;
    
    public DateTime ScheduledAtUtc { get; set; }
    public DateTime? CompletedAtUtc { get; set; }
    
    public decimal? OdometerReading { get; set; }
    
    public string Description { get; set; } = string.Empty;
    public string Result { get; set; } = string.Empty;
    
    public string Status { get; set; } = "Scheduled"; // Scheduled, InProgress, Completed, Cancelled
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public Vehicle Vehicle { get; set; } = null!;
}
