using System;
using Integration.Domain.Enums;

namespace Integration.Domain.Entities;

public class Partner
{
    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public string PartnerCode { get; private set; }
    public string LegalName { get; private set; }
    public string DisplayName { get; private set; }
    public SystemType PartnerType { get; private set; }
    
    public PartnerStatus Status { get; private set; }
    public string Country { get; private set; }
    public string Environment { get; private set; } // e.g., "Production", "Sandbox"
    public string ContactReference { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    protected Partner() { }

    public Partner(Guid tenantId, string partnerCode, string legalName, SystemType partnerType, string environment)
    {
        Id = Guid.NewGuid();
        TenantId = tenantId;
        PartnerCode = partnerCode;
        LegalName = legalName;
        DisplayName = legalName;
        PartnerType = partnerType;
        Environment = environment;
        Status = PartnerStatus.Active;
        CreatedAt = DateTime.UtcNow;
    }
}
