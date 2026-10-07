namespace Shipping.Contracts.Events;

/// <summary>Published when a new shipment is created (Draft status).</summary>
public record ShipmentCreatedEvent(
    Guid ShipmentId,
    Guid TenantId,
    string ShipmentNumber,
    string ShipperReference,
    string ConsigneeReference,
    string OriginCountryCode,
    string OriginPortCode,
    string DestinationCountryCode,
    string DestinationPortCode,
    string TransportMode,
    string CargoType,
    string Incoterm,
    DateTime CreatedAtUtc
);

/// <summary>Published when a shipment status changes.</summary>
public record ShipmentStatusChangedEvent(
    Guid ShipmentId,
    Guid TenantId,
    string ShipmentNumber,
    string FromStatus,
    string ToStatus,
    string? Reason,
    DateTime ChangedAtUtc
);

/// <summary>Published when customs clearance is confirmed on a shipment.</summary>
public record ShipmentCustomsClearedEvent(
    Guid ShipmentId,
    Guid TenantId,
    string ShipmentNumber,
    Guid CustomsCaseId,
    string CustomsCaseNumber,
    DateTime ClearedAtUtc
);

/// <summary>Published when cargo is delivered to the consignee.</summary>
public record ShipmentDeliveredEvent(
    Guid ShipmentId,
    Guid TenantId,
    string ShipmentNumber,
    string ConsigneeReference,
    DateTime DeliveredAtUtc
);

/// <summary>Published when a new tracking event is recorded.</summary>
public record ShipmentTrackingEventRecordedEvent(
    Guid ShipmentId,
    Guid TenantId,
    string ShipmentNumber,
    string EventType,
    string Location,
    string? Description,
    double? Latitude,
    double? Longitude,
    DateTime OccurredAtUtc
);
