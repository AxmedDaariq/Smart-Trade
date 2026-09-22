namespace Ectn.Domain.Entities;

public class EctnDocumentVerification
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid EctnDocumentId { get; set; }
    
    public string VerifiedBy { get; set; } = string.Empty; // System or User ID
    public string VerificationType { get; set; } = string.Empty; // Manual, Automated, External
    
    public string Status { get; set; } = "Passed"; // Passed, Failed, Warning
    public string Reason { get; set; } = string.Empty;
    
    public DateTime VerifiedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public EctnDocument EctnDocument { get; set; } = null!;
}
