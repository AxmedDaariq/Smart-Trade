# Cache Policy

- Implement explicit Cache-Aside strategy.
- Cache misses must query the authoritative PostgreSQL database.
- Treat Redis explicitly as non-authoritative acceleration.
- Implement invalidation via Outbox pattern (domain events). Do not assume cache updates persist simultaneously with SQL commits.
