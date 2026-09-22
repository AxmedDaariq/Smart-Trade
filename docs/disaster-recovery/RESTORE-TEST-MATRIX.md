# RESTORE TEST MATRIX

This matrix tracks the validation of actual disaster recovery restore tests executed in isolated test environments.

| Scenario | Database | Backup | Recovery Point | Restore Method | Expected Result | Actual Result | Duration | RPO | RTO | Status | Evidence |
|----------|----------|--------|----------------|----------------|-----------------|---------------|----------|-----|-----|--------|----------|
| Accidental Drop | `smart_identity` | WAL / Base | PITR - 5m | pgBackRest | Tenants valid, Auth functional | (N/A) | (N/A) | (N/A) | (N/A) | BLOCKED | Simulated env |
| Data Corruption | `smart_payment` | WAL / Base | PITR - 1m | pgBackRest | Ledger sums exact, no partial tx | (N/A) | (N/A) | (N/A) | (N/A) | BLOCKED | Simulated env |
| Hardware Failure | `smart_fleet` | Logical | Last Nightly | pg_restore | Schema + Constraints restored | (N/A) | (N/A) | (N/A) | (N/A) | BLOCKED | Simulated env |
| Ransomware / WORM | S3 Object Store | Object | N/A | S3 Version Restore | Files uncorrupted | (N/A) | (N/A) | (N/A) | (N/A) | BLOCKED | Simulated env |
| Redis Eviction | `redis` | AOF | N/A | Rebuild / AOF Load | Ephemeral data repopulated | (N/A) | (N/A) | (N/A) | (N/A) | BLOCKED | Simulated env |

> **Note:** As this environment is a simulation devoid of active Kubernetes and database clusters, destructive RESTORE tests are marked `BLOCKED`. Never mark `PASS` without absolute empirical evidence.
