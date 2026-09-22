# Backup and Recovery Foundation

## Strategy
- **PostgreSQL**: Use `pg_dump` or `pg_basebackup` depending on the requirement (logical vs physical).
- **MongoDB**: Use `mongodump` for logical backups or snapshotting based on deployment.
- **Redis**: Rely on primary DBs for persistent state; AOF/RDB can be configured if explicitly required but primarily treated as ephemeral/cache.

## Requirements
- Backups must be timestamped and follow standard naming conventions.
- Retention policy must be legally aligned.
- Backups must be tested for restoration integrity regularly.
