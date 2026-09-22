# ECTN Database Architecture

## ERD
The ECTN (Electronic Cargo Tracking Note) schema strictly enforces the logistics corridor tracking standards.

- **EctnApplication** (1:M) **EctnCertificate**, **EctnShipment**, **EctnPartyRole**, **EctnValidation**, **EctnReview**, **EctnDecision**, **EctnAmendment**, **EctnStatusHistory**, **EctnDocument**
- **EctnShipment** (1:M) **EctnCargo**, **EctnContainer**, **EctnBillOfLading**, **EctnVesselReference**, **EctnVoyageReference**
- **EctnCargo** (1:M) **EctnCargoItem**
- **EctnDocument** (1:M) **EctnDocumentVersion**, **EctnDocumentVerification**
- **EctnCertificate** (1:1) **EctnQrCode**

## Tenant Isolation
Multi-tenancy is native:
- `TenantId` exists on `EctnApplication`, `EctnCertificate`, `EctnReference`, `EctnIntegrationRecord`, `IdempotencyRecord`, `EctnAuditEntry`.
- Unique constraints strictly scope applications: `UNIQUE (TenantId, ApplicationNumber)`.

## Data Consistency & Auditing
- **Optimistic Concurrency:** Enforced via EF Core `Version` column (`xmin` based) on `EctnApplication`, `EctnAmendment`, `EctnDocument`.
- **Audit Logs:** `EctnAuditEntry` stores immutable changes utilizing PostgreSQL `JSONB` for `BeforeJson` and `AfterJson`.
- **Financial/Quantitative Types:** `TotalGrossWeight`, `TotalNetWeight`, `TotalVolume`, `DeclaredValue`, `FreightValue`, `InsuranceValue`, and `UnitPrice` are strongly mapped to PostgreSQL `numeric(18,2)` or `numeric(18,4)` to prevent floating-point precision loss.

## Document Storage
- Documents metadata is saved to PostgreSQL (`EctnDocument`).
- Actual binary data is assumed to be stored in an S3/MinIO compatible object store (`StorageObjectKey`).
- We enforce document immutability via `EctnDocumentVersion`.

## Integration
- Safe cross-service messaging uses `OutboxMessage` (Outbox pattern).
- Incoming events rely on `InboxMessage` and `IdempotencyRecord` to guarantee exactly-once processing safely.
