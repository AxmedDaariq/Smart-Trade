# Identity Database Architecture

## ERD
The Identity schema contains the following relationships:
- **Tenant** (1:M) **Organization**, (1:M) **User**, (1:M) **Role**, (1:M) **SecurityEvent**, (1:M) **IdentityAuditEntry**
- **User** (1:1) **UserSecurityState**, (1:M) **Session**, (1:M) **UserRole**, (1:M) **PasswordHistory**, (1:M) **EmailVerificationToken**, (1:M) **PasswordResetToken**, (1:M) **MfaConfiguration**
- **Role** (1:M) **UserRole**, (1:M) **RolePermission**
- **Permission** (1:M) **RolePermission**
- **Session** (1:M) **RefreshToken**
- **MfaConfiguration** (1:M) **MfaRecoveryCode**

## Tenant Isolation
- Tenants are the highest level of isolation.
- `tenant_id` exists on all major transactional/state tables.
- Constraints (`Unique(tenant_id, normalized_username)`, `Unique(tenant_id, organization_code)`) enforce this at the database level.
- Multi-tenancy is primarily enforced at the application level; RLS is not yet active but the schema structure fully supports future enablement.

## Security Model
- Passwords, Recovery Codes, Email Tokens, and Reset Tokens are **ONLY** stored as cryptographic hashes.
- `MfaConfiguration.SecretEncrypted` is symmetrically encrypted at the application level before database insertion.
- `SecurityEvent` and `IdentityAuditEntry` record all crucial identity actions and utilize PostgreSQL `JSONB` for extensible metadata without breaking the core relational model.
- `UserSecurityState` leverages an explicit `Version` concurrency token (`xmin` based) for optimistic locking to prevent race conditions during brute force scenarios or rapid concurrent logins.

## Retention Strategy
- Active configurations are never hard-deleted; `Status` flags (`Archived`, `Disabled`, `Revoked`) are used instead.
- Identity events (Security Events, Audit Entries) are append-only.
- Expiration (`ExpiresAtUtc`) acts as the active filter for tokens.

## Redis Integration
- The database is authoritative. Redis is solely used to cache roles, permissions, and active session verifications for performance. Redis is explicitly **not** the source of truth.
