# Enterprise Audit PostgreSQL Documentation

This directory encapsulates schema documentation, ERDs, and migration standards for the `smart_audit` database.

## Critical Ownership Principle
The Audit Service is the centralized platform audit and security-traceability service. **IT DOES NOT OWN THE BUSINESS DOMAIN.**
There are no User, Role, Declaration, Vehicle, or Payment core records here. The audit ledger exclusively holds serialized snapshots and immutable reference states (`ActorReference`, `ResourceReference`).

## Immutability
All core tables (`AuditEvent`, `AuditChange`, `AuditSecurityEvent`) are strictly append-only. 
The application MUST NOT be granted `UPDATE` or `DELETE` roles over these structures at runtime. Revisions to audits are captured via CORRECTION event insertions.

## Hash Chain
Tamper evidence is calculated continuously. Records insert a SHA-256 `IntegrityHash` derived from canonical event states and chained using `PreviousIntegrityHash`. `AuditIntegrityRecord` chunks these bounds for background validation.

## Execution
Run `dotnet ef migrations add InitialAuditSchema` manually from a host containing the .NET 10 SDK.
