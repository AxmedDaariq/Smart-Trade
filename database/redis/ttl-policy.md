# TTL Policy

Every cached or state key in Redis MUST have a Time-To-Live (TTL).

- **Short-Lived (10-60s)**: Locks, Idempotency tokens, Replay Nonces, Gate Authorizations.
- **Medium (1-15m)**: GPS Latest, ANPR State, Dashboard Summaries.
- **Long (1-24h)**: User Sessions (governed by OAuth refresh policies), Reference Data.

No permanent keys are allowed without architect exemption.
