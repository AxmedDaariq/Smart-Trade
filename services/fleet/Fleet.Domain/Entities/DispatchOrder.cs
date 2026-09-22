namespace Fleet.Domain.Entities;

public class DispatchOrder
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid FleetCompanyId { get; set; }
    
    public string DispatchReference { get; set; } = string.Empty;
    
    public Guid VehicleId { get; set; }
    public Guid? TrailerId { get; set; }
    public Guid DriverId { get; set; }
    
    public string OriginReference { get; set; } = string.Empty;
    public string DestinationReference { get; set; } = string.Empty;
    
    public string? CargoReference { get; set; }
    public string? DeclarationReference { get; set; }
    public string? EctnReference { get; set; }
    public string? CustomsReleaseReference { get; set; }
    
    public DateTime PlannedDepartureAtUtc { get; set; }
    public DateTime PlannedArrivalAtUtc { get; set; }
    
    public DateTime? ActualDepartureAtUtc { get; set; }
    public DateTime? ActualArrivalAtUtc { get; set; }
    
    public string Status { get; set; } = "Draft"; // Draft, PendingApproval, Approved, Ready, Dispatched, InTransit, Completed, Cancelled, Failed, Held
    public string Priority { get; set; } = "Normal";
    public string Instructions { get; set; } = string.Empty;
    
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public ICollection<DispatchRequirement> Requirements { get; set; } = new List<DispatchRequirement>();
    public ICollection<VehicleAssignment> VehicleAssignments { get; set; } = new List<VehicleAssignment>();
    public ICollection<DriverAssignment> DriverAssignments { get; set; } = new List<DriverAssignment>();
    public ICollection<TrailerAssignment> TrailerAssignments { get; set; } = new List<TrailerAssignment>();
    public ICollection<DispatchStatusHistory> StatusHistories { get; set; } = new List<DispatchStatusHistory>();
}
