using System;

namespace Declaration.Contracts.Events;

public class DeclarationRejectedIntegrationEvent
{
    public Guid EventId { get; set; } = Guid.NewGuid();
    public string EventType { get; set; } = "DeclarationRejected";
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    public Guid CorrelationId { get; set; }
    public Guid CausationId { get; set; }
    public Guid TenantId { get; set; }
    public Guid AggregateId { get; set; }
    public string AggregateType { get; set; } = "Declaration";
    public int SchemaVersion { get; set; } = 1;

    public string ReasonCode { get; set; }
    public string Reason { get; set; }
}
