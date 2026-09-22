# Rollback Runbook

## ArgoCD Rollback
1. **Git Revert**: The primary rollback mechanism is to revert the GitOps repository commit to the previous stable state.
2. **Sync**: ArgoCD detects the revert and syncs Kubernetes back to the N-1 image digest.

## Database Rollback Considerations
- **CRITICAL**: Application rollbacks MUST NOT trigger automatic database schema down-migrations. Down-migrations often involve destructive data loss.
- **Backward Compatibility**: Migrations must follow Expand/Contract patterns so that N-1 code can safely run against N schema during an emergency rollback.
