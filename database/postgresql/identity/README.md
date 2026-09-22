# Smart Platform Identity Database

This directory contains the database structure, migrations, and scripts specific to the **Identity & Access Microservice** (`smart_identity`).

## Governance
- **Owner:** Identity Microservice
- **Direct Access:** Strictly forbidden from outside the Identity application bounded context.

## Schema
- Uses `SmartDbContext` which inherits unified UTC auditing and concurrency standards.
- Uses PostgreSQL.
- Heavy utilization of `JSONB` for `SecurityEvent` and `IdentityAuditEntry`.

## Migrations
Migrations for this database are managed by EF Core within `src/services/Identity/Identity.Infrastructure`. Do not manually alter the schema outside of EF Migrations.

## Backups
Managed by the global `/database/scripts/backup` routines utilizing `pg_dump`.
