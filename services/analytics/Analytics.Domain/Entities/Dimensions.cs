namespace SmartTrade.Analytics.Domain.Entities;

public class DimTenant
{
    public Guid TenantKey { get; set; }
    public Guid TenantId { get; set; }
    public string TenantName { get; set; } = string.Empty;
    public string TenantType { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset EffectiveFrom { get; set; }
    public DateTimeOffset? EffectiveTo { get; set; }
    public bool IsCurrent { get; set; }
}

public class DimDate
{
    public int DateKey { get; set; }
    public DateTime Date { get; set; }
    public int Day { get; set; }
    public int DayOfWeek { get; set; }
    public string DayName { get; set; } = string.Empty;
    public int Week { get; set; }
    public int Month { get; set; }
    public string MonthName { get; set; } = string.Empty;
    public int Quarter { get; set; }
    public int Year { get; set; }
    public int FiscalYear { get; set; }
    public bool IsWeekend { get; set; }
}

public class DimVehicle
{
    public Guid VehicleKey { get; set; }
    public Guid VehicleId { get; set; }
    public Guid TenantId { get; set; }
    public string VehicleType { get; set; } = string.Empty;
    public string Registration { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset EffectiveFrom { get; set; }
    public DateTimeOffset? EffectiveTo { get; set; }
    public bool IsCurrent { get; set; }
}

public class DimCorridor
{
    public Guid CorridorKey { get; set; }
    public Guid CorridorId { get; set; }
    public string CorridorName { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset EffectiveFrom { get; set; }
    public DateTimeOffset? EffectiveTo { get; set; }
    public bool IsCurrent { get; set; }
}
