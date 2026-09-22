using System;

namespace Analytics.Domain.Entities;

public class KpiDefinition
{
    public string Code { get; private set; } // e.g., "avg-transit-time"
    public string Name { get; private set; }
    public string Category { get; private set; }
    public string Description { get; private set; }
    public string FormulaVersion { get; private set; }
    
    // Configuration
    public decimal TargetValue { get; private set; }
    public decimal WarningThreshold { get; private set; }
    public decimal CriticalThreshold { get; private set; }
    public string Unit { get; private set; } // e.g., "Hours", "Percentage"

    protected KpiDefinition() { }

    public KpiDefinition(string code, string name, string category, decimal targetValue, decimal warningThreshold, decimal criticalThreshold, string unit)
    {
        Code = code;
        Name = name;
        Category = category;
        TargetValue = targetValue;
        WarningThreshold = warningThreshold;
        CriticalThreshold = criticalThreshold;
        Unit = unit;
        FormulaVersion = "v1.0";
    }
}
