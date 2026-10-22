# Vault — SmartTrade Logistics Platform

## Overview

This directory contains all HashiCorp Vault configuration for the SmartTrade platform:

| File / Dir | Purpose |
|---|---|
| `setup-vault.sh` | Enterprise setup: Kubernetes auth, AppRole, dynamic DB credentials |
| `init-secrets.sh` | Dev init: writes all KV-v2 secrets on first boot |
| `policies/` | Per-service HCL policies (least-privilege read-only) |

---

## Secret Hierarchy (KV-v2)

```
secret/
├── shared/
│   └── jwt            ← JWT signing key, issuer, audience, lifetime
├── databases/
│   ├── postgres       ← host, port, db, username, password
│   ├── postgis        ← host, port, db, username, password
│   ├── mongodb        ← host, port, username, password
│   └── redis          ← host, port, password
├── messaging/
│   └── rabbitmq       ← host, port, username, password, vhost
├── services/
│   ├── identity       ← db connection string + JWT config
│   ├── payment        ← Stripe keys + db
│   ├── notification   ← Twilio + SendGrid + db
│   ├── fleet          ← Google Maps API key + db + postgis
│   ├── gps            ← postgis connection
│   ├── ais-vessel     ← AIS API key + db
│   ├── anpr           ← ANPR API key + db
│   ├── customs        ← db
│   ├── shipping       ← db
│   ├── analytics      ← db + mongo
│   ├── audit          ← mongo
│   ├── declaration    ← db
│   ├── corridor       ← postgis
│   ├── dryport        ← db
│   ├── ectn           ← db
│   └── integration    ← db
└── observability/
    └── grafana        ← admin password
```

---

## Local Development (Docker Compose)

```bash
# 1. Create your .env from the template
cp .env.example .env
# 2. Fill in every !!!CHANGE_ME!!! value

# 3. Start the full dev stack (Vault + all databases)
docker compose -f docker-compose.dev.yml up -d

# 4. Vault UI is at http://localhost:8200
#    Use the VAULT_DEV_ROOT_TOKEN from your .env to log in
```

The `vault-init` container runs automatically and writes all secrets.

### Reading a secret manually

```bash
export VAULT_ADDR=http://localhost:8200
export VAULT_TOKEN=<your VAULT_DEV_ROOT_TOKEN>

vault kv get secret/services/identity
vault kv get secret/shared/jwt
```

---

## Production / Staging

In production, Vault runs in **HA mode** (unsealed with auto-unseal via AWS KMS).  
Services authenticate using **Kubernetes auth** (bound to their ServiceAccount).

The Vault Agent sidecar injects secrets as environment variables — services
never call the Vault API directly.

### Applying policies

```bash
vault policy write identity-service infrastructure/vault/policies/identity-service.hcl
vault policy write payment-service  infrastructure/vault/policies/payment-service.hcl
vault policy write shared-base      infrastructure/vault/policies/shared-base.hcl
```

### Setting up Kubernetes auth (production)

```bash
bash infrastructure/vault/setup-vault.sh
```

---

## GitHub Actions Secrets

The following secrets must be set in **GitHub → Settings → Secrets and variables → Actions**:

| Secret name | Used by | Description |
|---|---|---|
| `JWT_SECRET` | CI build + deploy | Production JWT signing key |
| `JWT_SECRET_TEST` | CI tests | Integration-test JWT key (≥ 32 bytes) |
| `POSTGRES_PASSWORD` | CI tests | Test DB password |
| `STRIPE_SECRET_KEY` | CD deploy | Stripe live secret key |
| `STRIPE_WEBHOOK_SECRET` | CD deploy | Stripe webhook signing secret |
| `TWILIO_AUTH_TOKEN` | CD deploy | Twilio auth token |
| `TWILIO_ACCOUNT_SID` | CD deploy | Twilio account SID |
| `SENDGRID_API_KEY` | CD deploy | SendGrid API key |
| `GOOGLE_MAPS_API_KEY` | CD deploy | Google Maps API key |
| `AIS_API_KEY` | CD deploy | AIS vessel tracking API key |
| `ANPR_API_KEY` | CD deploy | ANPR/LPR API key |
| `VAULT_ADDR` | CD deploy | Vault server address |
| `VAULT_TOKEN` | CD deploy | Vault AppRole token (CI deployer role) |
| `AWS_READONLY_ROLE_ARN` | Security audit | ARN for ECR image scan |
| `AWS_ACCOUNT_ID` | Security audit | AWS account for ECR URI |
| `CODECOV_TOKEN` | CI | Codecov upload token |

> [!IMPORTANT]
> Rotate ALL secrets immediately if a secret is ever exposed in git history,
> logs, or error messages.  Use `git filter-repo` to purge history and force-push.
