using System;
using Corridor.Domain.Enums;

namespace Corridor.Domain.Entities;

public class CorridorAggregate
{
    public Guid Id { get; private set; }
    public Guid TenantId { get; private set; }
    public string CorridorCode { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }
    public string Origin { get; private set; }
    public string Destination { get; private set; }
    public string CountryFrom { get; private set; }
    public string CountryTo { get; private set; }
    public CorridorStatus Status { get; private set; }
    public bool Active { get; private set; }
    public DateTime? EffectiveFrom { get; private set; }
    public DateTime? EffectiveTo { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime? UpdatedAt { get; private set; }
    public byte[] Version { get; private set; }

    protected CorridorAggregate() { }

    public CorridorAggregate(Guid tenantId, string corridorCode, string name, string origin, string destination)
    {
        Id = Guid.NewGuid();
        TenantId = tenantId;
        CorridorCode = corridorCode;
        Name = name;
        Origin = origin;
        Destination = destination;
        Status = CorridorStatus.Draft;
        Active = false;
        CreatedAt = DateTime.UtcNow;
    }
}
