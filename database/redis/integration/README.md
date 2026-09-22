# Integration Redis Documentation

Redis in the Integration Service provides ephemeral caching to accelerate operational throughput for circuit breaking and rate limiting without compromising the authoritative integrity of PostgreSQL.

## Caching Strategies

### Idempotency
- `integration:idempotency:{tenantId}:{operation}:{key}`
**Purpose**: Rapidly reject duplicate payloads before attempting PostgreSQL inserts.

### Webhook Deliveries
- `integration:webhook:{provider}:{eventId}`
**Purpose**: Duplicate event debouncing for massive inbound streams.

### Circuit Breakers
- `integration:circuit:{systemId}:{endpointId}`
**Purpose**: Rapid state transitions (`Open`, `HalfOpen`) for external provider connectivity.

### Rate Limiting
- `integration:rate-limit:{tenantId}:{endpointId}`
**Purpose**: Token Bucket or Sliding Window storage.
