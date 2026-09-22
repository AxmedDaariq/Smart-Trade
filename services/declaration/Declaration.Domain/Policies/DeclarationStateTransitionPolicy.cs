using Declaration.Domain.Entities;
using Declaration.Domain.Enums;
using System;

namespace Declaration.Domain.Policies;

public class DeclarationStateTransitionPolicy : IDeclarationStateTransitionPolicy
{
    public bool CanTransition(CustomsDeclaration declaration, DeclarationStatus targetStatus)
    {
        return (declaration.Status, targetStatus) switch
        {
            (DeclarationStatus.Draft, DeclarationStatus.Submitted) => true,
            (DeclarationStatus.Draft, DeclarationStatus.Cancelled) => true,
            (DeclarationStatus.Submitted, DeclarationStatus.Validation) => true,
            (DeclarationStatus.Submitted, DeclarationStatus.UnderReview) => true,
            (DeclarationStatus.Submitted, DeclarationStatus.Rejected) => true,
            (DeclarationStatus.Submitted, DeclarationStatus.Cancelled) => true,
            (DeclarationStatus.Validation, DeclarationStatus.UnderReview) => true,
            (DeclarationStatus.Validation, DeclarationStatus.Rejected) => true,
            (DeclarationStatus.UnderReview, DeclarationStatus.Accepted) => true,
            (DeclarationStatus.UnderReview, DeclarationStatus.Rejected) => true,
            (DeclarationStatus.Rejected, DeclarationStatus.Draft) => true, // Resubmission
            (DeclarationStatus.Accepted, DeclarationStatus.AssessmentPending) => true,
            (DeclarationStatus.Accepted, DeclarationStatus.AmendmentRequested) => true,
            (DeclarationStatus.AmendmentRequested, DeclarationStatus.UnderReview) => true,
            (DeclarationStatus.AssessmentPending, DeclarationStatus.Cleared) => true,
            (DeclarationStatus.Cleared, DeclarationStatus.Released) => true,
            _ => false
        };
    }

    public void EnforceTransitionRules(CustomsDeclaration declaration, DeclarationStatus targetStatus)
    {
        if (!CanTransition(declaration, targetStatus))
        {
            throw new InvalidOperationException($"Invalid state transition from {declaration.Status} to {targetStatus}");
        }
    }
}
