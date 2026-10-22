namespace Shipping.Domain.Entities;

/// <summary>
/// Audit trail of every status change on a shipment.
/// </summary>
public class ShipmentStatusHistory
{
    public Guid Id { get; set; }
    public Guid ShipmentId { get; set; }
    public Shipment? Shipment { get; set; }

    public string FromStatus { get; set; } = string.Empty;
    public string ToStatus { get; set; } = string.Empty;
    public string? Reason { get; set; }

    public DateTime ChangedAtUtc { get; set; } = DateTime.UtcNow;
    public Guid ChangedBy { get; set; }
}
