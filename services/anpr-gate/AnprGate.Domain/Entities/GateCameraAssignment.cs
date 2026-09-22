namespace AnprGate.Domain.Entities;

public class GateCameraAssignment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid GateId { get; set; }
    public Guid CameraId { get; set; }
    public Guid? LaneId { get; set; }
    
    public string AssignmentType { get; set; } = string.Empty; // Entry, Exit, Overview, Backup, Other
    
    public DateTime EffectiveFromUtc { get; set; } = DateTime.UtcNow;
    public DateTime? EffectiveToUtc { get; set; }
    
    public string Status { get; set; } = "Active"; // Active, Inactive
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public Gate Gate { get; set; } = null!;
    public AnprCamera Camera { get; set; } = null!;
    public GateLane? Lane { get; set; }
}
