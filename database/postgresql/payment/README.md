# Payment Database Schema & Configurations

This directory manages the PostgreSQL schema definitions and migrations for the `smart_payment` database.

## Principles
- **No Siloed Data**: The payment system does not replicate the structural details of a Customs Declaration or an ECTN. It strictly receives the financial obligation via an event, generates an `Invoice`, and processes a `PaymentIntent`.
- **Idempotent Operations**: The use of `IdempotencyRecord` guarantees that a customer is never double-charged due to a retry timeout.
- **Strictly Immutable Ledger**: `LedgerEntry` is the ultimate source of truth for the financial state. It is an append-only log. Updates or deletions are forbidden by design. 
- **Secret Management**: API keys and Webhook secrets are never stored directly in the `PaymentGatewayConfiguration`. Instead, a reference (like an ARN or Vault Path) is stored in `ApiKeySecretRef`.

## Migration Structure
Entity Framework Migrations belong to `/src/services/Payment/Payment.Infrastructure`. Do not create raw `.sql` upgrade scripts in this folder outside of specialized tuning.

## Tenancy
Multi-tenant scoping ensures `TenantId` blocks cross-talk between different financial configurations using the platform.

## Backups
Follow the global `database/scripts/backup` standard for continuous WAL archiving and daily snapshotting to S3. Payment databases generally require the highest level of backup retention and security mirroring.
