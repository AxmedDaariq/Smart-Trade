# System Performance Inventory

## Services and Expected Workloads

| Service | Primary Endpoint / Transport | Auth Reqd | Tenant Reqd | Expected Workload | Backend Dependency | Realtime Dependency |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| **API Gateway** | `/*` (All HTTP routing) | Yes (JWT) | N/A | Extremely High (All ingress) | Identity (Auth) | N/A |
| **Identity** | `/connect/token` | No | N/A | High (Login/Refresh bursts) | PostgreSQL | N/A |
| **ECTN** | `POST /api/v1/ectn` | Yes | Yes | Medium (Batch processing) | PostgreSQL | N/A |
| **Declaration** | `POST /api/v1/declarations`| Yes | Yes | Medium (Sustained) | PostgreSQL | N/A |
| **Customs** | `POST /api/v1/customs/assess`| Yes | Yes | Medium (CPU Intensive) | PostgreSQL, Redis | Event Bus (MassTransit) |
| **Payment** | `POST /webhooks/payment` | Yes (Sig) | No | Low/Med (Idempotency critical)| PostgreSQL | N/A |
| **Fleet** | `GET /api/v1/fleet/vehicles` | Yes | Yes | High (Lookup/Dispatch) | PostgreSQL | N/A |
| **GPS** | `POST /api/v1/gps/ingest` | Yes (mTLS)| Yes | **Very High (1,000+ RPS)** | PostgreSQL (PostGIS) | Event Bus (MassTransit) |
| **ANPR** | `POST /api/v1/anpr/recognize`| Yes (mTLS)| Yes | High (Border Bursts) | PostgreSQL | Event Bus (MassTransit) |
| **Corridor** | `RabbitMQ Consumer` | N/A | N/A | High (Trip updates) | PostgreSQL | N/A |
| **Control Tower**| `ControlTowerHub` (SignalR) | Yes | Yes | **Very High (1,000+ Sockets)** | Redis (Backplane) | SignalR/WebSockets |
