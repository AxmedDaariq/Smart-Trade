namespace Declaration.Domain.Entities;

public class CustomsDeclaration
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid? OrganizationId { get; set; }
    
    public string DeclarationNumber { get; set; } = string.Empty;
    public string DeclarationType { get; set; } = string.Empty; // Import, Export, Transit, etc.
    public string ProcedureCode { get; set; } = string.Empty;
    public string Status { get; set; } = "Draft"; // Draft, ValidationPending, Submitted, UnderReview, Accepted, Rejected, etc.
    
    public int CurrentVersionNumber { get; set; } = 1;
    
    public DateTime? SubmittedAtUtc { get; set; }
    public DateTime? AcceptedAtUtc { get; set; }
    public DateTime? RejectedAtUtc { get; set; }
    public DateTime? CancelledAtUtc { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public Guid CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    
    public byte[] Version { get; set; } = Array.Empty<byte>();
    
    public ICollection<DeclarationVersion> Versions { get; set; } = new List<DeclarationVersion>();
    public ICollection<DeclarationReference> References { get; set; } = new List<DeclarationReference>();
    public ICollection<DeclarationValidation> Validations { get; set; } = new List<DeclarationValidation>();
    public ICollection<DeclarationSubmission> Submissions { get; set; } = new List<DeclarationSubmission>();
    public ICollection<DeclarationDecision> Decisions { get; set; } = new List<DeclarationDecision>();
    public ICollection<DeclarationAmendment> Amendments { get; set; } = new List<DeclarationAmendment>();
    public ICollection<DeclarationStatusHistory> StatusHistories { get; set; } = new List<DeclarationStatusHistory>();
    public ICollection<DeclarationExternalReference> ExternalReferences { get; set; } = new List<DeclarationExternalReference>();
    public ICollection<DeclarationIntegrationRecord> IntegrationRecords { get; set; } = new List<DeclarationIntegrationRecord>();
}
