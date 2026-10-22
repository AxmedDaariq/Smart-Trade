using System;

namespace Ectn.Domain.Entities;

public class EctnApproval
{
    public Guid Id { get; private set; }
    public Guid EctnId { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid ApproverId { get; private set; }
    public string ApproverRole { get; private set; }
    public string Decision { get; private set; } // Approved, Rejected
    public string Reason { get; private set; }
    public DateTime Timestamp { get; private set; }
    public string Step { get; private set; }
    public string IPAddress { get; private set; }
    public Guid CorrelationId { get; private set; }

    protected EctnApproval() { }

    public EctnApproval(Guid ectnId, Guid tenantId, Guid approverId, string decision, string reason, string ipAddress)
    {
        Id = Guid.NewGuid();
        EctnId = ectnId;
        TenantId = tenantId;
        ApproverId = approverId;
        Decision = decision;
        Reason = reason;
        Timestamp = DateTime.UtcNow;
        IPAddress = ipAddress;
    }
}
