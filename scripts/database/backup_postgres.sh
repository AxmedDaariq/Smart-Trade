#!/bin/bash
set -eo pipefail

echo "Starting logical backup of all databases..."
TIMESTAMP=$(date +%Y%m%d_%H%M%S)
BACKUP_DIR="/backups"
DB_LIST="smart_identity smart_ectn smart_declaration smart_customs smart_payment smart_fleet smart_gps smart_corridor smart_anpr smart_integration smart_notification smart_audit smart_analytics"

# Requires aws-cli to be installed in the pod/container running this
for DB in $DB_LIST; do
  echo "Backing up $DB..."
  DUMP_FILE="${BACKUP_DIR}/${DB}_${TIMESTAMP}.sql.gz"
  
  # Perform logical backup
  pg_dump -h $PGHOST -U $PGUSER -d $DB | gzip > $DUMP_FILE
  
  # Generate checksum
  CHECKSUM=$(sha256sum $DUMP_FILE | awk '{print $1}')
  echo "$CHECKSUM" > "${DUMP_FILE}.sha256"
  
  # Upload to S3
  echo "Uploading $DB backup to S3..."
  aws s3 cp $DUMP_FILE s3://${S3_BUCKET}/postgres/${DB}/${TIMESTAMP}/
  aws s3 cp "${DUMP_FILE}.sha256" s3://${S3_BUCKET}/postgres/${DB}/${TIMESTAMP}/
  
  echo "Completed backup for $DB (Checksum: $CHECKSUM)"
done

echo "All logical backups completed successfully."
