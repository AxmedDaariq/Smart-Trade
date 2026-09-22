namespace GpsTelematics.Domain.Entities;

public class DeviceConnection
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid GpsDeviceId { get; set; }
    
    public string ConnectionType { get; set; } = string.Empty;
    
    public DateTime ConnectedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? DisconnectedAtUtc { get; set; }
    
    public string? SourceIpHash { get; set; }
    public string Protocol { get; set; } = string.Empty;
    
    public string Status { get; set; } = "Connected"; // Connected, Disconnected, Unknown, Rejected
    
    public string? DisconnectReason { get; set; }
    public string CorrelationId { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public GpsDevice GpsDevice { get; set; } = null!;
}
