# Enterprise Redis Architecture

## Overview
Redis acts as a distributed runtime and acceleration layer for the Smart Trade & Logistics Corridor Platform. It provides fast access to idempotent checks, locks, and temporary state.

**CRITICAL TENET**: Redis MUST NEVER become the authoritative source of truth for business data.

## Namespace Structure
All keys must adhere to the `smart:{service}:{resource}:{scope}:{identifier}` naming convention.
- `service`: `identity`, `fleet`, `gps`, `corridor`, `controltower`, `system`, etc.
- `resource`: `vehicle`, `session`, `ratelimit`, `trip`, etc.
- `scope`: `tenantId` (Mandatory wherever applicable to enforce multi-tenancy isolation).
- `identifier`: The specific resource ID.

Example: `smart:fleet:vehicle:t1234:v5678`

## Serialization
Use JSON explicitly with a `schemaVersion` attached, avoiding arbitrary object types from the ORM.

## Fail-Open vs Fail-Closed
- **Fail-Open**: Configuration references, summary dashboards, generic health checks. If Redis fails, fall back directly to PostgreSQL.
- **Fail-Closed**: Idempotency gates, ANPR authorizations, lock acquisitions, replay protection (nonces). If Redis fails for these, reject the transaction gracefully.
