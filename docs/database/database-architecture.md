# Database Architecture

## Logical Architecture
React -> API Gateway -> Microservices -> Service-owned databases

## Database Ownership
| Service | Database | Note |
|---|---|---|
| Identity | PostgreSQL | |
| ECTN | PostgreSQL | |
| Declaration | PostgreSQL | |
| Customs | PostgreSQL | |
| Payment | PostgreSQL | |
| Fleet | PostgreSQL | |
| GPS | PostgreSQL + PostGIS | Spatial |
| AIS | PostgreSQL + MongoDB | Raw telemetry in Mongo |
| Corridor | PostgreSQL + PostGIS | Spatial |
| ANPR | PostgreSQL + PostGIS | Spatial |
| Integration | PostgreSQL | |
| Notification | PostgreSQL | |
| Audit | PostgreSQL | |
| Analytics | Analytics Store | |

## MongoDB Strategy
- AIS raw, Telemetry, Provider payloads, Flexible events.

## Redis Strategy
- Cache, Idempotency, Locks, Latest state, Rate limiting.

## Multi-Tenancy
- Enforced at application level via `tenant_id`.

## Backup Strategy
- PostgreSQL: `pg_dump` / `pg_basebackup`.
- MongoDB: `mongodump`.

## Disaster Recovery
- RPO/RTO: TBD / measured during later DR testing.
