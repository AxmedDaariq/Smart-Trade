using System;

namespace Ectn.Domain.Entities;

public class CargoShipment
{
    public Guid Id { get; private set; }
    public Guid EctnId { get; private set; }
    public Guid TenantId { get; private set; }
    public string Description { get; private set; }
    public string HSCode { get; private set; }
    public string PackageType { get; private set; }
    public int PackageCount { get; private set; }
    public decimal GrossWeight { get; private set; }
    public decimal NetWeight { get; private set; }
    public decimal Volume { get; private set; }
    public string WeightUnit { get; private set; }
    public string VolumeUnit { get; private set; }
    public string CountryOfOrigin { get; private set; }
    public string CountryOfDestination { get; private set; }
    public decimal DeclaredValue { get; private set; }
    public string Currency { get; private set; }
    public string MarksAndNumbers { get; private set; }
    public bool DangerousGoodsIndicator { get; private set; }
    public bool TemperatureControlled { get; private set; }
    public string CommodityCode { get; private set; }

    protected CargoShipment() { }

    public CargoShipment(Guid ectnId, Guid tenantId, string description, string hsCode, decimal grossWeight)
    {
        Id = Guid.NewGuid();
        EctnId = ectnId;
        TenantId = tenantId;
        Description = description;
        HSCode = hsCode;
        GrossWeight = grossWeight;
    }
}
