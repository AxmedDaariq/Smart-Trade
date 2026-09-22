using System;

namespace Analytics.Domain.Entities;

public class FactTripMilestone
{
    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid TripId { get; private set; }
    
    // Dimension References
    public Guid VehicleId { get; private set; }
    public Guid CorridorId { get; private set; }
    
    // Core details
    public string MilestoneType { get; private set; } // e.g., "PortExit", "BorderCrossing", "DryPortArrival"
    public string LocationName { get; private set; }
    
    // Analytics Metrics
    public DateTime OccurredAt { get; private set; }
    public TimeSpan? ExpectedDurationFromStart { get; private set; }
    public TimeSpan? ActualDurationFromStart { get; private set; }
    public TimeSpan? DelayDuration { get; private set; }
    
    // Data Lineage
    public Guid SourceEventId { get; private set; }
    public DateTime ProcessedAt { get; private set; }

    protected FactTripMilestone() { }

    public FactTripMilestone(
        Guid tenantId,
        Guid tripId,
        Guid vehicleId,
        Guid corridorId,
        string milestoneType,
        string locationName,
        DateTime occurredAt,
        TimeSpan? expectedDuration,
        TimeSpan? actualDuration,
        TimeSpan? delayDuration,
        Guid sourceEventId)
    {
        Id = Guid.NewGuid();
        TenantId = tenantId;
        TripId = tripId;
        VehicleId = vehicleId;
        CorridorId = corridorId;
        MilestoneType = milestoneType;
        LocationName = locationName;
        OccurredAt = occurredAt;
        ExpectedDurationFromStart = expectedDuration;
        ActualDurationFromStart = actualDuration;
        DelayDuration = delayDuration;
        SourceEventId = sourceEventId;
        ProcessedAt = DateTime.UtcNow;
    }
}
