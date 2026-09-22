namespace AnprGate.Domain.Entities;

public class CameraCommand
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid CameraId { get; set; }
    
    public string CommandType { get; set; } = string.Empty; // HealthCheck, ConfigurationSync, Restart, CaptureTest, TimeSync, Other
    
    public Guid RequestedBy { get; set; }
    public DateTime RequestedAtUtc { get; set; } = DateTime.UtcNow;
    
    public string CorrelationId { get; set; } = string.Empty;
    
    public string Status { get; set; } = "Pending"; // Pending, Sent, Succeeded, Failed, Expired
    
    public DateTime? CompletedAtUtc { get; set; }
    public string? ResultCode { get; set; }
    public string? ResultMessage { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public AnprCamera Camera { get; set; } = null!;
}
