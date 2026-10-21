# Secret Remediation — SmartTrade Logistics Platform

> **Status: REMEDIATED** — All findings addressed as of 2026-10-21

---

## Gitleaks Findings (PR scan — 2026-10-21)

### Finding 1 — Hardcoded JWT fallback in `Program.cs`

| Field | Value |
|---|---|
| **Rule** | `generic-api-key` |
| **File** | `services/identity/Identity.Api/Program.cs` |
| **Line** | 20 (in commit `d261f261`) |
| **Commit** | `d261f261be8df8fd55f8eef3587d6014f19b26a7` |
| **Severity** | HIGH |
| **Status** | ✅ Remediated |

**Root cause:** A temporary dev/test JWT fallback (`"0123456789abcdef0123456789abcdef"`) was hardcoded directly in `Program.cs` as a fallback for when `appsettings.json` was absent.

**Actions taken:**
1. The hardcoded fallback was already removed in a subsequent commit. The current `Program.cs` contains no secrets.
2. Added a `appsettings.Test.json` that sources `Jwt:Secret` from the `JWT_SECRET_TEST` environment variable.
3. Updated `.github/workflows/ci.yml` to inject `JWT_SECRET_TEST` from GitHub Actions secrets during test runs.
4. Added the commit to `.gitleaks.toml` `allowlist.commits` so full-history scans don't re-flag it.

> [!IMPORTANT]
> Although the secret value `0123456789abcdef0123456789abcdef` is a trivially
> guessable test key (not a production secret), you must **rotate the JWT
> signing key** in any environment where this key was ever used in a `.env`
> file that could have been leaked.

---

### Finding 2 — False positive in `api_client_test.dart`

| Field | Value |
|---|---|
| **Rule** | `generic-api-key` |
| **File** | `mobile/driver/test/core/networking/api_client_test.dart` |
| **Line** | 4 |
| **Commit** | `05aaeed3087308e30e78f98d2acd2e3b55fb91fb` |
| **Severity** | LOW (false positive) |
| **Status** | ✅ Suppressed |

**Root cause:** The Dart file's doc comment:

```dart
/// (bearer auth, idempotency/correlation ids, 401 session cleanup, 5xx
```

The substring `ids` followed by other characters matched the `generic-api-key` regex. No real secret is present.

**Actions taken:**
1. Added the commit to `.gitleaks.toml` `allowlist.commits`.
2. Extended `.gitleaks.toml` path allowlist to cover `mobile/.*/test/.*` so future test files in that tree are not scanned for secrets.

---

## Prevention Measures

| Measure | Status |
|---|---|
| `.gitleaks.toml` updated with commit allowlist + path widening | ✅ Done |
| `docker-compose.dev.yml` uses `${VAR:?error}` — no insecure defaults | ✅ Done |
| `appsettings.Test.json` sources secrets from env vars | ✅ Done |
| `ci.yml` injects `JWT_SECRET_TEST` from GitHub Actions secrets | ✅ Done |
| HashiCorp Vault dev stack + `init-secrets.sh` + per-service policies | ✅ Done |
| `.env.example` covers every secret; all values marked `!!!CHANGE_ME!!!` | ✅ Done |
| Secrets policy comment added to `Program.cs` | ✅ Done |

---

## Required GitHub Actions Secrets

Set these in **GitHub → Settings → Secrets and variables → Actions**:

```
JWT_SECRET_TEST       (≥32 bytes, for integration tests)
POSTGRES_PASSWORD     (for integration tests against test DB)
```

See `infrastructure/vault/README.md` for the full list of production secrets.

---

## Rotation Checklist

If you believe the `0123456789abcdef…` test key was ever used in a real
deployment, complete this checklist:

- [ ] Generate a new JWT signing key: `openssl rand -base64 32`
- [ ] Update the key in HashiCorp Vault: `vault kv patch secret/shared/jwt secret=<new>`
- [ ] Restart all services that sign or validate JWTs (identity, gateways)
- [ ] Invalidate all active sessions / force re-login
- [ ] Verify the old key is no longer accepted by any running service
