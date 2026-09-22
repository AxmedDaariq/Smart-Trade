# Redis Standards

## Use Cases
- Caching, distributed locks, idempotency, rate limiting, temporary session state.
- **MUST NOT be authoritative.** Systems must recover from PostgreSQL/MongoDB if Redis is lost.

## Key Naming Standard
- `service:resource:id`
- `service:tenant:resource:id`
- `service:cache:resource:key`
Examples: `identity:user:{id}:permissions`, `gps:vehicle:{id}:latest`

## Policies
- Define TTL policy for all entries.
- Never store secrets in Redis.
- Explicit distributed lock rules and idempotency key rules.
