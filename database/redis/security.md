# Redis Security Policy

- NO plaintext tokens, passwords, private keys, or raw payment details are allowed in Redis.
- Access Control Lists (ACLs) restrict microservices exclusively to their required namespaces (e.g., Fleet service -> `~smart:fleet:*`).
- TLS is mandatory in-transit.
- Secrets mapping for connecting to Redis must leverage Vault (`secret/platform/redis/services/{service}`).
