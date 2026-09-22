using System;

namespace AisVessel.Domain.Enums;

public enum VesselStatus
{
    Unknown,
    Expected,
    Approaching,
    Anchored,
    Berthing,
    Berthed,
    Loading,
    Discharging,
    Operational,
    Departing,
    AtSea,
    Inactive,
    Suspended
}

public enum PortCallStatus
{
    Planned,
    Expected,
    Approaching,
    ArrivedAnchorage,
    AwaitingBerth,
    BerthAllocated,
    Berthed,
    OperationsStarted,
    OperationsCompleted,
    ReadyToDepart,
    Departed,
    Cancelled
}

public enum BerthStatus
{
    Available,
    Reserved,
    Occupied,
    Maintenance,
    Closed,
    Unavailable
}

public enum PortZoneType
{
    PortBoundary,
    ApproachChannel,
    Anchorage,
    BerthingArea,
    Terminal,
    RestrictedArea,
    SafetyZone,
    PilotZone,
    Custom
}

public enum VesselType
{
    ContainerShip,
    BulkCarrier,
    Tanker,
    GeneralCargo,
    RoRo,
    Passenger,
    Fishing,
    Tug,
    Barge,
    Offshore,
    Other
}
