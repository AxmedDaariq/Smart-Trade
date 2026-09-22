# Production Architecture & Kubernetes Hardening

## 1. Environments & Namespaces
- `platform-dev`
- `platform-test`
- `platform-staging`
- `platform-performance`
- `platform-production`

## 2. Kubernetes Network Policies (Default DENY)
- **Ingress**: External traffic is ONLY permitted to the YARP API Gateway Pods.
- **Service-to-Service**: Gateway may route to domain services. Domain services may communicate with internal Redis, PostgreSQL, and RabbitMQ exclusively via explicitly allowed egress rules.
- **Public Exposure**: PostgreSQL, Redis, and RabbitMQ are strictly isolated to internal cluster networking.

## 3. Pod Security Standards
All application pods enforce the **Restricted** profile:
- `runAsNonRoot: true`
- `allowPrivilegeEscalation: false`
- `readOnlyRootFilesystem: true` (where possible)
- Drops all default Linux capabilities.

## 4. Autoscaling (HPA) & Disruption (PDB)
- **HPA**: Configured for Gateway, Control Tower BFF, and high-volume consumers (GPS/ANPR) based on Phase 15.7 load test targets.
- **PDB**: Minimum availability set to ensure rolling updates or node drains do not drop critical workloads below 50% capacity.
