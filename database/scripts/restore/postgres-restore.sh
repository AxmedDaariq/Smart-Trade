#!/bin/bash
# Restore PostgreSQL databases
if [ -z "$1" ]; then
  echo "Usage: $0 <backup-file>"
  exit 1
fi

BACKUP_FILE=$1
echo "Restoring from $BACKUP_FILE..."

pg_restore -h localhost -U postgres -d smart_platform -1 $BACKUP_FILE

echo "Restore completed."
