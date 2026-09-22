# Production Deployment Runbook

## Deployment Execution (GitOps via ArgoCD)
1. **Approval**: CI pipeline successfully updates the GitOps manifest repo. Pull Request to `production` branch is approved by Tech Lead.
2. **Sync**: ArgoCD detects the change on the `production` branch and triggers a sync.
3. **Rollout**: Kubernetes executes a RollingUpdate. 
4. **Probes**: Readiness probes hold traffic until new pods initialize dependencies (Redis, Postgres).
5. **Database Migrations**: Executed as a pre-sync hook job or dedicated init-container strategy. Must be backward-compatible with the running N-1 application version.
6. **Smoke Test**: Automated post-deploy validation (Auth check, critical API health).
