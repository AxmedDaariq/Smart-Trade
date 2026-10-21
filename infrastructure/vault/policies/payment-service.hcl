# Policy: payment-service
path "secret/data/services/payment" {
  capabilities = ["read"]
}
path "secret/metadata/services/payment" {
  capabilities = ["read"]
}
path "secret/data/shared/jwt" {
  capabilities = ["read"]
}
path "secret/data/databases/postgres" {
  capabilities = ["read"]
}
path "secret/data/messaging/rabbitmq" {
  capabilities = ["read"]
}
