namespace AisVessel.Domain.Entities;

public class BerthAssignment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid PortCallId { get; set; }
    public Guid BerthId { get; set; }
    
    public DateTime AssignedAtUtc { get; set; } = DateTime.UtcNow;
    
    public DateTime PlannedStartAtUtc { get; set; }
    public DateTime? PlannedEndAtUtc { get; set; }
    
    public DateTime? ActualStartAtUtc { get; set; }
    public DateTime? ActualEndAtUtc { get; set; }
    
    public string Status { get; set; } = "Scheduled";
    public Guid AssignedBy { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public PortCall PortCall { get; set; } = null!;
    public Berth Berth { get; set; } = null!;
}
