# Smart Platform ECTN Database

This directory contains the database structures, configurations, and definitions specific to the **ECTN Microservice** (`smart_ectn`).

## Governance
- **Owner:** ECTN Microservice
- **Direct Access:** Strictly forbidden from external services (e.g. Customs, Fleet, Payment). All interactions must be through approved REST API endpoints or asynchronous Events.

## Schema Highlights
- Relies on `SmartDbContext` for centralized UTC timestamp tracking (`CreatedAtUtc`, `UpdatedAtUtc`) and auditing.
- Robust usage of PostgreSQL `NUMERIC` for precise freight weight and monetary calculations.
- PostgreSQL `JSONB` applied exclusively to telemetry/extensible logging (`ValidationDetailsJson`, `AuditEntry`).

## Deletion Policy
- `DeleteBehavior.Cascade` is used internally within the core aggregate (e.g. Shipment -> Cargo).
- `DeleteBehavior.Restrict` is strictly enforced where external domain references are linked, or to prevent orphaned `EctnApplication` children (Certificates, Audits, StatusHistory).
