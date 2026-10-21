# Shared policy — grants read access to any service that only needs the shared
# JWT config, Postgres, Redis, RabbitMQ, and its own secrets path.
# Individual service policies extend this via per-service HCL files.

path "secret/data/shared/*" {
  capabilities = ["read"]
}
path "secret/metadata/shared/*" {
  capabilities = ["read"]
}
path "secret/data/databases/*" {
  capabilities = ["read"]
}
path "secret/metadata/databases/*" {
  capabilities = ["read"]
}
path "secret/data/messaging/*" {
  capabilities = ["read"]
}
path "secret/metadata/messaging/*" {
  capabilities = ["read"]
}
