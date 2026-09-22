namespace AnprGate.Domain.Entities;

public class CameraConfiguration
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid CameraId { get; set; }
    
    public int ConfigurationVersion { get; set; }
    
    public string PlateCountryMode { get; set; } = string.Empty;
    public double ConfidenceThreshold { get; set; }
    public string CaptureDirection { get; set; } = string.Empty;
    public int MaxPayloadSize { get; set; }
    public int ClockToleranceSeconds { get; set; }
    
    public DateTime EffectiveFromUtc { get; set; } = DateTime.UtcNow;
    public DateTime? EffectiveToUtc { get; set; }
    
    public string Status { get; set; } = "Draft"; // Draft, Approved, Published, Superseded, Archived
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public Guid CreatedBy { get; set; }
    public Guid? ApprovedBy { get; set; }

    public AnprCamera Camera { get; set; } = null!;
}
