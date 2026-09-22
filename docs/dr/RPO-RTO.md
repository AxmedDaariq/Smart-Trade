# RPO and RTO Targets (Engineering Specifications)

| Service | RPO Target (Max Data Loss) | RTO Target (Max Downtime) | Observed RPO | Observed RTO | Status |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **API Gateway** | 0 (Stateless) | < 1 minute | N/A | N/A | **NOT TESTED** |
| **Identity** | 0 (PITR) | < 5 minutes | N/A | N/A | **NOT TESTED** |
| **Customs (Core)** | 0 (PITR + Outbox) | < 15 minutes | N/A | N/A | **NOT TESTED** |
| **Payment (Core)**| 0 (PITR + Ledger) | < 15 minutes | N/A | N/A | **NOT TESTED** |
| **GPS (Telemetry)**| < 5 minutes | < 30 minutes | N/A | N/A | **NOT TESTED** |
| **ANPR (Edge)** | < 1 minute | < 15 minutes | N/A | N/A | **NOT TESTED** |
| **RabbitMQ** | < 1 minute (Outbox) | < 10 minutes | N/A | N/A | **NOT TESTED** |
