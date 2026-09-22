using System;

namespace Payment.Domain.Enums;

public enum PaymentIntentStatus
{
    Created,
    Pending,
    PartiallyPaid,
    Paid,
    Failed,
    Cancelled,
    Expired,
    RefundPending,
    Refunded
}

public enum PaymentTransactionStatus
{
    Initiated,
    Pending,
    Authorized,
    Succeeded,
    Failed,
    Cancelled,
    Expired,
    Refunded,
    PartiallyRefunded
}

public enum RefundStatus
{
    Requested,
    UnderReview,
    Approved,
    Processing,
    Succeeded,
    Failed,
    Rejected
}

public enum ReconciliationStatus
{
    Matched,
    AmountMismatch,
    CurrencyMismatch,
    MissingInternal,
    MissingExternal,
    DuplicateExternal,
    DuplicateInternal,
    Unknown,
    PendingReview
}
