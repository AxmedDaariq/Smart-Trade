# Failure Policy

If Redis is down:
- Core operational reads must fallback to PostgreSQL (Fail-Open).
- Security verifications (e.g., ANPR gate authorization cache, Authentication replays) must Fail-Closed and explicitly deny access to prevent bypass.
