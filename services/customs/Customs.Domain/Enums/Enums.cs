namespace Customs.Domain.Enums;

public enum CustomsStatus
{
    Received,
    ValidationPending,
    RiskAssessment,
    Assessment,
    InspectionPending,
    NoInspection,
    DutyAssessed,
    PaymentPending,
    PaymentConfirmed,
    ClearanceReview,
    Cleared,
    ReleaseAuthorized,
    Released,
    Held,
    Rejected
}

public enum InspectionStatus
{
    Pending,
    InProgress,
    Completed,
    Cancelled
}

public enum PaymentStatus
{
    Pending,
    Confirmed,
    Failed
}

public enum ClearanceStatus
{
    Pending,
    Approved,
    Rejected
}

public enum RiskLevel
{
    Low,
    Medium,
    High,
    Critical
}

public enum RateType
{
    AdValorem,
    Specific,
    Compound,
    Mixed
}

public enum ValuationMethod
{
    TransactionValue,
    IdenticalGoods,
    SimilarGoods,
    DeductiveValue,
    ComputedValue,
    FallbackMethod
}
