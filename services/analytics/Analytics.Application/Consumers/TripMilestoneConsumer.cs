using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Analytics.Domain.Entities;
using Analytics.Infrastructure.Persistence;

namespace Analytics.Application.Consumers;

public class TripMilestoneEvent
{
    public Guid EventId { get; set; }
    public Guid TenantId { get; set; }
    public Guid TripId { get; set; }
    public Guid VehicleId { get; set; }
    public Guid CorridorId { get; set; }
    public string MilestoneType { get; set; }
    public string LocationName { get; set; }
    public DateTime OccurredAt { get; set; }
    public TimeSpan? ExpectedDuration { get; set; }
    public TimeSpan? ActualDuration { get; set; }
    public TimeSpan? DelayDuration { get; set; }
}

public class TripMilestoneConsumer : IConsumer<TripMilestoneEvent>
{
    private readonly AnalyticsDbContext _context;

    public TripMilestoneConsumer(AnalyticsDbContext context)
    {
        _context = context;
    }

    public async Task Consume(ConsumeContext<TripMilestoneEvent> context)
    {
        var msg = context.Message;
        
        // Idempotency Check: Don't ingest the same event twice
        var exists = await _context.FactTripMilestones.AnyAsync(f => f.SourceEventId == msg.EventId);
        if (exists) return;

        // Data Quality Rule: Log and skip negative durations
        if (msg.ActualDuration.HasValue && msg.ActualDuration.Value.TotalSeconds < 0)
        {
            // Ideally, we'd log this into a DataQualityIssue table
            return;
        }

        var fact = new FactTripMilestone(
            msg.TenantId,
            msg.TripId,
            msg.VehicleId,
            msg.CorridorId,
            msg.MilestoneType,
            msg.LocationName,
            msg.OccurredAt,
            msg.ExpectedDuration,
            msg.ActualDuration,
            msg.DelayDuration,
            msg.EventId
        );

        _context.FactTripMilestones.Add(fact);
        await _context.SaveChangesAsync();
    }
}
