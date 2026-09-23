using System;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Ectn.Domain.Entities;
using Ectn.Domain.Enums;
using Ectn.Domain.Services;
using EctnEntity = Ectn.Domain.Entities.Ectn;

namespace Ectn.UnitTests.Domain;

public class EctnStateTransitionServiceTests
{
    private readonly EctnStateTransitionService _service = new();

    private static EctnEntity BuildEctnInStatus(EctnStatus status, bool withParty = true)
    {
        var actorId = Guid.NewGuid();
        var ectn = new EctnEntity(Guid.NewGuid(), "APP-TST-001", EctnType.Standard, actorId);

        if (withParty)
        {
            ectn.AddParty(new EctnParty { Id = Guid.NewGuid(), LegalName = "Test Shipper" });
        }

        switch (status)
        {
            case EctnStatus.Draft:
                break;
            case EctnStatus.Submitted:
                ectn.Submit(actorId);
                break;
            case EctnStatus.UnderReview:
                ectn.Submit(actorId);
                ectn.StartReview(actorId);
                break;
            case EctnStatus.Approved:
                ectn.Submit(actorId);
                ectn.Approve(actorId);
                break;
            case EctnStatus.Issued:
                ectn.Submit(actorId);
                ectn.Approve(actorId);
                ectn.Issue("ECTN-SVC-001", actorId);
                break;
            case EctnStatus.Rejected:
                ectn.Submit(actorId);
                ectn.Reject("Test rejection.", actorId);
                break;
            case EctnStatus.Cancelled:
                ectn.Cancel("Test cancellation.", actorId);
                break;
        }

        return ectn;
    }

    // ─── Submit Transition ────────────────────────────────────────────────────

    [Fact]
    public async Task ValidateTransition_Should_Pass_When_SubmittingDraft_WithParties()
    {
        var ectn = BuildEctnInStatus(EctnStatus.Draft, withParty: true);
        var act = async () => await _service.ValidateTransitionAsync(ectn, EctnStatus.Submitted, Guid.NewGuid());
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ValidateTransition_Should_Throw_When_SubmittingDraft_WithoutParties()
    {
        var ectn = BuildEctnInStatus(EctnStatus.Draft, withParty: false);
        var act = async () => await _service.ValidateTransitionAsync(ectn, EctnStatus.Submitted, Guid.NewGuid());
        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("ECTN must have at least one party defined before submission.");
    }

    [Fact]
    public async Task ValidateTransition_Should_Throw_When_SubmittingNonDraft()
    {
        var ectn = BuildEctnInStatus(EctnStatus.Submitted);
        var act = async () => await _service.ValidateTransitionAsync(ectn, EctnStatus.Submitted, Guid.NewGuid());
        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("ECTN must be in Draft state to be submitted.");
    }

    // ─── Approve Transition ───────────────────────────────────────────────────

    [Fact]
    public async Task ValidateTransition_Should_Pass_When_ApprovingSubmitted()
    {
        var ectn = BuildEctnInStatus(EctnStatus.Submitted);
        var act = async () => await _service.ValidateTransitionAsync(ectn, EctnStatus.Approved, Guid.NewGuid());
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ValidateTransition_Should_Pass_When_ApprovingUnderReview()
    {
        var ectn = BuildEctnInStatus(EctnStatus.UnderReview);
        var act = async () => await _service.ValidateTransitionAsync(ectn, EctnStatus.Approved, Guid.NewGuid());
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ValidateTransition_Should_Throw_When_ApprovingDraft()
    {
        var ectn = BuildEctnInStatus(EctnStatus.Draft);
        var act = async () => await _service.ValidateTransitionAsync(ectn, EctnStatus.Approved, Guid.NewGuid());
        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("ECTN must be Submitted or Under Review to be approved.");
    }

    // ─── Issue Transition ─────────────────────────────────────────────────────

    [Fact]
    public async Task ValidateTransition_Should_Pass_When_IssuingApproved_WithEctnNumber()
    {
        var ectn = BuildEctnInStatus(EctnStatus.Approved);
        ectn.AssignEctnNumber("ECTN-2026-SERVICE-001");
        var act = async () => await _service.ValidateTransitionAsync(ectn, EctnStatus.Issued, Guid.NewGuid());
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ValidateTransition_Should_Throw_When_IssuingApproved_WithoutEctnNumber()
    {
        var ectn = BuildEctnInStatus(EctnStatus.Approved);
        // No EctnNumber assigned
        var act = async () => await _service.ValidateTransitionAsync(ectn, EctnStatus.Issued, Guid.NewGuid());
        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("ECTN must have an official number assigned before issuance.");
    }

    [Fact]
    public async Task ValidateTransition_Should_Throw_When_IssuingDraft()
    {
        var ectn = BuildEctnInStatus(EctnStatus.Draft);
        var act = async () => await _service.ValidateTransitionAsync(ectn, EctnStatus.Issued, Guid.NewGuid());
        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("ECTN must be Approved before it can be Issued.");
    }

    // ─── Reject Transition ────────────────────────────────────────────────────

    [Fact]
    public async Task ValidateTransition_Should_Pass_When_RejectingSubmitted()
    {
        var ectn = BuildEctnInStatus(EctnStatus.Submitted);
        var act = async () => await _service.ValidateTransitionAsync(ectn, EctnStatus.Rejected, Guid.NewGuid());
        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task ValidateTransition_Should_Throw_When_RejectingIssuedEctn()
    {
        var ectn = BuildEctnInStatus(EctnStatus.Issued);
        var act = async () => await _service.ValidateTransitionAsync(ectn, EctnStatus.Rejected, Guid.NewGuid());
        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("Cannot reject an ECTN that is already Issued, Cancelled, or Expired.");
    }

    [Fact]
    public async Task ValidateTransition_Should_Throw_When_RejectingCancelledEctn()
    {
        var ectn = BuildEctnInStatus(EctnStatus.Cancelled);
        var act = async () => await _service.ValidateTransitionAsync(ectn, EctnStatus.Rejected, Guid.NewGuid());
        await act.Should().ThrowAsync<InvalidOperationException>()
                 .WithMessage("Cannot reject an ECTN that is already Issued, Cancelled, or Expired.");
    }
}
