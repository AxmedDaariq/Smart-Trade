using System;

namespace BuildingBlocks.Contracts.Events;

public interface IIntegrationEvent
{
    Guid EventId { get; }
    string EventType { get; }
    int EventVersion { get; }
    DateTime OccurredAtUtc { get; }
    DateTime PublishedAtUtc { get; }
    Guid CorrelationId { get; }
    Guid? CausationId { get; }
    string TraceId { get; }
    Guid TenantId { get; }
    Guid ActorId { get; }
    string SourceService { get; }
    string AggregateType { get; }
    string AggregateId { get; }
    string SchemaVersion { get; }
}

public abstract record IntegrationEvent(
    Guid EventId,
    string EventType,
    int EventVersion,
    DateTime OccurredAtUtc,
    DateTime PublishedAtUtc,
    Guid CorrelationId,
    Guid? CausationId,
    string TraceId,
    Guid TenantId,
    Guid ActorId,
    string SourceService,
    string AggregateType,
    string AggregateId,
    string SchemaVersion
) : IIntegrationEvent;
