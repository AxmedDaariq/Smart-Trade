namespace Corridor.Domain.Entities;

public class TripStatusHistory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    
    public Guid TripId { get; set; }
    
    public string FromStatus { get; set; } = string.Empty;
    public string ToStatus { get; set; } = string.Empty;
    
    public DateTime ChangedAtUtc { get; set; } = DateTime.UtcNow;
    public Guid? ChangedBy { get; set; }
    
    public string? Reason { get; set; }
    public string Source { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

    public Trip Trip { get; set; } = null!;
}
