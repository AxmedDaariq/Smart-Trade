#!/bin/bash
# Kani waa Script u qaabaysan Enterprise oo lagu rakibayo Policies, 
# Kubernetes Auth (bedelka AppRole ee k8s), iyo Dynamic Database Credentials.

# Hubi in DB_ADMIN_PASSWORD ay jirto
if [ -z "$DB_ADMIN_PASSWORD" ]; then
    echo "Error: DB_ADMIN_PASSWORD environment variable is not set."
    exit 1
fi

# 1. Samaynta Vault Policies
echo "Abuurista Vault Policies..."
vault policy write customs-api-policy - <<EOF
# Oggolow in la aqriyo siraha rasmiga ah (Static Secrets)
path "secret/data/customs-api/*" {
  capabilities = ["read"]
}
# Oggolow in la aqriyo Dynamic Database Credentials
path "database/creds/customs-db-role" {
  capabilities = ["read"]
}
EOF

# 2. Shidida Kubernetes Auth Method (Waa kan ugu fiican Kubernetes, bedel u ah AppRole)
echo "Shidida Kubernetes Authentication..."
vault auth enable kubernetes

# Isku xirka Vault iyo Kubernetes API-ga
vault write auth/kubernetes/config \
    kubernetes_host="https://\$KUBERNETES_PORT_443_TCP_ADDR:443"

# Abuurista Role-ka u oggolaanaya 'customs-api' Service Account inuu soo qaato sirta
vault write auth/kubernetes/role/customs-api-role \
    bound_service_account_names=customs-api-sa \
    bound_service_account_namespaces=default \
    policies=customs-api-policy \
    ttl=1h

# (Ikhtiyaari) 3. Haddii aad dhab ahaan u baahan tahay AppRole adeegyada dibadda ah (External Services)
echo "Shidida AppRole ee adeegyada ka baxsan Kubernetes..."
vault auth enable approle
vault write auth/approle/role/external-api-role \
    token_policies="customs-api-policy" \
    secret_id_ttl=10m \
    token_ttl=20m

# 4. Habaynta Dynamic Database Credentials (PostgreSQL tusaale ahaan)
echo "Shidida Dynamic Database Secrets Engine..."
vault secrets enable database

# Isku xirka Vault iyo Database-ka
vault write database/config/my-postgresql-database \
    plugin_name=postgresql-database-plugin \
    allowed_roles="customs-db-role" \
    connection_url="postgresql://{{username}}:{{password}}@prod-db.smart-trade.com:5432/postgres" \
    username="admin" \
    password="${DB_ADMIN_PASSWORD}"

# Abuurista Role u qaabilsan inuu abuuro Dynamic Credentials oo dhacaya (Expire) 1 saac kadib
vault write database/roles/customs-db-role \
    db_name=my-postgresql-database \
    creation_statements="CREATE ROLE \"{{name}}\" WITH LOGIN PASSWORD '{{password}}' VALID UNTIL '{{expiration}}'; GRANT SELECT ON ALL TABLES IN SCHEMA public TO \"{{name}}\";" \
    default_ttl="1h" \
    max_ttl="24h"

echo "Hambalyo! Vault Policy, Kubernetes Auth, AppRole, iyo Dynamic Database Credentials si buuxda ayaa loo habeeyay."
