using System;

namespace Declaration.Contracts.Events;

public class DeclarationAcceptedIntegrationEvent
{
    public Guid EventId { get; set; } = Guid.NewGuid();
    public string EventType { get; set; } = "DeclarationAccepted";
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    public Guid CorrelationId { get; set; }
    public Guid CausationId { get; set; }
    public Guid TenantId { get; set; }
    public Guid AggregateId { get; set; }
    public string AggregateType { get; set; } = "Declaration";
    public int SchemaVersion { get; set; } = 1;

    public string DeclarationNumber { get; set; }
}
