# Platform DevSecOps Inventory

## 1. CI/CD Pipeline (Target Architecture)
- **CI Server**: Jenkins (Pipeline as Code via `Jenkinsfile`).
- **Container Registry**: Secure Private Registry.
- **GitOps Controller**: ArgoCD.

## 2. Infrastructure as Code (IaC)
- **Provisioning**: Terraform (Network, Kubernetes clusters, managed Postgres databases).
- **Configuration**: Ansible (for baseline OS hardening on worker nodes, if applicable).

## 3. Security Scanning & Compliance
- **SAST**: Roslyn Analyzers (.NET) & ESLint/SonarQube (React).
- **Secret Scanning**: Gitleaks (pre-commit and CI boundary).
- **Container Scanning**: Trivy (scans image layers before push to registry).
- **IaC Scanning**: Checkov/Terrascan (scans Terraform modules for misconfigurations).
- **SBOM**: Syft (generates SPDX/CycloneDX for supply chain verification).

## 4. Secret Management
- **Primary Vault**: HashiCorp Vault.
- **Kubernetes Integration**: Vault Agent Injector / External Secrets Operator to prevent plaintext Kubernetes `Secret` resources in Git.
