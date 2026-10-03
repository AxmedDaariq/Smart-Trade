# 🚚 Smart Trade Logistics Platform

<div align="center">

![Platform](https://img.shields.io/badge/Platform-.NET%208%20%7C%20TypeScript-blueviolet?style=for-the-badge)
![Docker](https://img.shields.io/badge/Docker-Compose-2496ED?style=for-the-badge&logo=docker)
![License](https://img.shields.io/badge/License-MIT-green?style=for-the-badge)
![CI](https://img.shields.io/badge/CI-GitHub%20Actions-181717?style=for-the-badge&logo=github-actions)

**An enterprise-grade, cloud-native trade logistics platform built on a distributed micro-service architecture.**

[Features](#-features) • [Tech Stack](#-tech-stack) • [Getting Started](#-getting-started) • [Services](#-micro-services) • [Infrastructure](#-infrastructure) • [Testing](#-testing) • [Contributing](#-contributing)

</div>

---

## 🌍 Overview

Smart Trade Logistics Platform provides a unified, end-to-end solution for managing international trade operations. It covers the full logistics lifecycle — from shipment declaration and customs clearance, to real-time GPS tracking, fleet management, ANPR gate control, vessel AIS monitoring, and payment processing.

The system is designed for **scalability**, **resilience**, and **observability**, with full Kubernetes support via Helm and ArgoCD.

---

## ✨ Features

| Feature | Description |
|---|---|
| 🛰️ **GPS Telematics** | Real-time vehicle telemetry ingestion and tracking |
| 🚢 **AIS Vessel Tracking** | Automatic Identification System integration for maritime logistics |
| 🛃 **Customs Management** | Full lifecycle customs case processing and declarations |
| 📦 **ECTN Management** | Electronic Cargo Tracking Note generation and validation |
| 🚗 **ANPR Gate Control** | Automatic Number Plate Recognition for gate entry/exit |
| 🚛 **Fleet Management** | Driver and vehicle fleet operations |
| 💳 **Payment Processing** | Secure payment intent creation and management |
| 🔔 **Notifications** | Multi-channel notification dispatch (email, SMS, push) |
| 📊 **Analytics** | Business intelligence and reporting service |
| 🔐 **Identity & Auth** | Centralized authentication and authorization (JWT + HashiCorp Vault) |
| 🔗 **Corridor Management** | Trade corridor routing and logistics lane tracking |
| 🔄 **Integration Hub** | External third-party system integrations |
| 📝 **Audit Log** | Immutable audit trail for all platform actions |

---

## 🛠️ Tech Stack

### Backend
- **.NET 8 / C#** — ASP.NET Core Web API, minimal APIs
- **Entity Framework Core** — ORM & database migrations
- **MediatR** — CQRS pattern (Commands / Queries / Handlers)
- **FluentValidation** — Request validation
- **gRPC / HTTP** — Service-to-service communication

### Frontend & Mobile
- **Web App** (`apps/web`) — TypeScript-based web client
- **Admin Tools** (`apps/admin-tools`) — Back-office management portal
- **Driver Mobile** (`apps/driver-mobile`) — Mobile app for drivers

### Infrastructure & DevOps
- **Docker / Docker Compose** — Local dev environment
- **Kubernetes + Helm** — Production-grade container orchestration
- **ArgoCD** — GitOps continuous delivery
- **Terraform** — Infrastructure as Code
- **Ansible** — Configuration management
- **Jenkins / GitHub Actions** — CI/CD pipelines
- **HashiCorp Vault** — Secrets management
- **Prometheus + Grafana** — Metrics and monitoring
- **Jaeger / OpenTelemetry** — Distributed tracing
- **Elasticsearch** — Centralized logging

---

## 🚀 Getting Started

### Prerequisites

| Tool | Version |
|---|---|
| [Docker Desktop](https://www.docker.com/products/docker-desktop) | Latest |
| [.NET SDK](https://dotnet.microsoft.com/download) | 8.0+ |
| [Node.js](https://nodejs.org/) | 20+ |
| [PowerShell](https://github.com/PowerShell/PowerShell) | 7+ (Windows) |

### Installation

```bash
# 1. Clone the repository
git clone https://github.com/your-org/smart-trade-logistics-platform.git
cd smart-trade-logistics-platform

# 2. Copy environment config
cp .env.example .env   # edit values as needed

# 3. Build all Docker images
docker compose build
```

### Running the Application

```bash
# Start all core services
docker compose up -d

# Start with observability stack (Prometheus, Grafana, Jaeger)
docker compose -f docker-compose.yml -f docker-compose.observability.yml up -d

# Check service health
curl http://localhost:5000/health
```

### Database Migrations

```powershell
# Windows
./run-migrations.ps1

# Linux / macOS
./run-migrations.sh
```

---

## 📦 Micro-Services

```
services/
├── ais/              # AIS maritime vessel tracking
├── ais-vessel/       # Vessel data enrichment
├── analytics/        # Reporting & business intelligence
├── anpr/             # ANPR plate recognition engine
├── anpr-gate/        # Gate entry/exit controller
├── audit/            # Audit log service
├── corridor/         # Trade corridor management
├── customs/          # Customs cases & declarations
├── declaration/      # Shipment declaration service
├── ectn/             # ECTN generation & validation
├── fleet/            # Fleet & driver management
├── gps/              # GPS data processor
├── gps-telematics/   # Real-time telemetry ingestion
├── identity/         # Auth, JWT, user management
├── integration/      # External integration hub
├── notification/     # Multi-channel notifications
├── payment/          # Payment intent & transactions
└── shipping/         # Shipping order management
```

Each service follows a **Clean Architecture** pattern:

```
ServiceName/
├── ServiceName.Api/           # HTTP controllers, gRPC endpoints
├── ServiceName.Application/   # CQRS handlers, commands, queries
├── ServiceName.Domain/        # Entities, value objects, domain events
├── ServiceName.Infrastructure/# DB, external integrations
└── ServiceName.Tests/         # Unit & integration tests
```

---

## 🏗️ Infrastructure

```
infrastructure/
├── argocd/       # ArgoCD GitOps app definitions
├── ansible/      # Server configuration playbooks
├── docker/       # Base Dockerfile templates
├── helm/         # Helm charts (per service + vault)
├── jenkins/      # Jenkins pipeline definitions
├── k8s/          # Raw Kubernetes manifests
├── kubernetes/   # Kubernetes cluster config
├── logging/      # ELK stack configuration
├── monitoring/   # Prometheus & Grafana dashboards
├── terraform/    # Cloud infrastructure IaC
├── tracing/      # Jaeger / OpenTelemetry config
└── vault/        # HashiCorp Vault Helm values
```

---

## 🧪 Testing

### .NET Services

```bash
# Run all service tests
dotnet test services/gps-telematics/GpsTelematics.Tests
dotnet test services/customs/Customs.Tests
dotnet test services/payment/Payment.Application.Tests
dotnet test tests/Ectn.UnitTests

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

### TypeScript / Frontend

```bash
cd tests/unit
npm install
npm test
```

### Integration Tests

```bash
docker compose -f docker-compose.test.yml up --abort-on-container-exit
```

---

## 🤝 Contributing

Contributions are welcome and encouraged! To get started:

1. **Fork** the repository
2. **Create** a feature branch
   ```bash
   git checkout -b feature/your-feature-name
   ```
3. **Write** tests for your changes
4. **Ensure** all tests pass
   ```bash
   dotnet test
   ```
5. **Commit** using [Conventional Commits](https://www.conventionalcommits.org/)
   ```bash
   git commit -m "feat(customs): add case status workflow"
   ```
6. **Open** a Pull Request with a clear description

### Code Style
- C# follows the `.editorconfig` rules in the root
- TypeScript follows ESLint/Prettier config
- All PRs must include unit tests

---

## 📄 License

This project is licensed under the **MIT License** — see the [LICENSE](LICENSE) file for details.

---

<div align="center">

Built with ❤️ by the Smart Trade team

</div>
