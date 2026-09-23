using Customs.Domain.Entities;
using Customs.Domain.Enums;
using Customs.Domain.Policies;
using FluentAssertions;
using Xunit;

namespace Customs.UnitTests.Domain;

public class CustomsStateTransitionPolicyTests
{
    private readonly CustomsStateTransitionPolicy _policy = new();

    // ── CanTransition: valid paths ────────────────────────────────────────────

    [Theory]
    [InlineData(CustomsStatus.Received,          CustomsStatus.ValidationPending)]
    [InlineData(CustomsStatus.ValidationPending, CustomsStatus.RiskAssessment)]
    [InlineData(CustomsStatus.ValidationPending, CustomsStatus.Rejected)]
    [InlineData(CustomsStatus.RiskAssessment,    CustomsStatus.Assessment)]
    [InlineData(CustomsStatus.RiskAssessment,    CustomsStatus.InspectionPending)]
    [InlineData(CustomsStatus.RiskAssessment,    CustomsStatus.NoInspection)]
    [InlineData(CustomsStatus.InspectionPending, CustomsStatus.Assessment)]
    [InlineData(CustomsStatus.NoInspection,      CustomsStatus.Assessment)]
    [InlineData(CustomsStatus.Assessment,        CustomsStatus.DutyAssessed)]
    [InlineData(CustomsStatus.DutyAssessed,      CustomsStatus.PaymentPending)]
    [InlineData(CustomsStatus.DutyAssessed,      CustomsStatus.ClearanceReview)]
    [InlineData(CustomsStatus.PaymentPending,    CustomsStatus.PaymentConfirmed)]
    [InlineData(CustomsStatus.PaymentConfirmed,  CustomsStatus.ClearanceReview)]
    [InlineData(CustomsStatus.ClearanceReview,   CustomsStatus.Cleared)]
    [InlineData(CustomsStatus.ClearanceReview,   CustomsStatus.Rejected)]
    [InlineData(CustomsStatus.Cleared,           CustomsStatus.ReleaseAuthorized)]
    [InlineData(CustomsStatus.ReleaseAuthorized, CustomsStatus.Released)]
    [InlineData(CustomsStatus.Assessment,        CustomsStatus.Held)]
    [InlineData(CustomsStatus.PaymentPending,    CustomsStatus.Held)]
    [InlineData(CustomsStatus.ClearanceReview,   CustomsStatus.Held)]
    public void CanTransition_ValidTransition_ReturnsTrue(CustomsStatus from, CustomsStatus to)
    {
        _policy.CanTransition(from, to).Should().BeTrue(
            because: $"transition {from} -> {to} is a valid customs workflow step");
    }

    // ── CanTransition: invalid paths ─────────────────────────────────────────

    [Theory]
    [InlineData(CustomsStatus.Received,      CustomsStatus.Released)]
    [InlineData(CustomsStatus.Released,      CustomsStatus.Received)]
    [InlineData(CustomsStatus.Released,      CustomsStatus.Assessment)]
    [InlineData(CustomsStatus.PaymentPending, CustomsStatus.Received)]
    [InlineData(CustomsStatus.Held,          CustomsStatus.Released)]
    public void CanTransition_InvalidTransition_ReturnsFalse(CustomsStatus from, CustomsStatus to)
    {
        _policy.CanTransition(from, to).Should().BeFalse(
            because: $"transition {from} -> {to} is not an allowed customs workflow step");
    }

    // ── EnforceTransitionRules: happy path ───────────────────────────────────

    [Fact]
    public void EnforceTransitionRules_ValidTransition_DoesNotThrow()
    {
        var customsCase = BuildCase("Received");

        var act = () => _policy.EnforceTransitionRules(customsCase, CustomsStatus.ValidationPending);

        act.Should().NotThrow();
    }

    // ── EnforceTransitionRules: invalid transition throws ────────────────────

    [Fact]
    public void EnforceTransitionRules_InvalidTransition_Throws()
    {
        var customsCase = BuildCase("Received");

        var act = () => _policy.EnforceTransitionRules(customsCase, CustomsStatus.Released);

        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*Invalid state transition*");
    }

    // ── EnforceTransitionRules: unrecognised status string throws ────────────

    [Fact]
    public void EnforceTransitionRules_UnknownStatusString_Throws()
    {
        var customsCase = BuildCase("INVALID_STATUS_XYZ");

        var act = () => _policy.EnforceTransitionRules(customsCase, CustomsStatus.ValidationPending);

        act.Should().Throw<InvalidOperationException>()
           .WithMessage("*Invalid or unrecognized*");
    }

    // ── Full workflow path: Received -> Released ─────────────────────────────

    [Fact]
    public void FullWorkflow_ReceivedToReleased_AllTransitionsValid()
    {
        var workflow = new[]
        {
            (CustomsStatus.Received,          CustomsStatus.ValidationPending),
            (CustomsStatus.ValidationPending, CustomsStatus.RiskAssessment),
            (CustomsStatus.RiskAssessment,    CustomsStatus.Assessment),
            (CustomsStatus.Assessment,        CustomsStatus.DutyAssessed),
            (CustomsStatus.DutyAssessed,      CustomsStatus.PaymentPending),
            (CustomsStatus.PaymentPending,    CustomsStatus.PaymentConfirmed),
            (CustomsStatus.PaymentConfirmed,  CustomsStatus.ClearanceReview),
            (CustomsStatus.ClearanceReview,   CustomsStatus.Cleared),
            (CustomsStatus.Cleared,           CustomsStatus.ReleaseAuthorized),
            (CustomsStatus.ReleaseAuthorized, CustomsStatus.Released),
        };

        foreach (var (from, to) in workflow)
        {
            _policy.CanTransition(from, to).Should().BeTrue(
                because: $"{from} -> {to} must be valid in the full release workflow");
        }
    }

    // ── Exempt-payment path: DutyAssessed -> ClearanceReview ────────────────

    [Fact]
    public void ExemptPayment_DutyAssessedToClearanceReview_IsValid()
    {
        _policy.CanTransition(CustomsStatus.DutyAssessed, CustomsStatus.ClearanceReview)
               .Should().BeTrue(because: "exempt goods skip payment and go straight to clearance review");
    }

    // ── Hold transitions ─────────────────────────────────────────────────────

    [Theory]
    [InlineData(CustomsStatus.Assessment)]
    [InlineData(CustomsStatus.PaymentPending)]
    [InlineData(CustomsStatus.ClearanceReview)]
    public void Hold_FromActiveProcessingState_IsValid(CustomsStatus from)
    {
        _policy.CanTransition(from, CustomsStatus.Held)
               .Should().BeTrue(because: $"holds must be placeable from {from}");
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static CustomsCase BuildCase(string status) =>
        new()
        {
            Status = status,
            TenantId = Guid.NewGuid(),
            CaseNumber = "CASE-001"
        };
}
