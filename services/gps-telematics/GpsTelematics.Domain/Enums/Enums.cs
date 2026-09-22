using System;

namespace GpsTelematics.Domain.Enums;

public enum DeviceStatus
{
    Provisioning,
    Active,
    Online,
    Offline,
    Suspended,
    Retired,
    Unknown
}

public enum DeviceType
{
    GPS,
    OBD,
    CANBus,
    Telematics,
    Tracker,
    IoTGateway,
    Other
}

public enum AlertSeverity
{
    Info,
    Low,
    Medium,
    High,
    Critical
}

public enum TrackingSessionStatus
{
    Created,
    Active,
    Paused,
    Completed,
    Cancelled
}

public enum GeofenceType
{
    Port,
    Terminal,
    Border,
    DryPort,
    Warehouse,
    Customer,
    RestrictedArea,
    Checkpoint,
    CorridorZone,
    DangerZone,
    Custom
}
