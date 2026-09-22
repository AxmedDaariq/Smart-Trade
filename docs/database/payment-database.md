# Payment Database Architecture

## Overview
The `smart_payment` database is the authoritative source for all things related to billing, invoicing, payments, ledgers, and transactions. It intentionally avoids mirroring Declaration, Customs, or ECTN systems entirely to enforce a microservices boundary. Instead, it relies on integration events (like `CustomsAssessmentApproved`) to trigger invoice creation.

## ERD Core Groupings
- **Billing** 
  - **Invoice** -> **InvoiceLineItem**
  - **CustomerAccount**
  - **Receipt**
- **Payment & Transaction**
  - **PaymentIntent** -> **PaymentTransaction**
- **Ledger**
  - **LedgerEntry** (Strictly Immutable, Hash-linked)
- **Configuration**
  - **PaymentGatewayConfiguration** -> **PaymentMethodConfiguration**

## Financial Immutability & Precision
Because the Payment microservice is essentially the financial ledger of the platform, the following types were enforced:
- **`numeric(19,4)`**: Used natively for all currency amounts (e.g. `AmountRequested`, `TotalAmount`, `SubtotalAmount`, `TaxAmount`, `AmountPaid`, `AmountDue`). This prevents fractional floating-point corruption.
- **Immutable Ledger**: The `LedgerEntry` entity represents a double-entry accounting log. It is explicitly configured in Entity Framework Core to restrict updates and cascading deletes, enforcing strict append-only immutability. Furthermore, a `Hash` and `PreviousHash` link can be used to cryptographically chain entries, preventing tampering.

## Integration
Events like `InvoicePaid` and `PaymentFailed` use the `OutboxMessage` pattern to broadcast asynchronously to the Customs, ECTN, or Declaration services.
Idempotency keys are explicitly enforced via `IdempotencyRecord` to guarantee that network retries during payment capture never double-charge a customer account.
