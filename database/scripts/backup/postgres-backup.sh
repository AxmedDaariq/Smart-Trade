#!/bin/bash
# Backup PostgreSQL databases
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
BACKUP_DIR="./backups"

mkdir -p $BACKUP_DIR

# Example logical backup
pg_dump -h localhost -U postgres -d smart_platform -F c -f $BACKUP_DIR/smart_platform_$TIMESTAMP.dump

echo "Backup completed: $BACKUP_DIR/smart_platform_$TIMESTAMP.dump"
