# 🚀 Smart Trade Logistics Platform — Enterprise DevOps Architecture Guide

Mashruucan waxaan u qaabaynay qaab buuxa oo ah **Enterprise-Grade DevOps & DevSecOps Platform**, kaasoo u diyaarsan shirkadaha waaweyn ee caalamiga ah (Fortune 500 / FinTech / Logistics Standards).

---

## 1. 🏗️ Guudmarka Naqshadda (Architecture Overview)

```
        Git Repository (Push / PR / Tag)
                    │
                    ▼
     ┌────────────────────────────────────────────────────────┐
     │ 🚀 GitHub Actions Enterprise Workflows                 │
     │  - ci.yml              (Lint, Matrix Test, CodeQL,     │
     │                         Gitleaks, Trivy, Checkov)      │
     │  - cd.yml              (OIDC Keyless, Multi-Arch,      │
     │                         Cosign Signing, GitOps Push)   │
     │  - terraform.yml       (Remote S3/DynamoDB, Per-Env)   │
     │  - security-audit.yml  (Weekly Automated Scans)        │
     └────────────────────────────────────────────────────────┘
                    │
                    ▼
     ┌────────────────────────────────────────────────────────┐
     │ 🐳 AWS ECR & Cosign Registry                           │
     │  - Private Immutable Repositories                      │
     │  - Cryptographic Cosign Image Signatures               │
     │  - Automated KMS at-rest encryption & scan-on-push     │
     └────────────────────────────────────────────────────────┘
                    │
                    ▼
     ┌────────────────────────────────────────────────────────┐
     │ 🔄 ArgoCD (GitOps Engine)                              │
     │  - ApplicationSet (Dev / Staging / Production)         │
     │  - Zero-drift self-healing & automated pruning         │
     │  - Slack Notifications                                 │
     └────────────────────────────────────────────────────────┘
                    │
                    ▼
     ┌────────────────────────────────────────────────────────┐
     │ ☸️ Kubernetes (AWS EKS 1.30) Enterprise Cluster        │
     │  - Hardened Helm Charts (HPA, PDB, Zero-Trust NetPol)  │
     │  - Kyverno Pod Security Standards (Non-root, Drop-ALL) │
     │  - Full Observability (Prometheus, Grafana, Loki)      │
     │  - HashiCorp Vault Secrets Injection                   │
     └────────────────────────────────────────────────────────┘
```

---

## 2. 🛡️ Amniga & DevSecOps (Security Best Practices)

1. **OIDC Authentication (No Static AWS Keys):**
   - Ma jiro AWS Access Key iyo Secret Key ku kaydsan GitHub Secrets.
   - Waxaa la isticmaalayaa AWS IAM OIDC Provider oo si ku meel gaar ah u siiya GitHub Actions `AssumeRoleWithWebIdentity`.
2. **Cosign Image Signing:**
   - Sawir kasta oo Docker ah (Container image) waxaa lagu saxiixaa furayaasha OIDC ee Sigstore/Cosign kahor intaan loo daadgureynin Production.
3. **Hardened Helm Deployments:**
   - **Pod Security**: `runAsNonRoot: true`, `readOnlyRootFilesystem: true`, `drop: ["ALL"]`.
   - **Zero-Trust NetworkPolicy**: Dhammaan adeegyadu ma wada xiriiri karaan haddii aan si qeexan loogu oggolaan NetPol.
   - **Kyverno Admission Controller**: Wuxuu xannibaa pod kasta oo aan buuxin shuruudaha amniga.

---

## 3. ⚙️ Qaab-dhismeedka Terraform (IaC)

Waxaan u habeynay qaab Modular ah oo leh xaaladaha kala duwan (**Environments**):
- [modules/vpc/](file:///c:/Users/Tariq/.gemini/antigravity-ide/scratch/smart-trade-logistics-platform/infrastructure/terraform/modules/vpc/main.tf) - Shabakadda VPC, Subnets, NAT Gateways & EKS Tags.
- [modules/eks/](file:///c:/Users/Tariq/.gemini/antigravity-ide/scratch/smart-trade-logistics-platform/infrastructure/terraform/modules/eks/main.tf) - Kubernetes cluster, Node Groups, KMS Encryption & IRSA.
- [modules/rds/](file:///c:/Users/Tariq/.gemini/antigravity-ide/scratch/smart-trade-logistics-platform/infrastructure/terraform/modules/rds/main.tf) - Database PostgreSQL (Multi-AZ, Storage Auto-scaling, Secrets Manager).
- [modules/elasticache/](file:///c:/Users/Tariq/.gemini/antigravity-ide/scratch/smart-trade-logistics-platform/infrastructure/terraform/modules/elasticache/main.tf) - Redis Cluster (Transit TLS & At-rest encryption).
- [modules/ecr/](file:///c:/Users/Tariq/.gemini/antigravity-ide/scratch/smart-trade-logistics-platform/infrastructure/terraform/modules/ecr/main.tf) - 17 Private ECR Repositories oo leh Lifecycle Policies.
- [modules/iam-github-oidc/](file:///c:/Users/Tariq/.gemini/antigravity-ide/scratch/smart-trade-logistics-platform/infrastructure/terraform/modules/iam-github-oidc/main.tf) - IAM OIDC trust relationship.

---

## 4. 📈 Kormeerka & Ogaanshaha Ciladaha (Observability)

- **Prometheus Stack**: [values-prometheus-stack.yaml](file:///c:/Users/Tariq/.gemini/antigravity-ide/scratch/smart-trade-logistics-platform/infrastructure/helm/monitoring/values-prometheus-stack.yaml) wuxuu bixiyaa HA Prometheus (2 replicas), Alertmanager ku xiran Slack/PagerDuty, iyo Grafana Dashboards.
- **ServiceMonitor**: Si toos ah ayuu adeeg kasta ugu daraa dashboard-ka Prometheus isagoo xogta ka akhrinaya `/metrics`.

---

## 5. 🚀 Tallaabooyinka Bilowga (Quick Start Commands)

### A. Diyaarinta Terraform State (S3 & DynamoDB):
```bash
cd infrastructure/terraform
chmod +x bootstrap-state.sh
./bootstrap-state.sh dev
./bootstrap-state.sh production
```

### B. Orodka Terraform (Dev):
```bash
cd infrastructure/terraform/environments/dev
terraform init -backend-config="bucket=smart-trade-tfstate-dev" \
               -backend-config="key=dev/terraform.tfstate" \
               -backend-config="region=us-east-1" \
               -backend-config="dynamodb_table=smart-trade-tfstate-lock-dev"
terraform plan
terraform apply
```

### C. Dhaqaajinta ArgoCD GitOps:
```bash
kubectl apply -f infrastructure/argocd/projects/smart-trade-project.yaml
kubectl apply -f infrastructure/argocd/applicationset-services.yaml
kubectl apply -f infrastructure/argocd/notifications.yaml
```
