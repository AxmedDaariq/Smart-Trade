namespace GpsTelematics.Domain.Entities;

public class DeviceCommandResult
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public Guid CommandId { get; set; }
    public Guid GpsDeviceId { get; set; }
    
    public string Status { get; set; } = "Pending"; // Pending, Sent, Acknowledged, Succeeded, Failed, Expired, Cancelled
    
    public DateTime ReceivedAtUtc { get; set; } = DateTime.UtcNow;
    
    public string? ResultCode { get; set; }
    public string? ResultMessage { get; set; }
    public string ResultPayloadJson { get; set; } = "{}"; // JSONB
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public DeviceCommand Command { get; set; } = null!;
}
