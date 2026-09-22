using System;

namespace BuildingBlocks.Infrastructure.Messaging;

public enum OutboxMessageStatus
{
    Pending,
    Processing,
    Published,
    Failed,
    DeadLetter
}

public class OutboxMessage
{
    public Guid Id { get; private set; }
    public string EventType { get; private set; } = string.Empty;
    public int EventVersion { get; private set; }
    public string AggregateId { get; private set; } = string.Empty;
    public string Payload { get; private set; } = string.Empty;
    public DateTime OccurredAtUtc { get; private set; }
    public DateTime CreatedAtUtc { get; private set; }
    public DateTime? PublishedAtUtc { get; private set; }
    public int RetryCount { get; private set; }
    public OutboxMessageStatus Status { get; private set; }
    public string? LastError { get; private set; }
    public Guid CorrelationId { get; private set; }
    public Guid TenantId { get; private set; }

    protected OutboxMessage() { } // EF Core

    public OutboxMessage(Guid id, string eventType, int eventVersion, string aggregateId, string payload, Guid correlationId, Guid tenantId)
    {
        Id = id;
        EventType = eventType;
        EventVersion = eventVersion;
        AggregateId = aggregateId;
        Payload = payload;
        OccurredAtUtc = DateTime.UtcNow;
        CreatedAtUtc = DateTime.UtcNow;
        Status = OutboxMessageStatus.Pending;
        RetryCount = 0;
        CorrelationId = correlationId;
        TenantId = tenantId;
    }
}
