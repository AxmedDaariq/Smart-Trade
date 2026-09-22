# Phase 15.6 Security Baseline

## 1. Zero Trust Architecture
The Smart Trade & Logistics Corridor Platform operates on a strict Zero Trust model. Internal network location (e.g., executing within the Kubernetes cluster) does NOT confer trust. 

All service-to-service communication, including MassTransit/RabbitMQ integration events and internal REST API calls, MUST carry cryptographic proof of identity and tenant context (e.g., `X-Correlation-ID`, `X-Tenant-Id` derived from a validated OAuth2 JWT).

## 2. Trust Boundaries
- **Untrusted (Internet)**: Driver Mobile App, ANPR Cameras, External Payment Providers, Browser Clients.
- **Verification (Edge)**: API Gateway (YARP). Validates JWT signature, expiration, and issuer. Rejects malformed requests before routing.
- **Authorization (BFF & Domain)**: Control Tower BFF, Domain Microservices. Evaluate claims (Role, Permission, Tenant) against the requested resource. Frontend authorization (`RequirePermission.tsx`) is strictly for UX, not security.
- **Core (Database/Event Bus)**: PostgreSQL, RabbitMQ. Protected by mutual TLS (mTLS) and least-privilege service accounts.

## 3. Structural SAST Findings (from Scaffolding)
- `IIntegrationEvent.cs`: **PASS**. Strictly requires `TenantId`, preventing ambient cross-tenant contamination in the event stream.
- `RequirePermission.tsx`: **PASS**. Correctly assumes frontend is not the security boundary.
- `ControlTowerHub.cs`: **PASS**. Correctly partitions WebSocket connections by `Tenant_{tenantId}` during `OnConnectedAsync` to prevent SignalR event leakage.
