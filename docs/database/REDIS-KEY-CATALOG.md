# Redis Key Catalog

| Service       | Key Pattern                                     | Purpose                    | Data Type | Tenant Scoped | TTL       | Source of Truth | Security Classification | Eviction Policy | Failure Behavior |
|---------------|-------------------------------------------------|----------------------------|-----------|---------------|-----------|-----------------|-------------------------|-----------------|------------------|
| Identity      | `smart:identity:session:{sessionId}`            | Runtime Session State      | Hash      | Yes           | 1-24h     | Auth DB         | High                    | No Eviction     | Fail-Closed      |
| Fleet         | `smart:fleet:vehicle:{tenantId}:{vehicleId}`    | Cache Vehicle Summary      | String/JSON| Yes          | 15m       | Postgres        | Internal                | Volatile-LRU    | Fail-Open        |
| GPS           | `smart:gps:latest:{tenantId}:{vehicleId}`       | Latest Location            | String/JSON| Yes          | 60s       | PostGIS/Stream  | Confidential            | Volatile-LRU    | Fail-Open        |
| GPS           | `smart:gps:device:{tenantId}:{deviceId}`        | Device State               | Hash      | Yes           | 5m        | Postgres        | Confidential            | Volatile-LRU    | Fail-Open        |
| Corridor      | `smart:corridor:trip:{tenantId}:{tripId}`       | Realtime Trip State        | Hash      | Yes           | 1h        | Postgres        | Internal                | Volatile-LRU    | Fail-Open        |
| ANPR          | `smart:anpr:authorization:{tenantId}:{reqId}`   | Gate Authorization         | String    | Yes           | 15s       | None            | High                    | No Eviction     | Fail-Closed      |
| Notification  | `smart:notification:unread:{tenantId}:{userId}` | Unread Counter             | Integer   | Yes           | 24h       | Postgres        | Internal                | Volatile-LRU    | Fail-Open        |
| ControlTower  | `smart:controltower:trip:{tenantId}:{tripId}`   | Realtime Aggregation Cache | Hash      | Yes           | 1m        | Events          | Internal                | Volatile-LRU    | Fail-Open        |
| System        | `smart:system:ratelimit:{scope}:{identifier}`   | Rate Limiting              | Integer   | Optional      | 1m        | None            | Internal                | Volatile-TTL    | Fail-Open        |
| System        | `smart:security:nonce:{nonce}`                  | Replay Protection          | String    | No            | 5m        | None            | High                    | No Eviction     | Fail-Closed      |
