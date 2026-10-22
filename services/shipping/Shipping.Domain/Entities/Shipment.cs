using Shipping.Domain.Enums;

namespace Shipping.Domain.Entities;

/// <summary>
/// Root aggregate for a shipment — tracks a cargo movement from origin to destination.
/// </summary>
public class Shipment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid? OrganizationId { get; set; }

    // ── Reference numbers ─────────────────────────────────────────────────────
    /// <summary>Platform-generated unique shipment reference (e.g. SHP-2024-000001).</summary>
    public string ShipmentNumber { get; set; } = string.Empty;

    /// <summary>External reference from shipper/ERP system.</summary>
    public string? ExternalReference { get; set; }

    /// <summary>Bill of Lading / Airway Bill number issued by the carrier.</summary>
    public string? CarrierBookingReference { get; set; }

    /// <summary>Master Bill of Lading number.</summary>
    public string? MasterBillNumber { get; set; }

    /// <summary>House Bill of Lading number (for consolidations).</summary>
    public string? HouseBillNumber { get; set; }

    // ── Parties ───────────────────────────────────────────────────────────────
    public string ShipperName { get; set; } = string.Empty;
    public string ShipperReference { get; set; } = string.Empty;
    public string ConsigneeName { get; set; } = string.Empty;
    public string ConsigneeReference { get; set; } = string.Empty;
    public string? NotifyPartyName { get; set; }
    public string? CarrierCode { get; set; }
    public string? CarrierName { get; set; }
    public string? FreightForwarderCode { get; set; }

    // ── Route ─────────────────────────────────────────────────────────────────
    public string OriginCountryCode { get; set; } = string.Empty;
    public string OriginPortCode { get; set; } = string.Empty;
    public string OriginPortName { get; set; } = string.Empty;
    public string DestinationCountryCode { get; set; } = string.Empty;
    public string DestinationPortCode { get; set; } = string.Empty;
    public string DestinationPortName { get; set; } = string.Empty;
    public string? TransshipmentPortCode { get; set; }

    // ── Cargo & Mode ──────────────────────────────────────────────────────────
    public TransportMode TransportMode { get; set; }
    public CargoType CargoType { get; set; }
    public Incoterm Incoterm { get; set; }

    public decimal GrossWeightKg { get; set; }
    public decimal NetWeightKg { get; set; }
    public decimal VolumeM3 { get; set; }
    public int PackageCount { get; set; }
    public string? PackagingType { get; set; }
    public string? HsCode { get; set; }
    public string? GoodsDescription { get; set; }

    // ── Dates ─────────────────────────────────────────────────────────────────
    public DateTime? EstimatedDepartureUtc { get; set; }
    public DateTime? EstimatedArrivalUtc { get; set; }
    public DateTime? ActualDepartureUtc { get; set; }
    public DateTime? ActualArrivalUtc { get; set; }
    public DateTime? DeliveredAtUtc { get; set; }

    // ── Status ────────────────────────────────────────────────────────────────
    public ShipmentStatus Status { get; set; } = ShipmentStatus.Draft;
    public string? StatusReason { get; set; }

    // ── Financial ─────────────────────────────────────────────────────────────
    public string Currency { get; set; } = "USD";
    public decimal? FreightValue { get; set; }
    public decimal? InsuranceValue { get; set; }
    public decimal? DeclaredValue { get; set; }

    // ── Customs link ──────────────────────────────────────────────────────────
    public Guid? CustomsCaseId { get; set; }
    public string? CustomsCaseNumber { get; set; }
    public bool IsCustomsCleared { get; set; }
    public DateTime? CustomsClearedAtUtc { get; set; }

    // ── Audit ─────────────────────────────────────────────────────────────────
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    public Guid CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    public byte[] Version { get; set; } = Array.Empty<byte>();

    // ── Navigation ────────────────────────────────────────────────────────────
    public ICollection<ShipmentLine> Lines { get; set; } = new List<ShipmentLine>();
    public ICollection<ShipmentContainer> Containers { get; set; } = new List<ShipmentContainer>();
    public ICollection<TrackingEvent> TrackingEvents { get; set; } = new List<TrackingEvent>();
    public ICollection<ShipmentDocument> Documents { get; set; } = new List<ShipmentDocument>();
    public ICollection<ShipmentStatusHistory> StatusHistories { get; set; } = new List<ShipmentStatusHistory>();

    public Shipment() { }

    public Shipment(
        Guid tenantId,
        string shipmentNumber,
        string shipperName,
        string shipperReference,
        string consigneeName,
        string consigneeReference,
        string originCountryCode,
        string originPortCode,
        string originPortName,
        string destinationCountryCode,
        string destinationPortCode,
        string destinationPortName,
        TransportMode transportMode,
        CargoType cargoType,
        Incoterm incoterm,
        string currency,
        Guid createdBy)
    {
        TenantId = tenantId;
        ShipmentNumber = shipmentNumber;
        ShipperName = shipperName ?? string.Empty;
        ShipperReference = shipperReference ?? string.Empty;
        ConsigneeName = consigneeName ?? string.Empty;
        ConsigneeReference = consigneeReference ?? string.Empty;
        OriginCountryCode = originCountryCode ?? string.Empty;
        OriginPortCode = originPortCode ?? string.Empty;
        OriginPortName = originPortName ?? string.Empty;
        DestinationCountryCode = destinationCountryCode ?? string.Empty;
        DestinationPortCode = destinationPortCode ?? string.Empty;
        DestinationPortName = destinationPortName ?? string.Empty;
        TransportMode = transportMode;
        CargoType = cargoType;
        Incoterm = incoterm;
        Currency = currency ?? "USD";
        CreatedBy = createdBy;
        Status = ShipmentStatus.Draft;
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    // ── Domain methods ────────────────────────────────────────────────────────

    /// <summary>Transitions the shipment to a new status and records history.</summary>
    public void TransitionStatus(ShipmentStatus newStatus, string? reason, Guid changedBy)
    {
        StatusHistories.Add(new ShipmentStatusHistory
        {
            ShipmentId = Id,
            FromStatus = Status.ToString(),
            ToStatus = newStatus.ToString(),
            Reason = reason,
            ChangedAtUtc = DateTime.UtcNow,
            ChangedBy = changedBy
        });

        Status = newStatus;
        StatusReason = reason;
        UpdatedAtUtc = DateTime.UtcNow;
        UpdatedBy = changedBy;
    }

    /// <summary>Records a tracking event (GPS ping, milestone scan, etc.).</summary>
    public void AddTrackingEvent(TrackingEventType eventType, string location, string? description, Guid recordedBy)
    {
        TrackingEvents.Add(new TrackingEvent
        {
            ShipmentId = Id,
            EventType = eventType,
            Location = location,
            Description = description,
            OccurredAtUtc = DateTime.UtcNow,
            RecordedBy = recordedBy
        });
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
