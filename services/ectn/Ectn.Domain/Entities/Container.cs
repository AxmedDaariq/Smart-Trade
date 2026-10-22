using System;

namespace Ectn.Domain.Entities;

public class Container
{
    public Guid Id { get; private set; }
    public Guid EctnId { get; private set; }
    public Guid TenantId { get; private set; }
    public string ContainerNumber { get; private set; }
    public string ContainerType { get; private set; } // 20FT, 40FT, etc.
    public string SealNumber { get; private set; }
    public decimal GrossWeight { get; private set; }
    public decimal TareWeight { get; private set; }
    public decimal NetWeight { get; private set; }
    public string Status { get; private set; }

    protected Container() { }

    public Container(Guid ectnId, Guid tenantId, string containerNumber, string containerType)
    {
        Id = Guid.NewGuid();
        EctnId = ectnId;
        TenantId = tenantId;
        ContainerNumber = containerNumber;
        ContainerType = containerType;
    }
}
