namespace Declaration.Domain.Entities;

public class DeclarationPartyRole
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DeclarationVersionId { get; set; }
    public Guid DeclarationPartyId { get; set; }
    
    public string RoleType { get; set; } = string.Empty; 
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public DeclarationVersion DeclarationVersion { get; set; } = null!;
    public DeclarationParty DeclarationParty { get; set; } = null!;
}
