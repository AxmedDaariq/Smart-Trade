namespace Fleet.Domain.Entities;

public class DriverAssignment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid FleetCompanyId { get; set; }
    
    public Guid DriverId { get; set; }
    
    public string AssignmentType { get; set; } = string.Empty; // Vehicle, Dispatch, Trip, Fleet, Other
    
    public Guid? VehicleId { get; set; }
    public Guid? DispatchOrderId { get; set; }
    
    public DateTime StartAtUtc { get; set; }
    public DateTime? EndAtUtc { get; set; }
    
    public string Status { get; set; } = "Scheduled"; // Scheduled, Active, Completed, Cancelled, Suspended
    
    public Guid AssignedBy { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public Driver Driver { get; set; } = null!;
    public DispatchOrder? DispatchOrder { get; set; }
}
