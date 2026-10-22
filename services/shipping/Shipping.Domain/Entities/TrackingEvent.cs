using Shipping.Domain.Enums;

namespace Shipping.Domain.Entities;

/// <summary>
/// A single milestone/scan event recorded against a shipment for real-time tracking.
/// </summary>
public class TrackingEvent
{
    public Guid Id { get; set; }
    public Guid ShipmentId { get; set; }
    public Shipment? Shipment { get; set; }

    public TrackingEventType EventType { get; set; }

    /// <summary>Free-text location or UN/LOCODE where the event occurred.</summary>
    public string Location { get; set; } = string.Empty;

    /// <summary>Human-readable event description or carrier message.</summary>
    public string? Description { get; set; }

    /// <summary>GPS latitude at time of event (optional).</summary>
    public double? Latitude { get; set; }

    /// <summary>GPS longitude at time of event (optional).</summary>
    public double? Longitude { get; set; }

    /// <summary>Vessel or flight or vehicle identifier.</summary>
    public string? VehicleIdentifier { get; set; }

    /// <summary>Carrier-provided event code for reconciliation.</summary>
    public string? CarrierEventCode { get; set; }

    public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
    public Guid RecordedBy { get; set; }
}
