# Audit Redis Documentation

Redis in the Audit Service provides ephemeral caching to manage idempotency, block abusive access, and cache integrity checkpoints.

## Caching Strategies

### Idempotency
- `audit:idempotency:{tenantId}:{operation}:{key}`
**Purpose**: Rapidly reject duplicate event submissions from mass integrations before inserting into PostgreSQL.

### Export / Search Rate Limiting
- `audit:rate-limit:{tenantId}:{userId}:{operation}`
**Purpose**: Limit outgoing audit mass-exports or heavy search queries. Prevents abusive actors from locking up PostgreSQL via heavy analytic scans.

### Integrity Verification Locks
- `audit:verification:{partition}`
**Purpose**: Track which worker is currently executing the cryptographic integrity check over a specific date partition to prevent redundant computational load.
