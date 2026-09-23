using Ectn.Domain.Enums;

namespace Ectn.Domain.Entities;

public class EctnStatusHistory
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EctnApplicationId { get; set; }

    // Typed status used by Ectn aggregate
    public EctnStatus Status { get; set; }

    public string FromStatus { get; set; } = string.Empty;
    public string ToStatus { get; set; } = string.Empty;

    public Guid ChangedBy { get; set; }

    // Alias used by aggregate root (maps to ChangedAtUtc for persistence)
    public DateTime ChangedAt
    {
        get => ChangedAtUtc;
        set => ChangedAtUtc = value;
    }

    public DateTime ChangedAtUtc { get; set; } = DateTime.UtcNow;

    public string Reason { get; set; } = string.Empty;
    public string CorrelationId { get; set; } = string.Empty;

    public EctnApplication? EctnApplication { get; set; }
}
