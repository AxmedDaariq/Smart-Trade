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
            (CustomsStatus.DutyAssessed, CustomsStatus.ClearanceReview) => true, // if no payment required
            (CustomsStatus.PaymentPending, CustomsStatus.PaymentConfirmed) => true,
            (CustomsStatus.PaymentConfirmed, CustomsStatus.ClearanceReview) => true,
            (CustomsStatus.ClearanceReview, CustomsStatus.Cleared) => true,
            (CustomsStatus.ClearanceReview, CustomsStatus.Rejected) => true,
            (CustomsStatus.Cleared, CustomsStatus.ReleaseAuthorized) => true,
            (CustomsStatus.ReleaseAuthorized, CustomsStatus.Released) => true,
            _ => false
        };
    }

    public void EnforceTransitionRules(CustomsCase customsCase, CustomsStatus targetStatus)
    {
        if (!CanTransition(customsCase.Status, targetStatus))
        {
            throw new InvalidOperationException($"Invalid state transition from {customsCase.Status} to {targetStatus}");
        }
    }
}
