using System;

namespace Ectn.Contracts.Events;

public class EctnSubmittedIntegrationEvent
{
    public Guid EventId { get; set; } = Guid.NewGuid();
    public string EventType { get; set; } = "EctnSubmitted";
    public DateTime OccurredAt { get; set; } = DateTime.UtcNow;
    public Guid CorrelationId { get; set; }
    public Guid TenantId { get; set; }
    public Guid AggregateId { get; set; }
    public string AggregateType { get; set; } = "ECTN";
    public int SchemaVersion { get; set; } = 1;
    
    // Payload properties
    public string EctnNumber { get; set; }
    public string ApplicationReference { get; set; }
    public Guid SubmittedBy { get; set; }
}
