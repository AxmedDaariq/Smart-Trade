#!/bin/sh
# infrastructure/vault/init-secrets.sh
# ─────────────────────────────────────────────────────────────────────────────
# Vault Dev Initialiser
#
# Runs once after Vault starts in dev mode.  Writes ALL platform secrets into
# the KV-v2 secrets engine and applies per-service policies.
#
# Triggered by the `vault-init` service in docker-compose.dev.yml.
# In production, Terraform manages the Vault resources instead.
# ─────────────────────────────────────────────────────────────────────────────
set -euo pipefail

echo "==> Enabling KV-v2 secrets engine at secret/"
vault secrets enable -path=secret kv-v2 2>/dev/null || echo "    (already enabled)"

# ─────────────────────────────────────────────────────────────────────────────
# JWT / Authentication Secrets
# ─────────────────────────────────────────────────────────────────────────────
echo "==> Writing secret/data/shared/jwt"
vault kv put secret/shared/jwt \
  secret="${JWT_SECRET}" \
  issuer="${JWT_ISSUER}" \
  audience="${JWT_AUDIENCE}" \
  access_token_lifetime_minutes="${JWT_LIFETIME_MINUTES}"

# ─────────────────────────────────────────────────────────────────────────────
# Database Secrets (Static — dev only; prod uses dynamic DB credentials)
# ─────────────────────────────────────────────────────────────────────────────
echo "==> Writing secret/data/databases/postgres"
vault kv put secret/databases/postgres \
  host="postgres" \
  port="5432" \
  database="${POSTGRES_DB:-smart_platform}" \
  username="${POSTGRES_USER:-postgres}" \
  password="${POSTGRES_PASSWORD}"

echo "==> Writing secret/data/databases/postgis"
vault kv put secret/databases/postgis \
  host="postgis" \
  port="5432" \
  database="${POSTGIS_DB:-smart_gis}" \
  username="${POSTGIS_USER:-postgres}" \
  password="${POSTGIS_PASSWORD}"

echo "==> Writing secret/data/databases/mongodb"
vault kv put secret/databases/mongodb \
  host="mongodb" \
  port="27017" \
  username="${MONGO_USER:-root}" \
  password="${MONGO_PASSWORD}"

echo "==> Writing secret/data/databases/redis"
vault kv put secret/databases/redis \
  host="redis" \
  port="6379" \
  password="${REDIS_PASSWORD}"

echo "==> Writing secret/data/messaging/rabbitmq"
vault kv put secret/messaging/rabbitmq \
  host="rabbitmq" \
  port="5672" \
  username="${RABBITMQ_USER:-smart_trade}" \
  password="${RABBITMQ_PASSWORD}" \
  vhost="${RABBITMQ_VHOST:-/smart_trade}"

# ─────────────────────────────────────────────────────────────────────────────
# Per-Service Secrets
# ─────────────────────────────────────────────────────────────────────────────
echo "==> Writing secret/data/services/identity"
vault kv put secret/services/identity \
  jwt_secret="${JWT_SECRET}" \
  jwt_issuer="${JWT_ISSUER}" \
  jwt_audience="${JWT_AUDIENCE}" \
  db_connection_string="Host=postgres;Port=5432;Database=${POSTGRES_DB:-smart_platform};Username=${POSTGRES_USER:-postgres};Password=${POSTGRES_PASSWORD}"

echo "==> Writing secret/data/services/payment"
vault kv put secret/services/payment \
  stripe_secret_key="${STRIPE_SECRET_KEY}" \
  stripe_webhook_secret="${STRIPE_WEBHOOK_SECRET}" \
  db_connection_string="Host=postgres;Port=5432;Database=${POSTGRES_DB:-smart_platform};Username=${POSTGRES_USER:-postgres};Password=${POSTGRES_PASSWORD}"

echo "==> Writing secret/data/services/notification"
vault kv put secret/services/notification \
  twilio_auth_token="${TWILIO_AUTH_TOKEN}" \
  twilio_account_sid="${TWILIO_ACCOUNT_SID}" \
  sendgrid_api_key="${SENDGRID_API_KEY}" \
  db_connection_string="Host=postgres;Port=5432;Database=${POSTGRES_DB:-smart_platform};Username=${POSTGRES_USER:-postgres};Password=${POSTGRES_PASSWORD}"

echo "==> Writing secret/data/services/fleet"
vault kv put secret/services/fleet \
  google_maps_api_key="${GOOGLE_MAPS_API_KEY}" \
  db_connection_string="Host=postgres;Port=5432;Database=${POSTGRES_DB:-smart_platform};Username=${POSTGRES_USER:-postgres};Password=${POSTGRES_PASSWORD}" \
  postgis_connection_string="Host=postgis;Port=5432;Database=${POSTGIS_DB:-smart_gis};Username=${POSTGIS_USER:-postgres};Password=${POSTGIS_PASSWORD}"

echo "==> Writing secret/data/services/gps"
vault kv put secret/services/gps \
  db_connection_string="Host=postgis;Port=5432;Database=${POSTGIS_DB:-smart_gis};Username=${POSTGIS_USER:-postgres};Password=${POSTGIS_PASSWORD}"

echo "==> Writing secret/data/services/ais-vessel"
vault kv put secret/services/ais-vessel \
  ais_api_key="${AIS_API_KEY}" \
  db_connection_string="Host=postgres;Port=5432;Database=${POSTGRES_DB:-smart_platform};Username=${POSTGRES_USER:-postgres};Password=${POSTGRES_PASSWORD}"

echo "==> Writing secret/data/services/anpr"
vault kv put secret/services/anpr \
  anpr_api_key="${ANPR_API_KEY}" \
  db_connection_string="Host=postgres;Port=5432;Database=${POSTGRES_DB:-smart_platform};Username=${POSTGRES_USER:-postgres};Password=${POSTGRES_PASSWORD}"

echo "==> Writing secret/data/services/customs"
vault kv put secret/services/customs \
  db_connection_string="Host=postgres;Port=5432;Database=${POSTGRES_DB:-smart_platform};Username=${POSTGRES_USER:-postgres};Password=${POSTGRES_PASSWORD}"

echo "==> Writing secret/data/services/shipping"
vault kv put secret/services/shipping \
  db_connection_string="Host=postgres;Port=5432;Database=${POSTGRES_DB:-smart_platform};Username=${POSTGRES_USER:-postgres};Password=${POSTGRES_PASSWORD}"

echo "==> Writing secret/data/services/analytics"
vault kv put secret/services/analytics \
  db_connection_string="Host=postgres;Port=5432;Database=${POSTGRES_DB:-smart_platform};Username=${POSTGRES_USER:-postgres};Password=${POSTGRES_PASSWORD}" \
  mongo_connection_string="mongodb://${MONGO_USER:-root}:${MONGO_PASSWORD}@mongodb:27017"

echo "==> Writing secret/data/services/audit"
vault kv put secret/services/audit \
  mongo_connection_string="mongodb://${MONGO_USER:-root}:${MONGO_PASSWORD}@mongodb:27017"

echo "==> Writing secret/data/services/declaration"
vault kv put secret/services/declaration \
  db_connection_string="Host=postgres;Port=5432;Database=${POSTGRES_DB:-smart_platform};Username=${POSTGRES_USER:-postgres};Password=${POSTGRES_PASSWORD}"

echo "==> Writing secret/data/services/corridor"
vault kv put secret/services/corridor \
  db_connection_string="Host=postgis;Port=5432;Database=${POSTGIS_DB:-smart_gis};Username=${POSTGIS_USER:-postgres};Password=${POSTGIS_PASSWORD}"

echo "==> Writing secret/data/services/dryport"
vault kv put secret/services/dryport \
  db_connection_string="Host=postgres;Port=5432;Database=${POSTGRES_DB:-smart_platform};Username=${POSTGRES_USER:-postgres};Password=${POSTGRES_PASSWORD}"

echo "==> Writing secret/data/services/ectn"
vault kv put secret/services/ectn \
  db_connection_string="Host=postgres;Port=5432;Database=${POSTGRES_DB:-smart_platform};Username=${POSTGRES_USER:-postgres};Password=${POSTGRES_PASSWORD}"

echo "==> Writing secret/data/services/integration"
vault kv put secret/services/integration \
  db_connection_string="Host=postgres;Port=5432;Database=${POSTGRES_DB:-smart_platform};Username=${POSTGRES_USER:-postgres};Password=${POSTGRES_PASSWORD}"

# ─────────────────────────────────────────────────────────────────────────────
# Observability Secrets
# ─────────────────────────────────────────────────────────────────────────────
echo "==> Writing secret/data/observability/grafana"
vault kv put secret/observability/grafana \
  admin_password="${GRAFANA_ADMIN_PASSWORD}"

echo ""
echo "✅ All SmartTrade secrets written to Vault."
echo "   Vault UI:  http://localhost:8200"
echo "   Token:     \${VAULT_DEV_ROOT_TOKEN} (dev mode only — never use in prod)"
