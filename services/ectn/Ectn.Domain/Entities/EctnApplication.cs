namespace Ectn.Domain.Entities;

public class EctnApplication
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid TenantId { get; set; }
    public Guid? OrganizationId { get; set; }
    
    public string ApplicationNumber { get; set; } = string.Empty;
    public string ApplicationType { get; set; } = string.Empty; // New, Amendment, Renewal, Reissue
    public string Status { get; set; } = "Draft"; // Draft, Submitted, ValidationPending, UnderReview, Approved, Rejected, Issued, AmendmentPending, Amended, Cancelled, Expired
    
    public DateTime? SubmittedAtUtc { get; set; }
    public DateTime? ReviewedAtUtc { get; set; }
    public DateTime? ApprovedAtUtc { get; set; }
    public DateTime? RejectedAtUtc { get; set; }
    public DateTime? IssuedAtUtc { get; set; }
    public DateTime? CancelledAtUtc { get; set; }
    public DateTime? ExpiresAtUtc { get; set; }
    
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAtUtc { get; set; } = DateTime.UtcNow;
    
    public Guid CreatedBy { get; set; }
    public Guid? UpdatedBy { get; set; }
    
    public byte[] Version { get; set; } = Array.Empty<byte>();
    
    public ICollection<EctnCertificate> Certificates { get; set; } = new List<EctnCertificate>();
    public ICollection<EctnShipment> Shipments { get; set; } = new List<EctnShipment>();
    public ICollection<EctnPartyRole> PartyRoles { get; set; } = new List<EctnPartyRole>();
    public ICollection<EctnDocument> Documents { get; set; } = new List<EctnDocument>();
    public ICollection<EctnStatusHistory> StatusHistories { get; set; } = new List<EctnStatusHistory>();
}
