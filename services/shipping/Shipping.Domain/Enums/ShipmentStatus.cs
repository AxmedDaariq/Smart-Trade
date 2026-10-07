namespace Shipping.Domain.Enums;

/// <summary>
/// Represents the lifecycle status of a shipment.
/// </summary>
public enum ShipmentStatus
{
    /// <summary>Shipment order has been created but not yet confirmed.</summary>
    Draft,

    /// <summary>Shipment is confirmed and awaiting carrier pickup.</summary>
    Booked,

    /// <summary>Cargo has been collected by the carrier.</summary>
    PickedUp,

    /// <summary>Cargo has been accepted at origin port/terminal.</summary>
    AtOriginTerminal,

    /// <summary>Cargo has been loaded onto the vessel/aircraft/truck.</summary>
    Loaded,

    /// <summary>Shipment is in transit.</summary>
    InTransit,

    /// <summary>Shipment has arrived at destination port/terminal.</summary>
    AtDestinationTerminal,

    /// <summary>Cargo is being unloaded.</summary>
    Discharged,

    /// <summary>Awaiting customs clearance.</summary>
    AwaitingCustomsClearance,

    /// <summary>Customs clearance has been granted.</summary>
    CustomsCleared,

    /// <summary>Cargo is out for delivery to the consignee.</summary>
    OutForDelivery,

    /// <summary>Cargo has been successfully delivered.</summary>
    Delivered,

    /// <summary>Shipment is on hold (e.g. customs hold, payment issue).</summary>
    OnHold,

    /// <summary>Shipment has been cancelled.</summary>
    Cancelled,

    /// <summary>Shipment is being returned to origin.</summary>
    Returning,

    /// <summary>Shipment has been returned to origin.</summary>
    Returned
}
