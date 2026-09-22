# DATABASE THREAT MODEL

| Threat | Impact | Likelihood | Mitigation | Residual Risk |
|--------|--------|------------|------------|---------------|
| **SQL Injection** | Compromised Data | Low | Strict adherence to EF Core LINQ. Raw SQL parameterization. | Very Low |
| **Tenant Isolation Breach** | Data Leakage | Low | EF Core Global Query Filters (`TenantId`); JWT-driven identity. RLS on critical DBs. | Very Low |
| **Compromised App Pod** | Local DB Access | Medium | App roles lack DDL access (cannot DROP tables). Network Policies isolate DB to one pod. | Low |
| **Exposed Credentials** | DB Compromise | Low | Vault dynamic secrets. Secrets never logged or committed to Git. | Low |
| **Public Database Exposure** | DB Compromise | Low | Kubernetes internal VPC routing only. No public LoadBalancers attached to DB. | Very Low |
| **Malicious Insider (Admin)** | Data Theft/Tampering | Low | DB access audited via `pgaudit`. Read-only replicas used for analysis. | Medium |
| **Ransomware / Backup Deletion**| Data Loss | Low | S3 backups are versioned and logically separated from primary DB cluster. | Low |
