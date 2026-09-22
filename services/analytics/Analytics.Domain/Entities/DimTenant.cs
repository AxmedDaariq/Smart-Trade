using System;

namespace Analytics.Domain.Entities;

public class DimTenant
{
    public Guid Id { get; private set; } // Matches actual TenantId
    public string Name { get; private set; }
    public string Type { get; private set; } // Logistics, Government, TMA

    protected DimTenant() { }

    public DimTenant(Guid id, string name, string type)
    {
        Id = id;
        Name = name;
        Type = type;
    }
}
