using System;

namespace Fleet.Domain.Enums;

public enum VehicleStatus
{
    Draft,
    PendingVerification,
    Active,
    Available,
    Assigned,
    Dispatched,
    OnTrip,
    AtBorder,
    AtDryPort,
    Maintenance,
    Suspended,
    Held,
    Inactive,
    Retired
}

public enum DriverStatus
{
    PendingVerification,
    Active,
    Available,
    Assigned,
    OnTrip,
    Suspended,
    Unavailable,
    Expired,
    Inactive
}

public enum DispatchStatus
{
    Draft,
    PendingValidation,
    Ready,
    Approved,
    Dispatched,
    Cancelled,
    Completed,
    Blocked
}

public enum ComplianceStatus
{
    Compliant,
    Warning,
    NonCompliant,
    Expired,
    Blocked
}
