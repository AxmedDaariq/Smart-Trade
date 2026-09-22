# DATABASE DEPENDENCY MAP

This document maps the recovery order, criticality, RPO, and RTO across the Smart Trade & Logistics Corridor Platform data stores.

## Recovery Order Dependency Chain

1. **Vault / Secrets** (Infrastructure)
2. **smart_identity** (PostgreSQL) - Required by all for AuthZ/AuthN.
3. **smart_audit** (PostgreSQL) - Required for compliance appending on startup.
4. **RabbitMQ** (Messaging) - Core event bus.
5. **smart_integration** (PostgreSQL) - Provider mappings.
6. **smart_payment** (PostgreSQL) - Critical Financials.
7. **smart_customs** (PostgreSQL) - Critical Financials / Tariffs.
8. **smart_declaration** (PostgreSQL) - Trade logic.
9. **smart_ectn** (PostgreSQL) - Trade logic.
10. **smart_fleet** (PostgreSQL) - Logistics master data.
11. **smart_corridor** (PostgreSQL) - Geographic constraints.
12. **smart_anpr** (PostgreSQL) - Boundary operational data.
13. **smart_gps** (PostgreSQL / PostGIS) - High volume operational.
14. **smart_ais** (PostgreSQL) - Vessel operational.
15. **smart_notification** (PostgreSQL) - Outbound messaging.
16. **Redis** (In-Memory) - Rebuilds from runtime.
17. **smart_analytics** (PostgreSQL) - Rebuilds from persistent events if necessary.

## RPO & RTO Definition Matrix

| Service | Database | Criticality | RPO (Prod) | RTO (Prod) | Backup Method | Restore Method |
|---------|----------|-------------|------------|------------|---------------|----------------|
| Identity | `smart_identity` | CRITICAL | 5 minutes | 15 minutes | WAL Archiving / Base | pgBackRest / PITR |
| Payment | `smart_payment` | CRITICAL | 1 minute | 15 minutes | WAL Archiving / Base | pgBackRest / PITR |
| Customs | `smart_customs` | CRITICAL | 5 minutes | 30 minutes | WAL Archiving / Base | pgBackRest / PITR |
| Audit | `smart_audit` | CRITICAL | 5 minutes | 30 minutes | WAL Archiving / Base | pgBackRest / PITR |
| Fleet | `smart_fleet` | CRITICAL | 15 minutes | 1 hour | Logical / pg_dump | pg_restore |
| ECTN | `smart_ectn` | CRITICAL | 15 minutes | 1 hour | Logical / pg_dump | pg_restore |
| Declaration| `smart_declaration` | CRITICAL | 15 minutes | 1 hour | Logical / pg_dump | pg_restore |
| GPS | `smart_gps` | CRITICAL | 30 minutes | 2 hours | Logical + Base | pgBackRest |
| Corridor | `smart_corridor` | CRITICAL | 30 minutes | 2 hours | Logical | pg_restore |
| ANPR | `smart_anpr` | CRITICAL | 30 minutes | 2 hours | Logical | pg_restore |
| AIS | `smart_ais` | HIGH | 1 hour | 4 hours | Logical | pg_restore |
| Integration| `smart_integration` | HIGH | 15 minutes | 2 hours | Logical | pg_restore |
| Notification|`smart_notification`| HIGH | 1 hour | 2 hours | Logical | pg_restore |
| Analytics | `smart_analytics` | ANALYTICS | 24 hours | 8 hours | Logical / Event Replay| pg_restore / Rebuild |
