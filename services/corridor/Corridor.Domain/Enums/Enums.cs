using System;

namespace Corridor.Domain.Enums;

public enum TripStatus
{
    Draft,
    Planned,
    AwaitingAssignment,
    Assigned,
    ReadyForDispatch,
    Dispatched,
    InTransit,
    AtCheckpoint,
    AtBorder,
    BorderCrossing,
    AtDryPort,
    Delivered,
    Completed,
    Delayed,
    OnHold,
    Cancelled,
    Exception
}

public enum CorridorStatus
{
    Draft,
    Active,
    Suspended,
    Inactive,
    Archived
}

public enum RouteStatus
{
    Draft,
    Published,
    Suspended,
    Retired
}

public enum RouteVersionStatus
{
    Draft,
    Active,
    Expired,
    Retired
}

public enum TripExceptionType
{
    RouteDeviation,
    VehicleBreakdown,
    DriverIssue,
    BorderDelay,
    CustomsHold,
    PaymentHold,
    DocumentationIssue,
    PortDelay,
    DryPortDelay,
    Accident,
    UnexpectedStop,
    GPSOffline,
    ANPRMismatch,
    CargoIssue,
    SecurityIssue,
    Other
}

public enum ExceptionStatus
{
    Open,
    Acknowledged,
    Investigating,
    Resolved,
    Closed
}

public enum CheckpointType
{
    Port,
    Terminal,
    Weighbridge,
    InspectionPoint,
    Checkpoint,
    Border,
    DryPort,
    Warehouse,
    Customer,
    RestArea,
    FuelStop,
    Custom
}

public enum MilestoneType
{
    TripCreated,
    CargoReady,
    TruckAssigned,
    ReleaseConfirmed,
    PortDeparture,
    CheckpointReached,
    BorderApproach,
    BorderEntry,
    BorderExit,
    DryPortArrival,
    DryPortDeparture,
    DestinationArrival,
    DeliveryCompleted
}

public enum Severity
{
    Low,
    Medium,
    High,
    Critical
}
