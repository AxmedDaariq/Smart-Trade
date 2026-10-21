# Policy: notification-service
path "secret/data/services/notification" {
  capabilities = ["read"]
}
path "secret/metadata/services/notification" {
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
