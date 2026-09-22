# DISASTER RECOVERY RUNBOOK

This runbook outlines the operational steps taken during a SEV-1 catastrophic failure (e.g., regional outage, severe cluster failure, mass ransomware event).

## 1. Incident Declaration
If PagerDuty alerts fire for `PostgreSQL Primary Unreachable` across multiple domains, declare a SEV-1 incident in Slack (`#incident-sev1-database`).

## 2. Identify Failure & Freeze Writes
Identify if the outage is hardware, network, or logical data corruption.
**If Data Corruption:**
- Scale all API Deployments down to `0` to prevent cascading corruption and stop further data ingestion.
  ```bash
  kubectl scale deploy -l tier=api --replicas=0 -n smart-trade-prod
  ```

## 3. Provision Recovery Environment
If the primary cluster is permanently lost, instantiate the DR Terraform cluster.
- Apply Infrastructure (`terraform apply -var="env=dr"`)
- Ensure HashiCorp Vault is unsealed in the new region.

## 4. Execute Restore Sequence
Follow the exact dependency chain established in `DATABASE-DEPENDENCY-MAP.md`:
1. **Unseal Vault & Secrets**
2. **Restore PostgreSQL `smart_identity` (Auth Core)**
3. **Restore PostgreSQL `smart_payment`, `smart_customs` (Financial Core)**
4. **Restore Messaging (RabbitMQ state)**
5. **Restore Operational Domains (Fleet, GPS, Corridor)**
6. **Flush & Rebuild Redis** (from events)

## 5. Post-Restore Validation
- **Tenant Isolation:** Run API smoke tests ensuring Tenant A tokens cannot read Tenant B endpoints.
- **Financials:** Run Ledger Reconciliation checks on `smart_payment`.
- **Idempotency:** Verify Inbox/Outbox records to ensure RabbitMQ message replay won't duplicate customs assessments.

## 6. Resume Operations & Post-Incident Review
1. Scale APIs back up incrementally.
2. Monitor Application APM for Error Spikes.
3. Close the SEV-1.
4. Schedule a Post-Incident Review (PIR) to document the exact Data Loss Window and actual Time-To-Recover vs RTO.
