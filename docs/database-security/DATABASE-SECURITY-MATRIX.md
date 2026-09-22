# DATABASE SECURITY MATRIX

| Database | Service | Runtime Role | Migration Role | Read Access | Write Access | RLS | TLS | Network Restricted | Vault | Status |
|----------|---------|--------------|----------------|-------------|--------------|-----|-----|--------------------|-------|--------|
| `smart_identity` | Identity | `smart_identity_app` | `smart_migration` | Restricted | Restricted | Enabled | Yes | Yes (Identity Pod) | Yes | Secure |
| `smart_payment` | Payment | `smart_payment_app` | `smart_migration` | Restricted | Restricted | Enabled | Yes | Yes (Payment Pod) | Yes | Secure |
| `smart_customs` | Customs | `smart_customs_app` | `smart_migration` | Restricted | Restricted | Enabled | Yes | Yes (Customs Pod) | Yes | Secure |
| `smart_fleet` | Fleet | `smart_fleet_app` | `smart_migration` | Restricted | Restricted | No (EF Only) | Yes | Yes (Fleet Pod) | Yes | Secure |
| `smart_gps` | GPS | `smart_gps_app` | `smart_migration` | Restricted | Restricted | No (EF Only) | Yes | Yes (GPS Pod) | Yes | Secure |
| `smart_analytics`| Analytics| `smart_analytics_app`| `smart_migration` | Restricted | Read-Only | No (EF Only) | Yes | Yes (Analytics Pod)| Yes | Secure |
| `redis` | Cache | N/A | N/A | Restricted | Restricted | N/A | Yes | Yes | Yes | Secure |
