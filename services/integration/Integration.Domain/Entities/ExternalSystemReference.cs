using System;

namespace Integration.Domain.Entities;

public class ExternalSystemReference
{
    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public Guid IntegrationId { get; private set; }
    
    public string ExternalSystem { get; private set; } // e.g., "CustomsAuthority"
    public string ExternalEntityType { get; private set; } // e.g., "CustomsCase"
    public string ExternalEntityId { get; private set; } // The ID in their system
    
    public string InternalEntityType { get; private set; } // e.g., "ClearanceCase"
    public string InternalEntityId { get; private set; } // Our internal ID
    
    public string ExternalReference { get; private set; } // E.g., The human readable code
    public string MappingStatus { get; private set; }
    
    public DateTime FirstSeenAt { get; private set; }
    public DateTime LastSeenAt { get; private set; }
    public string Version { get; private set; }

    protected ExternalSystemReference() { }

    public ExternalSystemReference(
        Guid tenantId,
        Guid integrationId,
        string externalSystem,
        string externalEntityType,
        string externalEntityId,
        string internalEntityType,
        string internalEntityId,
        string externalReference)
    {
        Id = Guid.NewGuid();
        TenantId = tenantId;
        IntegrationId = integrationId;
        ExternalSystem = externalSystem;
        ExternalEntityType = externalEntityType;
        ExternalEntityId = externalEntityId;
        InternalEntityType = internalEntityType;
        InternalEntityId = internalEntityId;
        ExternalReference = externalReference;
        MappingStatus = "Mapped";
        FirstSeenAt = DateTime.UtcNow;
        LastSeenAt = DateTime.UtcNow;
    }
}
