namespace SmartTrade.Analytics.Domain.Entities;

public class KpiDefinition
{
    public Guid KpiDefinitionId { get; set; }
    public string KpiCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string FormulaDefinition { get; set; } = string.Empty;
    public decimal Target { get; set; }
    public string Frequency { get; set; } = string.Empty;
    public string Owner { get; set; } = string.Empty;
    public int Version { get; set; }
    public DateTimeOffset EffectiveFrom { get; set; }
    public DateTimeOffset? EffectiveTo { get; set; }
    public string Status { get; set; } = "ACTIVE";
}

public class KpiResult
{
    public Guid KpiResultId { get; set; }
    public Guid KpiDefinitionId { get; set; }
    public Guid TenantId { get; set; }
    public int DateKey { get; set; }
    public decimal Result { get; set; }
    public Guid CalculationRunId { get; set; }
    public DateTimeOffset CalculationTimestamp { get; set; }
}

public class IndicatorDefinition
{
    public Guid IndicatorId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Baseline { get; set; }
    public decimal Target { get; set; }
    public string ReportingPeriod { get; set; } = string.Empty;
}
