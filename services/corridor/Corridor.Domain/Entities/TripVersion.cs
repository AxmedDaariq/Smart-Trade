namespace Corridor.Domain.Entities;

public class TripVersion
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid TripId { get; set; }
    
    public int VersionNumber { get; set; }
    
    public string DispatchReference { get; set; } = string.Empty;
    public string VehicleReference { get; set; } = string.Empty;
    public string DriverReference { get; set; } = string.Empty;
    
    public Guid? CorridorVersionId { get; set; }
    public Guid? RouteVersionId { get; set; }
    
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    
    public DateTime PlannedStartAtUtc { get; set; }
    public DateTime? PlannedArrivalAtUtc { get; set; }
    
    public string ChangeReason { get; set; } = string.Empty;
    public Guid ChangedBy { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public Trip Trip { get; set; } = null!;
}
