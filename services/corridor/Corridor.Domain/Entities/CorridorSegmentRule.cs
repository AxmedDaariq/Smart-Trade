namespace Corridor.Domain.Entities;

public class CorridorSegmentRule
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid CorridorSegmentId { get; set; }
    
    public string RuleType { get; set; } = string.Empty; // MaximumSpeed, MinimumSpeed, ExpectedDuration, MandatoryCheckpoint, RestrictedTime, AllowedVehicleType, Other
    public string RuleValue { get; set; } = string.Empty;
    public string? Unit { get; set; }
    
    public DateTime EffectiveFromUtc { get; set; } = DateTime.UtcNow;
    public DateTime? EffectiveToUtc { get; set; }
    
    public string Status { get; set; } = "Active";
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;

    public CorridorSegment CorridorSegment { get; set; } = null!;
}
