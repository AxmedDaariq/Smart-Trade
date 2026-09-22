namespace Declaration.Domain.Entities;

public class DeclarationValidation
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid DeclarationId { get; set; }
    public Guid DeclarationVersionId { get; set; }
    
    public string ValidationType { get; set; } = string.Empty; // Schema, BusinessRule, Reference, ECTN, Party, Cargo, Document, Duplicate, Procedure, Transport, Other
    public string Status { get; set; } = "Pending"; // Pending, Passed, Failed, Warning
    
    public string ValidationCode { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    
    public string Severity { get; set; } = "Info"; // Info, Warning, Error, Critical
    
    public DateTime ValidatedAtUtc { get; set; } = DateTime.UtcNow;
    public Guid? ValidatedBy { get; set; }
    
    public string DetailsJson { get; set; } = "{}"; // JSONB
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public CustomsDeclaration CustomsDeclaration { get; set; } = null!;
}
