using System;

namespace Analytics.Domain.Entities;

public class ReportDefinition
{
    public Guid Id { get; private set; }
    public Guid? TenantId { get; private set; } // Null implies global/platform report
    
    public string Name { get; private set; }
    public string Type { get; private set; } // e.g., "Executive", "Corridor", "Fleet"
    public string Format { get; private set; } // "PDF", "CSV", "Excel"
    
    public string ScheduleCron { get; private set; } // e.g., "0 0 * * *"
    public bool IsActive { get; private set; }

    protected ReportDefinition() { }

    public ReportDefinition(Guid? tenantId, string name, string type, string format, string scheduleCron)
    {
        Id = Guid.NewGuid();
        TenantId = tenantId;
        Name = name;
        Type = type;
        Format = format;
        ScheduleCron = scheduleCron;
        IsActive = true;
    }
}
