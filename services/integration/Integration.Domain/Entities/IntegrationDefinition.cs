using System;
using Integration.Domain.Enums;

namespace Integration.Domain.Entities;

public class IntegrationDefinition
{
    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public string Code { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    
    public Guid PartnerId { get; private set; }
    
    public SystemType SystemType { get; private set; }
    public Direction Direction { get; private set; }
    public Protocol Protocol { get; private set; }
    
    public string Environment { get; private set; }
    public IntegrationStatus Status { get; private set; }
    public string Version { get; private set; }
    
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }

    protected IntegrationDefinition() { }

    public IntegrationDefinition(
        Guid tenantId,
        Guid partnerId,
        string code,
        string name,
        SystemType systemType,
        Direction direction,
        Protocol protocol,
        string version,
        string environment)
    {
        Id = Guid.NewGuid();
        TenantId = tenantId;
        PartnerId = partnerId;
        Code = code;
        Name = name;
        SystemType = systemType;
        Direction = direction;
        Protocol = protocol;
        Version = version;
        Environment = environment;
        Status = IntegrationStatus.Active;
        CreatedAt = DateTime.UtcNow;
    }
}
