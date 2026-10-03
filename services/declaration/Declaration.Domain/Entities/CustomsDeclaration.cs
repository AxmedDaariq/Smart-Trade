namespace Declaration.Domain.Entities;

public class CustomsDeclaration
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid? OrganizationId { get; set; }
    
    public string DeclarationNumber { get; set; } = string.Empty;
    public string ApplicationReference { get; set; } = string.Empty;
    public string EctnReference { get; set; } = string.Empty;
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

    public CustomsDeclaration() { }

    public CustomsDeclaration(
        Guid tenantId,
        string applicationReference,
        Declaration.Domain.Enums.DeclarationType declarationType,
        string procedureCode,
        Guid createdBy)
    {
        TenantId = tenantId;
        ApplicationReference = applicationReference ?? string.Empty;
        DeclarationType = declarationType.ToString();
        ProcedureCode = procedureCode ?? string.Empty;
        CreatedBy = createdBy;
        Status = Declaration.Domain.Enums.DeclarationStatus.Draft.ToString();
        CreatedAtUtc = DateTime.UtcNow;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void AssignEctnReference(string ectnReference)
    {
        EctnReference = ectnReference ?? string.Empty;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Submit(Guid userId)
    {
        Status = Declaration.Domain.Enums.DeclarationStatus.Submitted.ToString();
        SubmittedAtUtc = DateTime.UtcNow;
        UpdatedBy = userId;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Accept(string declarationNumber, Guid userId)
    {
        DeclarationNumber = declarationNumber ?? string.Empty;
        Status = Declaration.Domain.Enums.DeclarationStatus.Accepted.ToString();
        AcceptedAtUtc = DateTime.UtcNow;
        UpdatedBy = userId;
        UpdatedAtUtc = DateTime.UtcNow;
    }

    public void Reject(string reasonCode, string reason, Guid userId)
    {
        Status = Declaration.Domain.Enums.DeclarationStatus.Rejected.ToString();
        RejectedAtUtc = DateTime.UtcNow;
        UpdatedBy = userId;
        UpdatedAtUtc = DateTime.UtcNow;
    }
}
