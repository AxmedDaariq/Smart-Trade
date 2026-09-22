# Backup and Disaster Recovery Strategy

## 1. PostgreSQL (Transactional Data)
- **Type**: Logical backups (pg_dump) + WAL archiving (pgBackRest/Barman).
- **Frequency**: Continuous WAL archiving. Full backup daily.
- **Retention**: 30 days hot, 7 years cold (Audit/Customs compliance).
- **PITR**: Enabled. Can restore to any second within the 30-day window.

## 2. MinIO (Object Storage - Evidence/Documents)
- **Type**: Bucket replication + Versioning.
- **Frequency**: Continuous active-active replication to standby cluster.
- **Retention**: Indefinite for Customs Evidence (ANPR images, manifests).

## 3. Redis (Cache / Backplane)
- **Type**: Ephemeral (No backup required).
- **Recovery**: System designed to safely degrade and rebuild cache from authoritative PostgreSQL data upon Redis restart.

## 4. RabbitMQ (Messaging)
- **Type**: Configuration backup (Definitions JSON).
- **Data Recovery**: Queues are durable, but in a catastrophic broker loss, services rely on the EF Core Outbox table in PostgreSQL to republish unpublished integration events.
