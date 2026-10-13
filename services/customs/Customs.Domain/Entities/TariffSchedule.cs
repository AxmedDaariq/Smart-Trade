using System;
using Customs.Domain.Enums;
using System.Collections.Generic;

namespace Customs.Domain.Entities;

public class TariffSchedule
{
    public Guid Id { get; private set; }
    public string Code { get; private set; }
    public string Name { get; private set; }
    public string Version { get; private set; }
    public DateTime EffectiveFrom { get; private set; }
    public DateTime? EffectiveTo { get; private set; }
    public string Status { get; private set; }
    public string Country { get; private set; }
    public string RegulatorySource { get; private set; }
    public DateTime? PublishedAt { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid CreatedBy { get; private set; }

    private readonly List<TariffItem> _items = new List<TariffItem>();
    public IReadOnlyCollection<TariffItem> Items => _items.AsReadOnly();

    protected TariffSchedule() { }

    public TariffSchedule(
        string code,
        string name,
        string version,
        DateTime effectiveFrom,
        DateTime? effectiveTo,
        string country,
        string regulatorySource,
        Guid createdBy)
    {
        Id = Guid.NewGuid();
        Code = code;
        Name = name;
        Version = version;
        EffectiveFrom = effectiveFrom;
        EffectiveTo = effectiveTo;
        Country = country;
        RegulatorySource = regulatorySource;
        Status = "Active";
        CreatedAt = DateTime.UtcNow;
        CreatedBy = createdBy;
    }
}

public class TariffItem
{
    public Guid Id { get; private set; }
    public Guid TariffScheduleId { get; private set; }
    public string HSCode { get; private set; }
    public string Description { get; private set; }
    public RateType RateType { get; private set; }
    public decimal Rate { get; private set; }
    public string Unit { get; private set; }
    public decimal? MinimumRate { get; private set; }
    public decimal? MaximumRate { get; private set; }
    public string ProcedureCode { get; private set; }
    public string OriginRule { get; private set; }
    public DateTime EffectiveFrom { get; private set; }
    public DateTime? EffectiveTo { get; private set; }

    protected TariffItem() { }

    public TariffItem(
        Guid tariffScheduleId,
        string hsCode,
        string description,
        RateType rateType,
        decimal rate,
        string unit,
        string procedureCode,
        string originRule,
        DateTime effectiveFrom,
        DateTime? effectiveTo = null)
    {
        Id = Guid.NewGuid();
        TariffScheduleId = tariffScheduleId;
        HSCode = hsCode;
        Description = description;
        RateType = rateType;
        Rate = rate;
        Unit = unit;
        ProcedureCode = procedureCode;
        OriginRule = originRule;
        EffectiveFrom = effectiveFrom;
        EffectiveTo = effectiveTo;
    }
}
