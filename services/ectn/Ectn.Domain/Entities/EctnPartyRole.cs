namespace Ectn.Domain.Entities;

public class EctnPartyRole
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EctnApplicationId { get; set; }
    public Guid EctnPartyId { get; set; }
    
    public string RoleType { get; set; } = string.Empty; // Importer, Exporter, Shipper, Consignee, NotifyParty, FreightForwarder, Carrier, Agent, Other
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public EctnApplication EctnApplication { get; set; } = null!;
    public EctnParty EctnParty { get; set; } = null!;
}
