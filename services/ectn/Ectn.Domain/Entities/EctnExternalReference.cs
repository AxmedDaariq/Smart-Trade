namespace Ectn.Domain.Entities;

public class EctnExternalReference
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EctnApplicationId { get; set; }
    
    public string ExternalSystem { get; set; } = string.Empty;
    public string ExternalReference { get; set; } = string.Empty;
    public string ExternalStatus { get; set; } = string.Empty;
    
    public DateTime LastSynchronizedAtUtc { get; set; } = DateTime.UtcNow;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public EctnApplication EctnApplication { get; set; } = null!;
}
