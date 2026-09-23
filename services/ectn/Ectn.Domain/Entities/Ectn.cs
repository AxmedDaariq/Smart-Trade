using Ectn.Domain.Enums;

namespace Ectn.Domain.Entities;

/// <summary>
/// ECTN Aggregate Root – Electronic Cargo Tracking Note.
/// Encapsulates all domain invariants and state transitions.
/// </summary>
public class Ectn
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid TenantId { get; private set; }
    public string ApplicationReference { get; private set; }
    public EctnType Type { get; private set; }
    public EctnStatus Status { get; private set; } = EctnStatus.Draft;

    public string? EctnNumber { get; private set; }
    public string? RejectionReason { get; private set; }
    public string? CancellationReason { get; private set; }

    public DateTime CreatedAtUtc { get; private set; } = DateTime.UtcNow;
    public Guid CreatedBy { get; private set; }

    public DateTime? SubmittedAt { get; private set; }
    public DateTime? ReviewedAt { get; private set; }
    public DateTime? ApprovedAt { get; private set; }
    public DateTime? IssuedAt { get; private set; }
    public DateTime? RejectedAt { get; private set; }
    public DateTime? CancelledAt { get; private set; }

    public ICollection<EctnParty> Parties { get; private set; } = new List<EctnParty>();
    public ICollection<EctnStatusHistory> StatusHistories { get; private set; } = new List<EctnStatusHistory>();
    public ICollection<EctnDocument> Documents { get; private set; } = new List<EctnDocument>();

    // Required by EF Core
    protected Ectn() { ApplicationReference = string.Empty; }

    public Ectn(Guid tenantId, string applicationReference, EctnType type, Guid createdBy)
    {
        if (tenantId == Guid.Empty) throw new ArgumentException("TenantId cannot be empty.", nameof(tenantId));
        if (string.IsNullOrWhiteSpace(applicationReference)) throw new ArgumentException("ApplicationReference cannot be empty.", nameof(applicationReference));
        if (createdBy == Guid.Empty) throw new ArgumentException("CreatedBy cannot be empty.", nameof(createdBy));

        TenantId = tenantId;
        ApplicationReference = applicationReference;
        Type = type;
        CreatedBy = createdBy;

        AddStatusHistory(EctnStatus.Draft, createdBy);
    }

    // ─── Domain Behaviours ───────────────────────────────────────────

    public void Submit(Guid actorId)
    {
        if (Status != EctnStatus.Draft)
            throw new InvalidOperationException("Only draft ECTNs can be submitted.");

        Status = EctnStatus.Submitted;
        SubmittedAt = DateTime.UtcNow;
        AddStatusHistory(EctnStatus.Submitted, actorId);
    }

    public void StartReview(Guid actorId)
    {
        if (Status != EctnStatus.Submitted)
            throw new InvalidOperationException("Only submitted ECTNs can be placed under review.");

        Status = EctnStatus.UnderReview;
        ReviewedAt = DateTime.UtcNow;
        AddStatusHistory(EctnStatus.UnderReview, actorId);
    }

    public void Approve(Guid actorId)
    {
        if (Status != EctnStatus.Submitted && Status != EctnStatus.UnderReview)
            throw new InvalidOperationException("ECTN must be Submitted or Under Review to be approved.");

        Status = EctnStatus.Approved;
        ApprovedAt = DateTime.UtcNow;
        AddStatusHistory(EctnStatus.Approved, actorId);
    }

    public void Issue(string ectnNumber, Guid actorId)
    {
        if (Status != EctnStatus.Approved)
            throw new InvalidOperationException("Only approved ECTNs can be issued.");
        if (string.IsNullOrWhiteSpace(ectnNumber))
            throw new ArgumentException("ECTN number is required for issuance.", nameof(ectnNumber));

        EctnNumber = ectnNumber;
        Status = EctnStatus.Issued;
        IssuedAt = DateTime.UtcNow;
        AddStatusHistory(EctnStatus.Issued, actorId);
    }

    public void Reject(string reason, Guid actorId)
    {
        if (Status == EctnStatus.Issued || Status == EctnStatus.Cancelled || Status == EctnStatus.Expired)
            throw new InvalidOperationException("Cannot reject an ECTN that is already Issued, Cancelled, or Expired.");
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Rejection reason is required.", nameof(reason));

        RejectionReason = reason;
        Status = EctnStatus.Rejected;
        RejectedAt = DateTime.UtcNow;
        AddStatusHistory(EctnStatus.Rejected, actorId);
    }

    public void Cancel(string reason, Guid actorId)
    {
        if (Status == EctnStatus.Cancelled || Status == EctnStatus.Expired)
            throw new InvalidOperationException("ECTN is already cancelled or expired.");
        if (string.IsNullOrWhiteSpace(reason))
            throw new ArgumentException("Cancellation reason is required.", nameof(reason));

        CancellationReason = reason;
        Status = EctnStatus.Cancelled;
        CancelledAt = DateTime.UtcNow;
        AddStatusHistory(EctnStatus.Cancelled, actorId);
    }

    public void AssignEctnNumber(string ectnNumber)
    {
        if (string.IsNullOrWhiteSpace(ectnNumber))
            throw new ArgumentException("ECTN number cannot be empty.", nameof(ectnNumber));

        EctnNumber = ectnNumber;
    }

    public void AddParty(EctnParty party)
    {
        if (party == null) throw new ArgumentNullException(nameof(party));
        Parties.Add(party);
    }

    // ─── Private Helpers ─────────────────────────────────────────────

    private void AddStatusHistory(EctnStatus status, Guid actorId)
    {
        StatusHistories.Add(new EctnStatusHistory
        {
            Status = status,
            ChangedAt = DateTime.UtcNow,
            ChangedBy = actorId
        });
    }
}
