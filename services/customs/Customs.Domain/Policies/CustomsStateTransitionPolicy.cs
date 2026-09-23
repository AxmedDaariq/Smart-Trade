using System;
using Customs.Domain.Enums;
using Customs.Domain.Entities;

namespace Customs.Domain.Policies;

public interface ICustomsStateTransitionPolicy
{
    bool CanTransition(CustomsStatus currentStatus, CustomsStatus targetStatus);
    void EnforceTransitionRules(CustomsCase customsCase, CustomsStatus targetStatus);
}

public class CustomsStateTransitionPolicy : ICustomsStateTransitionPolicy
{
    public bool CanTransition(CustomsStatus currentStatus, CustomsStatus targetStatus)
    {
        return (currentStatus, targetStatus) switch
        {
            (CustomsStatus.Received, CustomsStatus.ValidationPending) => true,
            (CustomsStatus.ValidationPending, CustomsStatus.RiskAssessment) => true,
            (CustomsStatus.ValidationPending, CustomsStatus.Rejected) => true,
            (CustomsStatus.RiskAssessment, CustomsStatus.Assessment) => true,
            (CustomsStatus.RiskAssessment, CustomsStatus.InspectionPending) => true,
            (CustomsStatus.RiskAssessment, CustomsStatus.NoInspection) => true,
            (CustomsStatus.InspectionPending, CustomsStatus.Assessment) => true,
            (CustomsStatus.NoInspection, CustomsStatus.Assessment) => true,
            (CustomsStatus.Assessment, CustomsStatus.DutyAssessed) => true,
            (CustomsStatus.DutyAssessed, CustomsStatus.PaymentPending) => true,
            (CustomsStatus.DutyAssessed, CustomsStatus.ClearanceReview) => true, // if no payment required (e.g. exempt)
            (CustomsStatus.PaymentPending, CustomsStatus.PaymentConfirmed) => true,
            (CustomsStatus.PaymentConfirmed, CustomsStatus.ClearanceReview) => true,
            (CustomsStatus.ClearanceReview, CustomsStatus.Cleared) => true,
            (CustomsStatus.ClearanceReview, CustomsStatus.Rejected) => true,
            (CustomsStatus.Cleared, CustomsStatus.ReleaseAuthorized) => true,
            (CustomsStatus.ReleaseAuthorized, CustomsStatus.Released) => true,
            // Allow hold from any active processing state
            (CustomsStatus.Assessment, CustomsStatus.Held) => true,
            (CustomsStatus.PaymentPending, CustomsStatus.Held) => true,
            (CustomsStatus.ClearanceReview, CustomsStatus.Held) => true,
            _ => false
        };
    }

    public void EnforceTransitionRules(CustomsCase customsCase, CustomsStatus targetStatus)
    {
        if (!Enum.TryParse<CustomsStatus>(customsCase.Status, true, out var currentStatus))
        {
            throw new InvalidOperationException($"Invalid or unrecognized current status: '{customsCase.Status}'");
        }

        if (!CanTransition(currentStatus, targetStatus))
        {
            throw new InvalidOperationException($"Invalid state transition from {currentStatus} to {targetStatus}");
        }
    }
}
