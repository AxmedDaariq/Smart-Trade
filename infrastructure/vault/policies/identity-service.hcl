# SmartTrade Logistics Platform — Per-Service Vault Policies
# ─────────────────────────────────────────────────────────────────────────────
# Policy: identity-service
# Grants the Identity API read access to its own secrets and the shared JWT
# config.  No write access is ever granted to services.
#
# Apply: vault policy write identity-service identity-service.hcl
# ─────────────────────────────────────────────────────────────────────────────

path "secret/data/services/identity" {
  capabilities = ["read"]
}

path "secret/metadata/services/identity" {
  capabilities = ["read"]
}

path "secret/data/shared/jwt" {
  capabilities = ["read"]
}

path "secret/metadata/shared/jwt" {
  capabilities = ["read"]
}

path "secret/data/databases/postgres" {
  capabilities = ["read"]
}

path "secret/data/messaging/rabbitmq" {
  capabilities = ["read"]
}
