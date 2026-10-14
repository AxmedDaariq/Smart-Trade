# 🚢 Smart Trade Logistics Platform
### Enterprise-Grade Microservices Architecture

![Build Status](https://img.shields.io/badge/Build-Passing-brightgreen)
![Security](https://img.shields.io/badge/Security-OWASP_Top_10_Compliant-blue)
![Framework](https://img.shields.io/badge/.NET-10.0-purple)
![Infrastructure](https://img.shields.io/badge/K8s-Production_Ready-orange)

## 📖 Overview
The **Smart Trade Logistics Platform** is a high-scale, resilient ecosystem designed to manage the entire lifecycle of international trade. From user identity and ECTN issuance to customs clearance and fleet dispatch, the platform ensures a seamless, secure, and transparent flow of goods across borders.

## 🏛️ Enterprise Architecture

### 1. Microservices Ecosystem
The platform is decomposed into specialized services, each with its own Domain, Application, Infrastructure, and API layer:
- **Identity Service**: User management, RBAC, and KYC verification.
- **ECTN Service**: Electronic Cargo Tracking Note issuance and validation.
- **Customs Service**: HS Code classification, duty calculation, and legal clearance.
- **Shipping Service**: Order management and logistics orchestration.
- **Fleet Service**: Vehicle tracking, driver assignment, and dispatching.
- **Payment Service**: Invoice generation and secure payment processing.

### 2. Data Strategy
- **Relational (PostgreSQL)**: Used for ACID-compliant transactions (Orders, Payments, Customs).
- **NoSQL (MongoDB)**: Used for high-volume audit logs, notifications, and caching.
- **Optimization**: Implemented Concurrent Indexing and TTL strategies for sub-second query performance.

### 3. Infrastructure & DevOps
- **Orchestration**: Kubernetes (K8s) with Readiness/Liveness probes for self-healing.
- **Resilience**: Chaos Engineering implemented via Chaos Mesh to test pod and network failures.
- **BDR**: Automated Backup and Disaster Recovery with checksum validation.

---

## 🧪 The Quality Assurance Pyramid
We employ a "Defense in Depth" testing strategy to ensure 99.99% reliability.

| Level | Tool | Scope | Purpose |
| :--- | :--- | :--- | :--- |
| **Unit** | xUnit | Logic | Verify individual functions. |
| **Integration** | Testcontainers | Service $\leftrightarrow$ DB | Verify real DB interactions. |
| **E2E** | Cypress | User $\rightarrow$ UI $\rightarrow$ API | Verify business journeys. |
| **Performance** | JMeter | Load / Stress | Verify scalability under pressure. |
| **Chaos** | Chaos Mesh | Fault Injection | Verify system resilience. |
| **Security** | OWASP ZAP | Vuln Scanning | Protect against Top 10 attacks. |
| **Infrastructure**| Bash/Python | Network/K8s | Verify cloud availability. |

---

## 🚀 CI/CD Pipeline
Our **GitHub Actions** pipeline automates the entire path to production:

**Build** $\rightarrow$ **Unit Tests** $\rightarrow$ **Integration Tests** $\rightarrow$ **E2E Tests** $\rightarrow$ **Security Scan** $\rightarrow$ **Perf Audit** $\rightarrow$ **K8s Deploy** $\rightarrow$ **Health Check**

---

## 🛠️ Getting Started

### Prerequisites
- .NET 10.0 SDK
- Docker Desktop (for Testcontainers & Chaos Mesh)
- Node.js & NPM (for Cypress)
- Apache JMeter (for Performance tests)

### Running the Suite
```bash
# Run all unit tests
dotnet test tests/01-unit

# Run integration tests (Requires Docker)
dotnet test tests/02-integration

# Run E2E tests
cd tests/03-e2e/cypress && npm install && npm run cy:run

# Run Performance Stress Test
jmeter -n -t tests/04-performance/jmeter/scripts/stress_test_platform.jmx -l results.jtl

# Run Security Scan
bash tests/05-security/owasp-zap/zap-top10-scan.sh
```

## 🔒 Security Compliance
This platform is designed to meet international trade security standards:
- **OWASP Top 10** compliant.
- **Role-Based Access Control (RBAC)** strictly enforced.
- **Data Encryption** for sensitive KYC documents.
- **Audit Trails** for every regulatory change.

---
**Developed for Enterprise Logistics Excellence.**
