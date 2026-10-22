using System;
using Ectn.Domain.Entities;
using Ectn.Domain.Enums;
using FluentAssertions;
using Xunit;

namespace Ectn.UnitTests.Domain;

public class EctnDomainTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithDraftStatus()
    {
        // Arrange
        var tenantId = Guid.NewGuid();
        var actorId = Guid.NewGuid();

        // Act
        var ectn = new Ectn.Domain.Entities.Ectn(tenantId, "REF-12345", EctnType.Standard, actorId);

        // Assert
        ectn.Id.Should().NotBeEmpty();
        ectn.TenantId.Should().Be(tenantId);
        ectn.ApplicationReference.Should().Be("REF-12345");
        ectn.Status.Should().Be(EctnStatus.Draft);
        ectn.StatusHistories.Should().HaveCount(1);
    }

    [Fact]
    public void Submit_ShouldTransitionStatusToSubmitted()
    {
        // Arrange
        var actorId = Guid.NewGuid();
        var ectn = new Ectn.Domain.Entities.Ectn(Guid.NewGuid(), "REF-123", EctnType.Standard, actorId);

        // Act
        ectn.Submit(actorId);

        // Assert
        ectn.Status.Should().Be(EctnStatus.Submitted);
        ectn.SubmittedAt.Should().NotBeNull();
        ectn.StatusHistories.Should().HaveCount(2);
    }

    [Fact]
    public void FullApprovalAndIssuanceFlow_ShouldSucceed()
    {
        // Arrange
        var actorId = Guid.NewGuid();
        var ectn = new Ectn.Domain.Entities.Ectn(Guid.NewGuid(), "REF-123", EctnType.Standard, actorId);

        // Act & Assert Transitions
        ectn.Submit(actorId);
        ectn.Status.Should().Be(EctnStatus.Submitted);

        ectn.StartReview(actorId);
        ectn.Status.Should().Be(EctnStatus.UnderReview);

        ectn.Approve(actorId);
        ectn.Status.Should().Be(EctnStatus.Approved);

        ectn.Issue("ECTN-DJ-2026-0099", actorId);
        ectn.Status.Should().Be(EctnStatus.Issued);
        ectn.EctnNumber.Should().Be("ECTN-DJ-2026-0099");
        ectn.IssuedAt.Should().NotBeNull();
    }

    [Fact]
    public void Reject_OnIssuedEctn_ShouldThrowInvalidOperationException()
    {
        // Arrange
        var actorId = Guid.NewGuid();
        var ectn = new Ectn.Domain.Entities.Ectn(Guid.NewGuid(), "REF-123", EctnType.Standard, actorId);
        ectn.Submit(actorId);
        ectn.Approve(actorId);
        ectn.Issue("ECTN-999", actorId);

        // Act
        Action act = () => ectn.Reject("Fraud detected", actorId);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Cannot reject an ECTN that is already Issued*");
    }
}
