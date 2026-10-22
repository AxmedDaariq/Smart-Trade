using System;
using Customs.Domain.Entities;
using Customs.Domain.Enums;
using Customs.Domain.Policies;
using FluentAssertions;
using Xunit;

namespace Customs.UnitTests.Services;

/// <summary>
/// Unit tests for <see cref="CustomsStateTransitionPolicy"/>.
/// </summary>
public sealed class CustomsStateTransitionPolicyTests
{
    private readonly CustomsStateTransitionPolicy _policy = new();

    [Theory]
    [InlineData(CustomsStatus.Received, CustomsStatus.ValidationPending)]
    [InlineData(CustomsStatus.ValidationPending, CustomsStatus.RiskAssessment)]
    [InlineData(CustomsStatus.ValidationPending, CustomsStatus.Rejected)]
    [InlineData(CustomsStatus.RiskAssessment, CustomsStatus.Assessment)]
    [InlineData(CustomsStatus.RiskAssessment, CustomsStatus.InspectionPending)]
    [InlineData(CustomsStatus.RiskAssessment, CustomsStatus.NoInspection)]
    [InlineData(CustomsStatus.InspectionPending, CustomsStatus.Assessment)]
    [InlineData(CustomsStatus.NoInspection, CustomsStatus.Assessment)]
    [InlineData(CustomsStatus.Assessment, CustomsStatus.DutyAssessed)]
    [InlineData(CustomsStatus.DutyAssessed, CustomsStatus.PaymentPending)]
    [InlineData(CustomsStatus.DutyAssessed, CustomsStatus.ClearanceReview)]
    [InlineData(CustomsStatus.PaymentPending, CustomsStatus.PaymentConfirmed)]
    [InlineData(CustomsStatus.PaymentConfirmed, CustomsStatus.ClearanceReview)]
    [InlineData(CustomsStatus.ClearanceReview, CustomsStatus.Cleared)]
    [InlineData(CustomsStatus.ClearanceReview, CustomsStatus.Rejected)]
    [InlineData(CustomsStatus.Cleared, CustomsStatus.ReleaseAuthorized)]
    [InlineData(CustomsStatus.ReleaseAuthorized, CustomsStatus.Released)]
    [InlineData(CustomsStatus.Assessment, CustomsStatus.Held)]
    [InlineData(CustomsStatus.PaymentPending, CustomsStatus.Held)]
    [InlineData(CustomsStatus.ClearanceReview, CustomsStatus.Held)]
    public void CanTransition_ValidTransitions_ReturnsTrue(CustomsStatus from, CustomsStatus to)
    {
        // Act
        var result = _policy.CanTransition(from, to);

        // Assert
        result.Should().BeTrue();
    }

    [Theory]
    [InlineData(CustomsStatus.Received, CustomsStatus.Released)]
    [InlineData(CustomsStatus.Received, CustomsStatus.Cleared)]
    [InlineData(CustomsStatus.Cleared, CustomsStatus.Received)]
    [InlineData(CustomsStatus.Rejected, CustomsStatus.Assessment)]
    [InlineData(CustomsStatus.Released, CustomsStatus.ValidationPending)]
    public void CanTransition_InvalidTransitions_ReturnsFalse(CustomsStatus from, CustomsStatus to)
    {
        // Act
        var result = _policy.CanTransition(from, to);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void EnforceTransitionRules_ValidTransition_DoesNotThrow()
    {
        // Arrange
        var customsCase = new CustomsCase { Status = "Received" };

        // Act
        var act = () => _policy.EnforceTransitionRules(customsCase, CustomsStatus.ValidationPending);

        // Assert
        act.Should().NotThrow();
    }

    [Fact]
    public void EnforceTransitionRules_InvalidTransition_ThrowsInvalidOperationException()
    {
        // Arrange
        var customsCase = new CustomsCase { Status = "Received" };

        // Act
        var act = () => _policy.EnforceTransitionRules(customsCase, CustomsStatus.Released);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Invalid state transition*");
    }

    [Fact]
    public void EnforceTransitionRules_UnrecognizedCurrentStatus_ThrowsInvalidOperationException()
    {
        // Arrange
        var customsCase = new CustomsCase { Status = "UnknownStatusXYZ" };

        // Act
        var act = () => _policy.EnforceTransitionRules(customsCase, CustomsStatus.ValidationPending);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*unrecognized current status*");
    }
}
