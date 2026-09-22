namespace AnprGate.Domain.Entities;

public class CameraHealthRecord
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid CameraId { get; set; }
    
    public DateTime RecordedAtUtc { get; set; } = DateTime.UtcNow;
    
    public string OperationalStatus { get; set; } = string.Empty;
    
    public int? LatencyMs { get; set; }
    public double? FrameRate { get; set; }
    public string? StorageStatus { get; set; }
    public string? NetworkStatus { get; set; }
    public int? ClockOffsetSeconds { get; set; }
    
    public string? ProviderStatus { get; set; }
    public string? ErrorCode { get; set; }
    public string? ErrorMessage { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public AnprCamera Camera { get; set; } = null!;
}
