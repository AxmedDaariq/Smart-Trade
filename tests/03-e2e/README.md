# Smart Trade Logistics — E2E Tests (Cypress)

End-to-end test suite covering Frontend, Backend APIs, Databases (PostgreSQL, MongoDB, Redis, RabbitMQ), Mobile, and Full-Stack corridor flows.

## Prerequisites

- **Node.js 20+** (or 22+ recommended)
- **Docker** — for running backing services locally

## Quick Start

```bash
# 1. Start backing services
docker compose -f docker-compose.dev.yml up -d

# 2. Install dependencies
cd tests/03-e2e/cypress
npm install

# 3. Copy and configure environment
cp cypress.env.json.example cypress.env.json
# Edit cypress.env.json with your local URLs / credentials

# 4. Open Cypress UI (interactive)
npm run cy:open

# 5. Run all tests headless
npm run cy:run
```

## Test Suites

| Script | Coverage |
|---|---|
| `npm run cy:run:api` | Backend API (identity, gateway, microservices) |
| `npm run cy:run:web` | Frontend UI (auth, dashboard, customs, fleet) |
| `npm run cy:run:db` | PostgreSQL, MongoDB, Redis, RabbitMQ |
| `npm run cy:run:mobile` | Driver cockpit, ePOD mobile flows |
| `npm run cy:run:full` | Full corridor lifecycle + multi-tenant isolation |
| `npm run cy:ci` | All tests — Electron headless (CI mode) |

## CI / GitHub Actions

The workflow at `.github/workflows/e2e.yml` runs 4 parallel matrix jobs:

- `api` / `frontend` / `database` / `full-stack`

Each job spins up PostgreSQL, MongoDB, Redis, and RabbitMQ service containers automatically.

### Required GitHub Secrets (optional — defaults to dev values)

| Secret | Description |
|---|---|
| `E2E_ADMIN_EMAIL` | Admin user email |
| `E2E_ADMIN_PASS` | Admin user password |
| `E2E_DRIVER_EMAIL` | Driver user email |
| `E2E_DRIVER_PASS` | Driver user password |

### Required GitHub Variables (optional)

| Variable | Default |
|---|---|
| `E2E_BASE_URL` | `http://localhost:5173` |
| `E2E_API_URL` | `http://localhost:5000` |
| `E2E_GATEWAY` | `http://localhost:8080` |