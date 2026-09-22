using System;

namespace AnprGate.Domain.Enums;

public enum CameraStatus
{
    Provisioning,
    Active,
    Offline,
    Degraded,
    Maintenance,
    Suspended,
    Retired
}

public enum CameraType
{
    ANPR,
    Overview,
    Lane,
    Entry,
    Exit,
    Border,
    PlateCapture,
    Other
}

public enum GateStatus
{
    Active,
    Closed,
    Maintenance,
    EmergencyClosed,
    Inactive
}

public enum GateType
{
    PortEntry,
    PortExit,
    TerminalEntry,
    TerminalExit,
    BorderEntry,
    BorderExit,
    DryPortEntry,
    DryPortExit,
    Checkpoint,
    Warehouse,
    Other
}

public enum RecognitionStatus
{
    Detected,
    Validated,
    Matched,
    Unmatched,
    Rejected,
    ManualReview,
    Expired
}

public enum AuthorizationStatus
{
    Requested,
    Evaluating,
    Authorized,
    Denied,
    ManualReview,
    Expired,
    Cancelled
}

public enum PassageStatus
{
    Detected,
    PendingAuthorization,
    Authorized,
    Denied,
    ManualReview,
    Passed,
    Cancelled,
    Exception
}

public enum BorderCrossingStatus
{
    Approaching,
    EntryDetected,
    Processing,
    ExitDetected,
    Completed,
    Denied,
    Cancelled,
    Exception
}

public enum Direction
{
    Inbound,
    Outbound,
    Bidirectional,
    Unknown
}
