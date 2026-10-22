using System;

namespace Ectn.Domain.Entities;

public class BillOfLading
{
    public Guid Id { get; private set; }
    public Guid EctnId { get; private set; }
    public Guid TenantId { get; private set; }
    public string BolNumber { get; private set; }
    public string BolType { get; private set; } // Master, House
    public DateTime? IssueDate { get; private set; }
    public string Carrier { get; private set; }
    public string Shipper { get; private set; }
    public string Consignee { get; private set; }
    public string NotifyParty { get; private set; }
    public string PortOfLoading { get; private set; }
    public string PortOfDischarge { get; private set; }
    public string PlaceOfReceipt { get; private set; }
    public string FinalDestination { get; private set; }
    public string DocumentStatus { get; private set; }
    public string ExternalReference { get; private set; }

    protected BillOfLading() { }

    public BillOfLading(Guid ectnId, Guid tenantId, string bolNumber, string bolType)
    {
        Id = Guid.NewGuid();
        EctnId = ectnId;
        TenantId = tenantId;
        BolNumber = bolNumber;
        BolType = bolType;
    }
}
