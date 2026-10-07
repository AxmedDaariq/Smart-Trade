namespace Shipping.Domain.Entities;

/// <summary>
/// A physical container assigned to a shipment (ISO container or unit load device).
/// </summary>
public class ShipmentContainer
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ShipmentId { get; set; }
    public Shipment? Shipment { get; set; }

    /// <summary>ISO container number e.g. MSCU1234567.</summary>
    public string ContainerNumber { get; set; } = string.Empty;

    /// <summary>ISO size/type code e.g. 22G1, 45R1.</summary>
    public string ContainerType { get; set; } = string.Empty;

    /// <summary>Tare weight of the empty container in kg.</summary>
    public decimal TareWeightKg { get; set; }

    /// <summary>Gross cargo weight loaded into container in kg.</summary>
    public decimal CargoWeightKg { get; set; }

    /// <summary>Seal number applied at stuffing.</summary>
    public string? SealNumber { get; set; }

    /// <summary>Verified Gross Mass weight (SOLAS requirement).</summary>
    public decimal? VerifiedGrossMassKg { get; set; }

    public bool IsHazardous { get; set; }
    public string? ImdgClass { get; set; }
    public string? UnNumber { get; set; }

    public bool IsReeferActive { get; set; }
    public decimal? SetTemperatureCelsius { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
}
