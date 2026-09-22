# DevSecOps Runbook

## CI Pipeline Stages (Quality Gates)
1. **Checkout & Secret Scan**: Gitleaks fails build if secrets are detected.
2. **Build & Unit Tests**: .NET `dotnet build` and `dotnet test`.
3. **SAST**: Static analysis fails on Critical/High findings.
4. **Container Build**: Multi-stage Dockerfile builds minimal runtime image.
5. **Trivy Container Scan**: Fails pipeline on Critical/High CVEs.
6. **SBOM Generation**: Output pushed as Jenkins artifact.
7. **Image Push**: Secure push to Private Registry.
8. **GitOps Update**: Jenkins commits updated image tag/digest to the ArgoCD target repository.

## Vault Secret Rotation Strategy
- Database credentials rotated via Vault Dynamic Secrets.
- JWT Signing keys rotated with overlapping validation windows to prevent sudden session invalidation.
