namespace Declaration.Domain.Entities;

public class DeclarationParty
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DeclarationVersionId { get; set; }
    
    public string PartyType { get; set; } = string.Empty; // Importer, Exporter, Consignee, Declarant, CustomsAgent, NotifyParty, Carrier, Buyer, Seller, Other
    
    public string OrganizationReference { get; set; } = string.Empty;
    public string LegalName { get; set; } = string.Empty;
    public string TradingName { get; set; } = string.Empty;
    public string RegistrationNumber { get; set; } = string.Empty;
    public string? TaxIdentifier { get; set; }
    
    public string CountryCode { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    
    public string ContactName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public DeclarationVersion DeclarationVersion { get; set; } = null!;
    
    public ICollection<DeclarationPartyRole> Roles { get; set; } = new List<DeclarationPartyRole>();
}
