# ENTERPRISE DATABASE SECURITY BASELINE

## 1. Authentication & Vault Integration
All database credentials (`smart_identity_app`, `smart_payment_app`, etc.) are provisioned dynamically via **HashiCorp Vault**. 
- Secrets are never hardcoded in `appsettings.json` or committed to Git.
- PostgreSQL forces `scram-sha-256` password encryption.

## 2. TLS Encryption
All database connections (PostgreSQL, Redis) strictly require TLS. `sslmode=require` is enforced on all EF Core connection strings. Connections over plaintext are dropped at the Kubernetes Network layer.

## 3. Role Segregation (Least Privilege)
Applications operate using a restricted runtime role (`smart_app_runtime_role`).
- Runtime roles possess `SELECT, INSERT, UPDATE, DELETE` only.
- Runtime roles **cannot** run DDL (`CREATE`, `DROP`, `ALTER`).
- Migrations run through isolated CI/CD pipelines using `smart_migration_role` which does not process user traffic.

## 4. Tenant Isolation (RLS vs EF Filters)
- **EF Core Global Query Filters**: Enforced automatically on all `DbContext` configurations ensuring `WHERE TenantId = @tenantId`. The `TenantId` is securely resolved from the authenticated JWT token, NOT the request payload.
- **Row Level Security (RLS)**: Staged for extremely sensitive bounded contexts (`smart_payment`, `smart_customs`).

## 5. Network Isolation
Kubernetes `NetworkPolicies` ensure that `smart_payment` can only be queried by the `payment-api` pod. 
No external ingress is allowed to port 5432 or 6379. Direct database console access requires connecting to the private VPC via VPN.

## 6. Audit Logging
`pgaudit` is configured on the primary cluster to record all structural DDL changes and permission escalation attempts. Routine SELECTs are ignored to preserve IOPS.
