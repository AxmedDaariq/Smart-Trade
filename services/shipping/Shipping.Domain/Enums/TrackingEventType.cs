namespace Shipping.Domain.Enums;

/// <summary>
/// Status of a tracking event recorded against a shipment leg.
/// </summary>
public enum TrackingEventType
{
    /// <summary>Shipment order created in the system.</summary>
    OrderCreated,

    /// <summary>Booking confirmed with carrier.</summary>
    BookingConfirmed,

    /// <summary>Documents (B/L, AWB) issued by carrier.</summary>
    DocumentsIssued,

    /// <summary>Cargo picked up from shipper premises.</summary>
    CargoPickedUp,

    /// <summary>Cargo delivered to origin terminal / port.</summary>
    ArrivedAtOriginTerminal,

    /// <summary>Cargo loaded onto vessel / aircraft / truck.</summary>
    DepartedOrigin,

    /// <summary>Transshipment — cargo transferred at intermediate port.</summary>
    Transshipped,

    /// <summary>Cargo arrived at destination terminal / port.</summary>
    ArrivedAtDestination,

    /// <summary>Cargo unloaded (discharged) from vessel.</summary>
    Discharged,

    /// <summary>Customs inspection initiated.</summary>
    CustomsInspectionStarted,

    /// <summary>Customs clearance granted.</summary>
    CustomsCleared,

    /// <summary>Cargo released from terminal and in transit to consignee.</summary>
    OutForDelivery,

    /// <summary>Cargo successfully delivered to consignee.</summary>
    Delivered,

    /// <summary>Delivery attempt failed.</summary>
    DeliveryAttemptFailed,

    /// <summary>Shipment placed on hold.</summary>
    HoldPlaced,

    /// <summary>Hold released and shipment resumed.</summary>
    HoldReleased,

    /// <summary>Shipment exception — damage, loss, delay, etc.</summary>
    Exception,

    /// <summary>Return initiated.</summary>
    ReturnInitiated,

    /// <summary>Returned to origin.</summary>
    ReturnedToOrigin
}
