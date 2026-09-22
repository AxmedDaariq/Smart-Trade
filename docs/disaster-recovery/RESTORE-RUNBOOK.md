# DATABASE RESTORE RUNBOOK

This runbook describes the explicit steps to restore a PostgreSQL database (e.g. `smart_payment`) from an S3 backup, covering both Logical Dumps and Point-In-Time-Recovery (PITR).

## Prerequisites
- Access to the target Kubernetes Cluster (`kubectl`).
- Appropriate Vault access roles to decrypt the PostgreSQL and S3 credentials.

---

## Scenario A: Logical Restore (pg_dump recovery)
*Use when a non-critical database needs a full overwrite or a specific missing table needs recovery.*

1. **Locate the Backup in S3**
   ```bash
   aws s3 ls s3://smart-trade-backups-bucket/postgres/smart_payment/
   ```
2. **Download and Verify Checksum**
   ```bash
   aws s3 cp s3://smart-trade-backups-bucket/postgres/smart_payment/20260921_020000/smart_payment_20260921_020000.sql.gz .
   aws s3 cp s3://smart-trade-backups-bucket/postgres/smart_payment/20260921_020000/smart_payment_20260921_020000.sql.gz.sha256 .
   sha256sum -c smart_payment_20260921_020000.sql.gz.sha256
   ```
   *If `FAILED`, abort the recovery and locate a non-corrupted backup.*

3. **Restore to Isolated Database**
   ```bash
   gunzip -c smart_payment_20260921_020000.sql.gz | psql -h isolated-db-instance -U postgres -d smart_payment
   ```
4. **Validate Data**
   Run the test matrix queries (e.g., verifying Payment counts, ledger balance constraints).

---

## Scenario B: Point-In-Time-Recovery (PITR) using pgBackRest
*Use when critical financial/identity databases suffer from accidental truncation or corruption and must be restored to a specific minute before the failure.*

1. **Identify the exact failure timestamp (UTC)**
   Let's assume the failure occurred at `2026-09-21 14:32:00`. We want to restore to `14:31:59`.

2. **Execute pgBackRest Restore Command** (Running inside the Postgres pod or a sidecar recovery pod)
   ```bash
   pgbackrest --stanza=smart_payment --type=time --target="2026-09-21 14:31:59" --delta restore
   ```

3. **Startup PostgreSQL**
   The database will automatically replay the WAL logs from S3 until it reaches the exact target timestamp.

4. **Verify Constraints and Relationships**
   - Check the `smart_payment` `Outbox` table. Are there records that were processed between `14:31:59` and now? These may need replay.
