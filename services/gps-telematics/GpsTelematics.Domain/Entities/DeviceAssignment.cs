namespace GpsTelematics.Domain.Entities;

public class DeviceAssignment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid GpsDeviceId { get; set; }
    
    public string VehicleReference { get; set; } = string.Empty;
    public string? DriverReference { get; set; }
    public string FleetCompanyReference { get; set; } = string.Empty;
    
    public DateTime StartAtUtc { get; set; }
    public DateTime? EndAtUtc { get; set; }
    
    public string Status { get; set; } = "Active";
    
    public Guid AssignedBy { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();

    public GpsDevice GpsDevice { get; set; } = null!;
}
