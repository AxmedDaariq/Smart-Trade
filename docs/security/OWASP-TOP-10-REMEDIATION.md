# OWASP Top 10 (2021) Remediation

Audit and remediation of the .NET backend against the OWASP Top 10. Every HTTP service is wired
through the shared building blocks in `building-blocks/web/`, so the hardening below is applied
consistently rather than per-service.

## Shared building blocks

| File | Responsibility |
|---|---|
| `building-blocks/web/PlatformSecurityExtensions.cs` | Fallback authorization, rate limiting, CORS, exception handling, security headers |
| `building-blocks/web/PlatformAuthenticationExtensions.cs` | JWT bearer setup with strict key/issuer/audience validation |
| `building-blocks/web/PlatformExceptionHandler.cs` | Generic ProblemDetails responses; stack traces logged server-side only |
| `building-blocks/web/PlatformUrlGuard.cs` | SSRF guard for caller-supplied URLs (A10) |

All 12 API services (`ais-vessel`, `analytics`, `anpr-gate`, `corridor`, `customs`, `declaration`,
`ectn`, `fleet`, `gps-telematics`, `identity`, `integration`, `payment`) call
`AddPlatformJwtAuthentication`, `AddPlatformSecurity` and `UsePlatformSecurity`.

## A01 — Broken Access Control

- **Deny-by-default.** `AddPlatformAuthorization` registers a `FallbackPolicy` requiring an
  authenticated user, so any endpoint added later without `[Authorize]`/`[AllowAnonymous]` is
  protected automatically. Previously, services without a fallback policy silently defaulted to
  anonymous.
- **Explicit `[AllowAnonymous]` is deliberate and narrow**: health probes (`/health/live`,
  `/health/ready`), the login endpoint, and webhook ingestion (which authenticates by signature
  instead — see A08).
- **Tenant isolation** is taken from the validated JWT, not the query string. `KpiController` and
  `KpisController` previously accepted `tenantId` from the query and fell back to it when the claim
  was missing, allowing cross-tenant reads; both now require the claim.
- Authorization policies are declared per service (`identity.*`, `ectn.*`, `fleet.*`, `customs.*`,
  `ais.*`, `analytics.*`).

## A02 — Cryptographic Failures

- JWT signing key must be at least 32 UTF-8 bytes, and issuer/audience must be non-empty — validated
  at startup so a misconfigured service fails fast instead of booting with an insecure key.
- `AccessTokenLifetimeMinutes` must be between 1 and 60.
- `JwtTokenGenerator` now emits `nbf` (via `notBefore`) alongside `exp`. Without it, `ValidFrom`
  defaulted to `0001-01-01`, producing a token with an unbounded validity window on parsers that
  tolerate a missing `nbf`.
- Secrets come from configuration/environment (secret manager), never from committed
  `appsettings.json`.

## A03 — Injection

- **SQL (ECTN number generation).** The database sequence name is bound as a SQL parameter
  (`SELECT nextval(@sequenceName::regclass)`) instead of being interpolated into the statement. The
  client-supplied port code is reduced to alphanumerics before use.
- **Config path traversal.** `ConfigurationWebhookSecretStore` restricts the partner code to
  `^[A-Za-z0-9_-]{1,32}$` before it is used as a configuration index path, blocking values such as
  `../../Jwt/Secret`.
- EF Core parameterized queries are used throughout; no string-concatenated SQL remains.

## A04 — Insecure Design

- Per-IP rate limiting with a stricter partition (`platform-sensitive`, 10 req/min by default) for
  brute-forceable endpoints; global limit is 120 req/min. Rejections return a generic 429 body.
- Request size limits on document upload (10 MB).
- Webhook ingestion fails closed: unknown partners, missing secrets, and missing signatures are all
  rejected.

## A05 — Security Misconfiguration

- **CORS is opt-in.** `AddPlatformCors` builds a policy only from `Cors:AllowedOrigins`; there is no
  `AllowAnyOrigin` fallback, so an unconfigured service stays same-origin only.
- **Swagger is Development-only** — `UseSwagger`/`UseSwaggerUI` are gated on
  `app.Environment.IsDevelopment()`.
- **Security headers** on every response: `X-Content-Type-Options: nosniff`,
  `X-Frame-Options: DENY`, `Referrer-Policy: no-referrer`,
  `Permissions-Policy: camera=(), microphone=(), geolocation=(), payment=()`,
  `Cross-Origin-Opener-Policy: same-origin`; HSTS outside Development.
- `PlatformExceptionHandler` returns generic ProblemDetails; internal exception detail is logged but
  never returned to the client.
- CI/CD tokens are least-privilege (`permissions: contents: read`).

## A06 — Vulnerable and Outdated Components

- The `build-and-test` CI job runs `dotnet list <project> package --vulnerable --include-transitive
  --severity high` across every project and fails the pipeline when a High/Critical advisory is
  found.
- GitHub Actions are pinned to exact versions (no `@master`/`@main`), and `zaproxy/action-baseline`
  runs as an OWASP ZAP baseline scan on deploy.
- Note: the repo mixes `net8.0`, `net9.0` and `net10.0` targets. Test projects have been aligned to
  `net10.0` to match `global.json` (SDK 10) and the SDK installed in CI — a `net9.0` test project
  cannot run when only the .NET 10 runtime is present.

## A07 — Identification and Authentication Failures

- Startup validation of JWT configuration (key length, issuer, audience, token lifetime).
- Rate limiting on authentication endpoints (see A04).
- `MapInboundClaims = false` with explicit claim mapping so permission claims are read consistently.

## A08 — Software and Data Integrity Failures

- **Webhook signature verification.** `HmacExternalRequestSigner` computes HMAC-SHA256 over the raw
  payload and compares with `CryptographicOperations.FixedTimeEquals` (constant-time, so timing
  differences cannot be used to brute-force a signature). Signatures are expected as
  `sha256=<hex>`.
- `ProcessWebhookCommand` rejects unsigned/invalidly-signed payloads before any processing occurs.
- CI action pinning and least-privilege `GITHUB_TOKEN` (see A05/A06).

## A09 — Logging and Monitoring Failures

- Rate-limit rejections are logged (`PlatformSecurity` category) with method, path and remote IP.
- Unhandled exceptions are logged in full server-side while the client receives a generic response.
- Auth events (successful/failed authentication, forbidden access) are logged through the JWT bearer
  events in `PlatformAuthenticationExtensions`.

## A10 — Server-Side Request Forgery

- `PlatformUrlGuard.IsSafe` / `TryGetRejectionReason` / `EnsureSafe` validate any caller-supplied
  URL before the service issues an outbound request, rejecting non-HTTP(S) schemes and unsafe hosts.

## Verification

- **Unit/integration tests**: 19 test projects (see `find . -name '*.csproj' | grep -Ei
  'Tests\.csproj$'`).
- **Live smoke test**: `Fleet.Api` verified end-to-end — unauthenticated call returns `401` with
  `WWW-Authenticate: Bearer`, `/health/live` stays `200` anonymous, an unconfigured CORS origin gets
  no `Access-Control-Allow-*` headers, a valid JWT is accepted, and a tampered (bad-signature) JWT is
  rejected.
- **CI**: dependency vulnerability gate, Trivy image scan, Checkov IaC scan, SonarQube, OWASP ZAP
  baseline scan.

## Related documents

- `docs/security/PHASE-15.6-SECURITY-BASELINE.md` — Zero Trust architecture and trust boundaries
- `docs/security/threat-model.md` — threat model
- `docs/devsecops/DEVSECOPS-RUNBOOK.md` — pipeline operations
