namespace Ectn.Domain.Entities;

public class EctnStatusHistory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EctnApplicationId { get; set; }
    
    public string FromStatus { get; set; } = string.Empty;
    public string ToStatus { get; set; } = string.Empty;
    
    public Guid ChangedBy { get; set; }
    public DateTime ChangedAtUtc { get; set; } = DateTime.UtcNow;
    
    public string Reason { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;
    
    public EctnApplication EctnApplication { get; set; } = null!;
}
