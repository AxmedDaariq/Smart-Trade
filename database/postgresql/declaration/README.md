# Smart Platform Declaration Database

This directory contains the database structures, configurations, and definitions specific to the **Cargo & Customs Declaration Microservice** (`smart_declaration`).

## Governance
- **Owner:** Declaration Microservice
- **Direct Access:** Strictly forbidden from external services (e.g. Customs Assessment, Fleet, Payment, ECTN). All interactions must be through approved REST API endpoints or asynchronous Events.

## Schema Highlights
- Relies on `SmartDbContext` for centralized UTC timestamp tracking (`CreatedAtUtc`, `UpdatedAtUtc`) and auditing.
- Robust usage of PostgreSQL `NUMERIC(19,4)` for precise weight and monetary calculations.
- PostgreSQL `JSONB` applied exclusively to telemetry/extensible logging (`DetailsJson`, `ChangeSummaryJson`, `AuditEntry`).

## Versioning & Amendment Policies
- The `CustomsDeclaration` contains the mutable metadata of the transaction (Status, Dates).
- The `DeclarationVersion` acts as the root of the substantive content of the declaration. Once a version is Submitted/Accepted, it is treated as an immutable legal record. 
- Revisions are enforced via `DeclarationAmendment`, which generates a new `DeclarationVersion` rather than overwriting historical evidence.

## Deletion Policy
- `DeleteBehavior.Cascade` is used internally within the core version aggregate (e.g. Version -> Cargo -> Item).
- `DeleteBehavior.Restrict` is strictly enforced from `CustomsDeclaration` to prevent orphaned versions, decisions, submissions, and status histories.
