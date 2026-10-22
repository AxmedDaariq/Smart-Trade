using System;
using Declaration.Domain.Entities;
using Declaration.Domain.Enums;
using Declaration.Domain.Policies;

namespace Declaration.Tests.Domain;

public class DeclarationStateTransitionPolicyTests
{
    private readonly DeclarationStateTransitionPolicy _policy = new();

    [Theory]
    [InlineData(DeclarationStatus.Draft, DeclarationStatus.Submitted)]
    [InlineData(DeclarationStatus.Draft, DeclarationStatus.Cancelled)]
    [InlineData(DeclarationStatus.Submitted, DeclarationStatus.Validation)]
    [InlineData(DeclarationStatus.Submitted, DeclarationStatus.UnderReview)]
    [InlineData(DeclarationStatus.UnderReview, DeclarationStatus.Accepted)]
    [InlineData(DeclarationStatus.Accepted, DeclarationStatus.AssessmentPending)]
    [InlineData(DeclarationStatus.Accepted, DeclarationStatus.AmendmentRequested)]
    [InlineData(DeclarationStatus.AssessmentPending, DeclarationStatus.Cleared)]
    [InlineData(DeclarationStatus.Cleared, DeclarationStatus.Released)]
    public void CanTransition_AllowsConfiguredTransitions(DeclarationStatus from, DeclarationStatus to)
    {
        Assert.True(_policy.CanTransition(from, to));
    }

    [Theory]
    [InlineData(DeclarationStatus.Draft, DeclarationStatus.Accepted)]
    [InlineData(DeclarationStatus.Draft, DeclarationStatus.Released)]
    [InlineData(DeclarationStatus.Submitted, DeclarationStatus.Released)]
    [InlineData(DeclarationStatus.Cleared, DeclarationStatus.Draft)]
    [InlineData(DeclarationStatus.Released, DeclarationStatus.Draft)]
    public void CanTransition_RejectsIllegalTransitions(DeclarationStatus from, DeclarationStatus to)
    {
        Assert.False(_policy.CanTransition(from, to));
    }

    [Fact]
    public void EnforceTransitionRules_DoesNotThrow_OnValidTransition()
    {
        var declaration = new CustomsDeclaration { Status = DeclarationStatus.Draft.ToString() };

        var exception = Record.Exception(() =>
            _policy.EnforceTransitionRules(declaration, DeclarationStatus.Submitted));

        Assert.Null(exception);
    }

    [Fact]
    public void EnforceTransitionRules_Throws_OnInvalidTransition()
    {
        var declaration = new CustomsDeclaration { Status = DeclarationStatus.Draft.ToString() };

        Assert.Throws<InvalidOperationException>(() =>
            _policy.EnforceTransitionRules(declaration, DeclarationStatus.Released));
    }

    [Fact]
    public void EnforceTransitionRules_Throws_OnUnrecognizedCurrentStatus()
    {
        var declaration = new CustomsDeclaration { Status = "NotAStatus" };

        Assert.Throws<InvalidOperationException>(() =>
            _policy.EnforceTransitionRules(declaration, DeclarationStatus.Submitted));
    }
}
