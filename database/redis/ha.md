# High Availability (HA) Policy

- Production deployments require a managed Redis cluster or Redis Sentinel topology.
- Single-node Redis containers are strictly isolated to local Docker Compose workflows.
- Kubernetes operators ensure Pod Disruption Budgets (PDB) and health probes.
