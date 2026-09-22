# Customs Database Schema & Configurations

This directory manages the PostgreSQL schema definitions and migrations for the `smart_customs` database.

## Principles
- **No Hardcoded Tax Rates**: The system relies entirely on `TariffScheduleVersion`, `TariffItem`, and `CustomsRateVersion` to calculate duties. No `.NET` or `SQL` code contains "if VAT == 10%". 
- **No Siloed Data**: Domestic taxes (VAT, GST) are not automatically applied unless explicitly inserted by government configuration into the `CustomsChargeType` mapping.
- **Idempotent Operations**: The use of `IdempotencyRecord` prevents a system error or timeout from mistakenly re-charging an importer or duplicating an assessment line.

## Migration Structure
Entity Framework Migrations belong to `/src/services/Customs/Customs.Infrastructure`. Do not create raw `.sql` upgrade scripts in this folder outside of specialized tuning (like partition tuning for high-volume logs).

## Tenancy
Multi-tenant scoping ensures `TenantId` blocks cross-talk between different port systems or government instances using the platform.

## Backups
Follow the global `database/scripts/backup` standard for continuous WAL archiving and daily snapshotting to S3.
