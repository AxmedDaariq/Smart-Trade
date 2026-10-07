namespace Shipping.Domain.Entities;

/// <summary>
/// Represents a single cargo line item within a shipment (product / HS code level).
/// </summary>
public class ShipmentLine
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid ShipmentId { get; set; }
    public Shipment? Shipment { get; set; }

    public int LineNumber { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? HsCode { get; set; }
    public string? CountryOfOrigin { get; set; }
    public string? ProductCode { get; set; }
    public decimal Quantity { get; set; }
    public string UnitOfMeasure { get; set; } = "PCS";
    public decimal GrossWeightKg { get; set; }
    public decimal NetWeightKg { get; set; }
    public decimal VolumeM3 { get; set; }
    public decimal UnitValue { get; set; }
    public decimal TotalValue { get; set; }
    public string Currency { get; set; } = "USD";
    public string? Remarks { get; set; }

    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
}
