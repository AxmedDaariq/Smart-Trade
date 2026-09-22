using System;

namespace Declaration.Domain.Events;

public abstract class DomainEvent
{
    public Guid EventId { get; protected set; } = Guid.NewGuid();
    public DateTime OccurredAt { get; protected set; } = DateTime.UtcNow;
}

public class DeclarationCreatedEvent : DomainEvent
{
    public Guid DeclarationId { get; }
    public Guid TenantId { get; }
    public string ApplicationReference { get; }

    public DeclarationCreatedEvent(Guid declarationId, Guid tenantId, string applicationReference)
    {
        DeclarationId = declarationId;
        TenantId = tenantId;
        ApplicationReference = applicationReference;
    }
}

public class DeclarationSubmittedEvent : DomainEvent
{
    public Guid DeclarationId { get; }
    public Guid TenantId { get; }

    public DeclarationSubmittedEvent(Guid declarationId, Guid tenantId)
    {
        DeclarationId = declarationId;
        TenantId = tenantId;
    }
}

public class DeclarationValidationFailedEvent : DomainEvent
{
    public Guid DeclarationId { get; }
    public string Reason { get; }

    public DeclarationValidationFailedEvent(Guid declarationId, string reason)
    {
        DeclarationId = declarationId;
        Reason = reason;
    }
}

public class DeclarationReviewStartedEvent : DomainEvent
{
    public Guid DeclarationId { get; }
    public Guid OfficerId { get; }

    public DeclarationReviewStartedEvent(Guid declarationId, Guid officerId)
    {
        DeclarationId = declarationId;
        OfficerId = officerId;
    }
}

public class DeclarationAcceptedEvent : DomainEvent
{
    public Guid DeclarationId { get; }
    public Guid TenantId { get; }
    public string DeclarationNumber { get; }

    public DeclarationAcceptedEvent(Guid declarationId, Guid tenantId, string declarationNumber)
    {
        DeclarationId = declarationId;
        TenantId = tenantId;
        DeclarationNumber = declarationNumber;
    }
}

public class DeclarationRejectedEvent : DomainEvent
{
    public Guid DeclarationId { get; }
    public string ReasonCode { get; }
    public string Reason { get; }

    public DeclarationRejectedEvent(Guid declarationId, string reasonCode, string reason)
    {
        DeclarationId = declarationId;
        ReasonCode = reasonCode;
        Reason = reason;
    }
}

public class DeclarationResubmittedEvent : DomainEvent
{
    public Guid DeclarationId { get; }

    public DeclarationResubmittedEvent(Guid declarationId)
    {
        DeclarationId = declarationId;
    }
}

public class DeclarationAmendmentRequestedEvent : DomainEvent
{
    public Guid DeclarationId { get; }
    public string Reason { get; }

    public DeclarationAmendmentRequestedEvent(Guid declarationId, string reason)
    {
        DeclarationId = declarationId;
        Reason = reason;
    }
}

public class DeclarationAmendedEvent : DomainEvent
{
    public Guid DeclarationId { get; }

    public DeclarationAmendedEvent(Guid declarationId)
    {
        DeclarationId = declarationId;
    }
}

public class DeclarationCancelledEvent : DomainEvent
{
    public Guid DeclarationId { get; }
    public string Reason { get; }

    public DeclarationCancelledEvent(Guid declarationId, string reason)
    {
        DeclarationId = declarationId;
        Reason = reason;
    }
}

public class DeclarationDocumentUploadedEvent : DomainEvent
{
    public Guid DeclarationId { get; }
    public Guid DocumentId { get; }

    public DeclarationDocumentUploadedEvent(Guid declarationId, Guid documentId)
    {
        DeclarationId = declarationId;
        DocumentId = documentId;
    }
}

public class DeclarationAssignmentChangedEvent : DomainEvent
{
    public Guid DeclarationId { get; }
    public Guid OfficerId { get; }

    public DeclarationAssignmentChangedEvent(Guid declarationId, Guid officerId)
    {
        DeclarationId = declarationId;
        OfficerId = officerId;
    }
}
