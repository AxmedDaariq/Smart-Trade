# Rate Limit Policy

Rate limits must leverage `IRateLimiter`.

- Sliding or fixed windows managed entirely via atomic Lua scripts inside Redis.
- Local in-memory limits are forbidden for distributed production enforcement.
- Internal service calls must not be collaterally rate-limited by public API protections. Use specific namespaces (e.g. `smart:system:ratelimit:ip:{ip}`).
