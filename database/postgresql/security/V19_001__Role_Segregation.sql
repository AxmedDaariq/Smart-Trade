-- =======================================================
-- DB PHASE 19 - ENTERPRISE DATABASE SECURITY HARDENING
-- ROLE SEGREGATION SCRIPT
-- =======================================================

-- IMPORTANT: This script should be run by the database cluster SUPERUSER.

-- 1. Enforce SCRAM-SHA-256 for all new passwords
SET password_encryption = 'scram-sha-256';

-- 2. Revoke default public schema grants to prevent lateral escalation
REVOKE CREATE ON SCHEMA public FROM PUBLIC;
REVOKE ALL ON DATABASE smart_identity FROM PUBLIC;
REVOKE ALL ON DATABASE smart_payment FROM PUBLIC;
-- (Repeat for all DB01-DB17)

-- 3. Create explicit Migration Roles (Used by CI/CD EF Core bundle ONLY)
CREATE ROLE smart_migration_role NOLOGIN;
-- Migration role gets DDL access
GRANT CREATE ON SCHEMA public TO smart_migration_role;
-- Vault will assign dynamic scram-sha-256 passwords to this role during pipeline runs

-- 4. Create explicit Application Runtime Roles (Used by the active pods)
CREATE ROLE smart_app_runtime_role NOLOGIN;

-- The runtime role CANNOT CREATE/DROP tables. It can only manipulate data.
GRANT USAGE ON SCHEMA public TO smart_app_runtime_role;

-- Ensure default privileges assign correct rights to the runtime role when migrations run
ALTER DEFAULT PRIVILEGES FOR ROLE smart_migration_role 
GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO smart_app_runtime_role;

ALTER DEFAULT PRIVILEGES FOR ROLE smart_migration_role 
GRANT USAGE, SELECT ON SEQUENCES TO smart_app_runtime_role;

-- 5. Service-Specific Identity Examples
-- Example for Identity Service
CREATE USER smart_identity_app WITH PASSWORD 'scram-sha-256...'; -- Injected via Vault
GRANT smart_app_runtime_role TO smart_identity_app;
-- Ensure smart_identity_app can only connect to smart_identity
GRANT CONNECT ON DATABASE smart_identity TO smart_identity_app;

-- Example for Payment Service
CREATE USER smart_payment_app WITH PASSWORD 'scram-sha-256...';
GRANT smart_app_runtime_role TO smart_payment_app;
GRANT CONNECT ON DATABASE smart_payment TO smart_payment_app;

-- 6. Read-Only Analytics Role (For ETL / Event polling if CDC is implemented)
CREATE ROLE smart_readonly_reporting_role NOLOGIN;
GRANT USAGE ON SCHEMA public TO smart_readonly_reporting_role;
ALTER DEFAULT PRIVILEGES FOR ROLE smart_migration_role 
GRANT SELECT ON TABLES TO smart_readonly_reporting_role;

-- 7. Audit specific protections (Append Only Example)
-- Revoke UPDATE and DELETE on audit tables from the runtime role.
-- REVOKE UPDATE, DELETE ON TABLE smart_audit.audit_events FROM smart_app_runtime_role;
