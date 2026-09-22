namespace Declaration.Domain.Entities;

public class DeclarationVersion
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DeclarationId { get; set; }
    
    public int VersionNumber { get; set; }
    public string VersionStatus { get; set; } = "Draft"; // Draft, Submitted, Accepted, Rejected, Superseded, Cancelled
    
    public string DeclarationType { get; set; } = string.Empty;
    public string ProcedureCode { get; set; } = string.Empty;
    
    public DateTime DeclarationDateUtc { get; set; } = DateTime.UtcNow;
    public DateTime EffectiveDateUtc { get; set; } = DateTime.UtcNow;
    public DateTime? SubmissionDateUtc { get; set; }
    
    public decimal TotalDeclaredValue { get; set; }
    public string CurrencyCode { get; set; } = string.Empty;
    
    public int TotalPackages { get; set; }
    public decimal TotalGrossWeight { get; set; }
    public decimal TotalNetWeight { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public Guid CreatedBy { get; set; }
    
    public string ChangeReason { get; set; } = string.Empty;
    public string SnapshotHash { get; set; } = string.Empty;
    
    public byte[] Version { get; set; } = Array.Empty<byte>();
    
    public CustomsDeclaration CustomsDeclaration { get; set; } = null!;
    
    public ICollection<DeclarationParty> Parties { get; set; } = new List<DeclarationParty>();
    public ICollection<DeclarationCargo> Cargos { get; set; } = new List<DeclarationCargo>();
    public ICollection<DeclarationContainer> Containers { get; set; } = new List<DeclarationContainer>();
    public ICollection<DeclarationTransport> Transports { get; set; } = new List<DeclarationTransport>();
    public ICollection<DeclarationRoute> Routes { get; set; } = new List<DeclarationRoute>();
    public ICollection<DeclarationDocument> Documents { get; set; } = new List<DeclarationDocument>();
}
