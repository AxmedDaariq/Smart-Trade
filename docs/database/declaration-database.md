# Declaration Database Architecture

## ERD
The Cargo & Customs Declaration schema strictly enforces the regulatory and logistical transaction boundaries.

- **CustomsDeclaration** (1:M) **DeclarationVersion**, **DeclarationValidation**, **DeclarationSubmission**, **DeclarationDecision**, **DeclarationAmendment**, **DeclarationStatusHistory**, **DeclarationExternalReference**, **DeclarationIntegrationRecord**, **DeclarationReference**
- **DeclarationVersion** (1:M) **DeclarationParty**, **DeclarationCargo**, **DeclarationContainer**, **DeclarationTransport**, **DeclarationRoute**, **DeclarationDocument**
- **DeclarationParty** (1:M) **DeclarationPartyRole**
- **DeclarationCargo** (1:M) **DeclarationCargoItem**
- **DeclarationDocument** (1:M) **DeclarationDocumentVersion**

## Tenant Isolation
Multi-tenancy is native:
- `TenantId` exists on `CustomsDeclaration`, `DeclarationReference`, `DeclarationIntegrationRecord`, `IdempotencyRecord`, `DeclarationAuditEntry`.
- Unique constraints strictly scope applications: `UNIQUE (TenantId, DeclarationNumber)`.

## Data Consistency & Auditing
- **Optimistic Concurrency:** Enforced via EF Core `Version` column (`xmin` based) on `CustomsDeclaration`, `DeclarationVersion`, `DeclarationAmendment`, `DeclarationDocument`.
- **Audit Logs:** `DeclarationAuditEntry` stores immutable changes utilizing PostgreSQL `JSONB` for `BeforeJson` and `AfterJson`.
- **Financial/Quantitative Types:** `TotalGrossWeight`, `TotalNetWeight`, `TotalVolume`, `DeclaredValue`, `UnitPrice`, and `TotalValue` are strongly mapped to PostgreSQL `numeric(19,4)` to prevent floating-point precision loss.

## Document Storage
- Documents metadata is saved to PostgreSQL (`DeclarationDocument`).
- Actual binary data is assumed to be stored in an S3/MinIO compatible object store (`StorageObjectKey`).
- Enforces document immutability via `DeclarationDocumentVersion`.

## Integration Boundaries
- Safe cross-service messaging uses `OutboxMessage`.
- Incoming events rely on `InboxMessage` and `IdempotencyRecord` to guarantee exactly-once processing safely.
- **ECTN/Customs/Fleet/AIS Separation:** The declaration schema deliberately does NOT duplicate master definitions from ECTN, Customs Assessments, Payment Ledgers, or Fleet master tracking. It stores snapshots or reference IDs (`VehicleReference`, `EctnReference`) required for the specific transaction in time.
