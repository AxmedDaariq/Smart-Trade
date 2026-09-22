# Notification Redis Documentation

Redis in the Notification Service provides ephemeral caching to handle notification storms, deduping, rate limiting, and real-time state resolution without overwhelming the `smart_notification` PostgreSQL instance.

## Caching Strategies

### Storm Protection / Idempotency
- `notification:idempotency:{tenantId}:{operation}:{key}`
**Purpose**: Rapidly reject duplicate payloads before attempting PostgreSQL inserts (e.g. 500 identical GPS offline events).

### Unread States
- `notification:unread:{tenantId}:{userReference}`
**Purpose**: Instant retrieval of the unread badge count for UI clients.

### Rate Limiting
- `notification:rate-limit:{tenantId}:{userReference}:{channelType}`
**Purpose**: Limit outgoing SMS/Email per user/tenant to prevent abusive loops.

### Health
- `notification:provider-health:{provider}`
**Purpose**: Fast circuit breaker checks on external providers (SMS/Email Gateways).
