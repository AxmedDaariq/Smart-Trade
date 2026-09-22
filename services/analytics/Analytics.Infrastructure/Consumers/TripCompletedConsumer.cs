using MassTransit;
using Microsoft.EntityFrameworkCore;
using SmartTrade.Analytics.Infrastructure.Data;
using SmartTrade.Analytics.Domain.Entities;
using Microsoft.Extensions.Logging;

namespace SmartTrade.Analytics.Infrastructure.Consumers;

public record TripCompletedEvent(Guid TripId, Guid TenantId, decimal ActualDistance, decimal ActualDuration, decimal DelayMinutes, DateTimeOffset CompletedAt);

public class TripCompletedConsumer : IConsumer<TripCompletedEvent>
{
    private readonly AnalyticsDbContext _dbContext;
    private readonly ILogger<TripCompletedConsumer> _logger;

    public TripCompletedConsumer(AnalyticsDbContext dbContext, ILogger<TripCompletedConsumer> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<TripCompletedEvent> context)
    {
        var msg = context.Message;
        _logger.LogInformation("Processing TripCompletedEvent for Analytics: {TripId}", msg.TripId);

        // Idempotency Check
        var exists = await _dbContext.FactTrips.AnyAsync(x => x.TripId == msg.TripId);
        if (exists)
        {
            _logger.LogWarning("Trip {TripId} already recorded in analytics fact table. Skipping duplicate.", msg.TripId);
            return;
        }

        var fact = new FactTrip
        {
            TripKey = Guid.NewGuid(),
            TripId = msg.TripId,
            TenantKey = msg.TenantId,
            DateKey = int.Parse(msg.CompletedAt.ToString("yyyyMMdd")),
            ActualDistance = msg.ActualDistance,
            ActualDuration = msg.ActualDuration,
            DelayMinutes = msg.DelayMinutes,
            Status = "COMPLETED",
            Completed = true,
            CreatedAt = msg.CompletedAt
        };

        _dbContext.FactTrips.Add(fact);
        await _dbContext.SaveChangesAsync();
    }
}
