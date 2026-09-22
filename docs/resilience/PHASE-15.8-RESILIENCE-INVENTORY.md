# Platform Resilience Inventory

## 1. Retry Policies (Polly)
- **Database (EF Core)**: Uses `EnableRetryOnFailure()` for transient SQL connection errors.
- **Messaging (MassTransit)**: Configured with `UseMessageRetry(r => r.Interval(3, TimeSpan.FromSeconds(5)))`.
- **External Providers (Payment)**: Uses Exponential Backoff with Jitter for external HTTP calls to prevent retry storms on provider failure.

## 2. Circuit Breakers
- **API Gateway (YARP)**: Rejects traffic to backend pools that fail consecutive health checks, failing fast for the client.
- **Customs to Fleet**: Circuit breaker triggers after 50% failure rate over 30 seconds, returning degraded cache state where permitted.

## 3. Data Durability & Idempotency
- **Outbox Pattern**: All 14 microservices use EF Core Outbox (`OutboxMessage.cs`) to guarantee atomicity between Postgres business state and RabbitMQ event publishing.
- **Inbox Pattern**: Consumers check `InboxMessage` table to ensure duplicate RabbitMQ deliveries (at-least-once guarantee) do not result in double ledger entries or duplicate customs clearance.

## 4. Kubernetes Resilience
- **Probes**: Liveness (TCP), Readiness (HTTP `/health`), and Startup probes configured for all API pods.
- **PodDisruptionBudget (PDB)**: Ensures minimum 1 replica available during node drains.
