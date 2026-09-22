namespace AisVessel.Domain.Entities;

public class AisDeviceAssignment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid AisDeviceId { get; set; }
    public Guid VesselId { get; set; }
    
    public DateTime StartAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? EndAtUtc { get; set; }
    
    public string Status { get; set; } = "Active";
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public AisDevice AisDevice { get; set; } = null!;
    public Vessel Vessel { get; set; } = null!;
}
