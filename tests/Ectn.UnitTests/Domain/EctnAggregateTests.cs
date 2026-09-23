using System;
using Xunit;
using FluentAssertions;
using Ectn.Domain.Entities;
using Ectn.Domain.Enums;
using EctnEntity = Ectn.Domain.Entities.Ectn;

namespace Ectn.UnitTests.Domain;

public class EctnAggregateTests
{
    // ─── Helpers ────────────────────────────────────────────────────────────────

    private static EctnEntity CreateDraftEctn(Guid? tenantId = null, Guid? actorId = null)
    {
        return new EctnEntity(
            tenantId ?? Guid.NewGuid(),
            "APP-REF-001",
            EctnType.Standard,
            actorId ?? Guid.NewGuid()
        );
    }

    // ─── Construction ────────────────────────────────────────────────────────────

    [Fact]
    public void Constructor_Should_CreateDraftEctn_WithValidInputs()
    {
        var tenantId = Guid.NewGuid();
        var actorId = Guid.NewGuid();

        var ectn = new EctnEntity(tenantId, "APP-REF-100", EctnType.Containerized, actorId);

        ectn.Id.Should().NotBe(Guid.Empty);
        ectn.TenantId.Should().Be(tenantId);
        ectn.ApplicationReference.Should().Be("APP-REF-100");
        ectn.Type.Should().Be(EctnType.Containerized);
        ectn.Status.Should().Be(EctnStatus.Draft);
        ectn.CreatedBy.Should().Be(actorId);
        ectn.StatusHistories.Should().ContainSingle(h => h.Status == EctnStatus.Draft);
    }

    [Fact]
    public void Constructor_Should_Throw_When_TenantIdIsEmpty()
    {
        Action act = () => new EctnEntity(Guid.Empty, "APP-REF-001", EctnType.Standard, Guid.NewGuid());
        act.Should().Throw<ArgumentException>().WithParameterName("tenantId");
    }

    [Fact]
    public void Constructor_Should_Throw_When_ApplicationReferenceIsEmpty()
    {
        Action act = () => new EctnEntity(Guid.NewGuid(), "", EctnType.Standard, Guid.NewGuid());
        act.Should().Throw<ArgumentException>().WithParameterName("applicationReference");
    }

    [Fact]
    public void Constructor_Should_Throw_When_CreatedByIsEmpty()
    {
        Action act = () => new EctnEntity(Guid.NewGuid(), "APP-REF-001", EctnType.Standard, Guid.Empty);
        act.Should().Throw<ArgumentException>().WithParameterName("createdBy");
    }

    // ─── Submit ──────────────────────────────────────────────────────────────────

    [Fact]
    public void Submit_Should_ChangeStatusToSubmitted_When_StatusIsDraft()
    {
        var actorId = Guid.NewGuid();
        var ectn = CreateDraftEctn(actorId: actorId);

        ectn.Submit(actorId);

        ectn.Status.Should().Be(EctnStatus.Submitted);
        ectn.SubmittedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        ectn.StatusHistories.Should().Contain(h => h.Status == EctnStatus.Submitted);
    }

    [Fact]
    public void Submit_Should_ThrowException_When_StatusIsNotDraft()
    {
        var actorId = Guid.NewGuid();
        var ectn = CreateDraftEctn(actorId: actorId);
        ectn.Submit(actorId); // now Submitted

        Action act = () => ectn.Submit(actorId);
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("Only draft ECTNs can be submitted.");
    }

    [Fact]
    public void Submit_Should_RecordStatusHistoryEntry()
    {
        var actorId = Guid.NewGuid();
        var ectn = CreateDraftEctn(actorId: actorId);

        ectn.Submit(actorId);

        ectn.StatusHistories.Should().HaveCount(2); // Draft + Submitted
        ectn.StatusHistories.Should().ContainSingle(h =>
            h.Status == EctnStatus.Submitted && h.ChangedBy == actorId);
    }

    // ─── StartReview ─────────────────────────────────────────────────────────────

    [Fact]
    public void StartReview_Should_SetStatusToUnderReview_When_Submitted()
    {
        var actorId = Guid.NewGuid();
        var ectn = CreateDraftEctn(actorId: actorId);
        ectn.Submit(actorId);

        ectn.StartReview(actorId);

        ectn.Status.Should().Be(EctnStatus.UnderReview);
        ectn.ReviewedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void StartReview_Should_Throw_When_StatusIsNotSubmitted()
    {
        var ectn = CreateDraftEctn();

        Action act = () => ectn.StartReview(Guid.NewGuid());
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("Only submitted ECTNs can be placed under review.");
    }

    // ─── Approve ─────────────────────────────────────────────────────────────────

    [Fact]
    public void Approve_Should_SetStatusToApproved_When_UnderReview()
    {
        var actorId = Guid.NewGuid();
        var ectn = CreateDraftEctn(actorId: actorId);
        ectn.Submit(actorId);
        ectn.StartReview(actorId);

        ectn.Approve(actorId);

        ectn.Status.Should().Be(EctnStatus.Approved);
        ectn.ApprovedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void Approve_Should_SetStatusToApproved_When_Submitted()
    {
        var actorId = Guid.NewGuid();
        var ectn = CreateDraftEctn(actorId: actorId);
        ectn.Submit(actorId);

        ectn.Approve(actorId);

        ectn.Status.Should().Be(EctnStatus.Approved);
    }

    [Fact]
    public void Approve_Should_Throw_When_StatusIsDraft()
    {
        var ectn = CreateDraftEctn();

        Action act = () => ectn.Approve(Guid.NewGuid());
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("ECTN must be Submitted or Under Review to be approved.");
    }

    // ─── Issue ───────────────────────────────────────────────────────────────────

    [Fact]
    public void Issue_Should_SetStatusToIssued_And_AssignEctnNumber_When_Approved()
    {
        var actorId = Guid.NewGuid();
        var ectn = CreateDraftEctn(actorId: actorId);
        ectn.Submit(actorId);
        ectn.Approve(actorId);

        ectn.Issue("ECTN-2026-000001", actorId);

        ectn.Status.Should().Be(EctnStatus.Issued);
        ectn.EctnNumber.Should().Be("ECTN-2026-000001");
        ectn.IssuedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void Issue_Should_Throw_When_StatusIsNotApproved()
    {
        var actorId = Guid.NewGuid();
        var ectn = CreateDraftEctn(actorId: actorId);
        ectn.Submit(actorId);

        Action act = () => ectn.Issue("ECTN-2026-000001", actorId);
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("Only approved ECTNs can be issued.");
    }

    [Fact]
    public void Issue_Should_Throw_When_EctnNumberIsEmpty()
    {
        var actorId = Guid.NewGuid();
        var ectn = CreateDraftEctn(actorId: actorId);
        ectn.Submit(actorId);
        ectn.Approve(actorId);

        Action act = () => ectn.Issue("", actorId);
        act.Should().Throw<ArgumentException>().WithParameterName("ectnNumber");
    }

    // ─── Reject ──────────────────────────────────────────────────────────────────

    [Fact]
    public void Reject_Should_SetStatusToRejected_With_Reason()
    {
        var actorId = Guid.NewGuid();
        var ectn = CreateDraftEctn(actorId: actorId);
        ectn.Submit(actorId);

        ectn.Reject("Missing Bill of Lading.", actorId);

        ectn.Status.Should().Be(EctnStatus.Rejected);
        ectn.RejectionReason.Should().Be("Missing Bill of Lading.");
        ectn.RejectedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void Reject_Should_Throw_When_EctnIsAlreadyIssued()
    {
        var actorId = Guid.NewGuid();
        var ectn = CreateDraftEctn(actorId: actorId);
        ectn.Submit(actorId);
        ectn.Approve(actorId);
        ectn.Issue("ECTN-2026-000001", actorId);

        Action act = () => ectn.Reject("Invalid", actorId);
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("Cannot reject an ECTN that is already Issued, Cancelled, or Expired.");
    }

    [Fact]
    public void Reject_Should_Throw_When_ReasonIsEmpty()
    {
        var actorId = Guid.NewGuid();
        var ectn = CreateDraftEctn(actorId: actorId);
        ectn.Submit(actorId);

        Action act = () => ectn.Reject("", actorId);
        act.Should().Throw<ArgumentException>().WithParameterName("reason");
    }

    // ─── Cancel ──────────────────────────────────────────────────────────────────

    [Fact]
    public void Cancel_Should_SetStatusToCancelled_From_Draft()
    {
        var actorId = Guid.NewGuid();
        var ectn = CreateDraftEctn(actorId: actorId);

        ectn.Cancel("Applicant withdrew.", actorId);

        ectn.Status.Should().Be(EctnStatus.Cancelled);
        ectn.CancellationReason.Should().Be("Applicant withdrew.");
        ectn.CancelledAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    [Fact]
    public void Cancel_Should_SetStatusToCancelled_From_Issued()
    {
        var actorId = Guid.NewGuid();
        var ectn = CreateDraftEctn(actorId: actorId);
        ectn.Submit(actorId);
        ectn.Approve(actorId);
        ectn.Issue("ECTN-2026-000001", actorId);

        ectn.Cancel("Duplicate submission.", actorId);

        ectn.Status.Should().Be(EctnStatus.Cancelled);
    }

    [Fact]
    public void Cancel_Should_Throw_When_AlreadyCancelled()
    {
        var actorId = Guid.NewGuid();
        var ectn = CreateDraftEctn(actorId: actorId);
        ectn.Cancel("First cancellation.", actorId);

        Action act = () => ectn.Cancel("Second cancellation.", actorId);
        act.Should().Throw<InvalidOperationException>()
           .WithMessage("ECTN is already cancelled or expired.");
    }

    [Fact]
    public void Cancel_Should_Throw_When_ReasonIsEmpty()
    {
        var ectn = CreateDraftEctn();

        Action act = () => ectn.Cancel("", Guid.NewGuid());
        act.Should().Throw<ArgumentException>().WithParameterName("reason");
    }

    // ─── Full Lifecycle ──────────────────────────────────────────────────────────

    [Fact]
    public void FullLifecycle_Draft_Submit_Review_Approve_Issue_Should_HaveSixHistoryEntries()
    {
        var actorId = Guid.NewGuid();
        var ectn = CreateDraftEctn(actorId: actorId);

        ectn.Submit(actorId);
        ectn.StartReview(actorId);
        ectn.Approve(actorId);
        ectn.Issue("ECTN-2026-FULL-001", actorId);

        ectn.StatusHistories.Should().HaveCount(5); // Draft + Submitted + UnderReview + Approved + Issued
        ectn.Status.Should().Be(EctnStatus.Issued);
        ectn.EctnNumber.Should().Be("ECTN-2026-FULL-001");
    }

    [Fact]
    public void EctnNumber_Should_BeAssignable_WithAssignEctnNumber_Method()
    {
        var ectn = CreateDraftEctn();

        ectn.AssignEctnNumber("ECTN-2026-ASSIGN-001");

        ectn.EctnNumber.Should().Be("ECTN-2026-ASSIGN-001");
    }

    [Fact]
    public void AssignEctnNumber_Should_Throw_When_NumberIsEmpty()
    {
        var ectn = CreateDraftEctn();

        Action act = () => ectn.AssignEctnNumber("   ");
        act.Should().Throw<ArgumentException>().WithParameterName("ectnNumber");
    }
}
