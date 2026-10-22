# Smart Trade Logistics — End-to-End (E2E) Test Suite

Welcome to the **Smart Trade Logistics Enterprise End-to-End (E2E) Test Suite** (`tests/03-e2e`).

This suite delivers 100% automated test coverage across all tiers of the platform:
- **Frontend Web Portal**: React 18, React Query, RBAC routing, Control Tower, Customs & ECTN, Dryport Demurrage.
- **Backend API Gateway & Microservices**: Ocelot Gateway, Identity, Shipping, Customs, ECTN, Fleet Telemetry, Payments, Notifications.
- **Database & Messaging Layer**:
  - **PostgreSQL**: EF Core migrations, multi-tenant isolation, seed data verification, and referential integrity.
  - **MongoDB**: High-frequency unstructured IoT & telematics telemetry (GPS breadcrumbs, cold-chain reefer sensor readings, smart e-seal tamper logs).
  - **Redis**: Low-latency cache, real-time vehicle GPS positions for live control tower map polling, and JWT/session token revocation blacklist.
  - **RabbitMQ**: Asynchronous corridor event bus, message queues (`shipment.dispatched`, `customs.cleared`, `fleet.emergency.sos`), and queue health.
- **Mobile Driver Client**: Handheld mobile viewport, mission HUD, GPS telematics, cold-chain reefer temp, IoT e-seals, and e-POD handover.
- **Full-Stack Scenarios**: Multi-service cross-corridor lifecycle (Berbera Port ⇄ Tog-Wajaale ⇄ Addis Ababa) and multi-tenant security isolation.

---

## 📁 Directory Architecture

```
tests/03-e2e/
├── README.md                                  # This documentation guide
├── cypress/                                   # Unified Enterprise Cypress Test Project
│   ├── cypress.config.js                      # Global Cypress config, DB/Mongo/Redis/Rabbit tasks, Mochawesome
│   ├── package.json                           # NPM scripts and dependencies
│   ├── cypress/
│   │   ├── e2e/
│   │   │   ├── frontend/                      # Web Portal E2E specs
│   │   │   │   ├── auth.cy.js                 # Multi-tenant login, registration, RBAC, logout
│   │   │   │   ├── customs-ectn.cy.js         # ECTN issuance, WCO risk channels, customs declaration
│   │   │   │   ├── logistics-dashboard.cy.js  # Logistics KPIs, corridor track, shipment filters
│   │   │   │   ├── shipping-fleet-payment.cy.js# Consignment dispatch, clearance & payments
│   │   │   │   └── control-tower-dryport.cy.js# Control Tower alerts & Dryport demurrage
│   │   │   ├── backend/                       # API integration & security specs
│   │   │   │   ├── gateway-api.cy.js          # Reverse proxy routing, CORS & rate limits
│   │   │   │   ├── identity-api.cy.js         # JWT token issuance, validation & roles
│   │   │   │   └── microservices-api.cy.js    # Shipping, Customs, ECTN, Fleet, Payments
│   │   │   ├── database/                      # Persistence & Messaging specs
│   │   │   │   ├── postgres-verification.cy.js# PostgreSQL DB connectivity, schema & row verification
│   │   │   │   ├── seed-migration.cy.js       # EF Core migrations, seed data & indexes
│   │   │   │   ├── mongodb-telematics.cy.js   # MongoDB unstructured IoT, GPS, Reefer & E-Seal telemetry
│   │   │   │   └── redis-rabbitmq-verification.cy.js # Redis cache/blacklist & RabbitMQ corridor broker
│   │   │   ├── mobile/                        # Field & Mobile Driver specs
│   │   │   │   ├── driver-cockpit.cy.js       # Handheld viewport, mission HUD & telemetry
│   │   │   │   └── manifest-epod.cy.js        # Waybill inspection, e-POD & offline sync
│   │   │   └── full-stack/                    # End-to-end corridor scenarios
│   │   │       ├── corridor-trade-lifecycle.cy.js # Berbera ⇄ Addis Ababa full lifecycle
│   │   │       └── multi-tenant-isolation.cy.js   # Cross-tenant boundary & header security
│   │   ├── fixtures/                          # Test payloads & mock data
│   │   │   ├── api-endpoints.json
│   │   │   ├── users.json
│   │   │   ├── shipments.json
│   │   │   ├── customs-and-payments.json
│   │   │   ├── mobile-driver.json
│   │   │   └── sample-passport.pdf
│   │   └── support/                           # Custom commands & hooks
│   │       ├── commands.js                    # Cypress commands implementation (DB, Mongo, Redis, RabbitMQ)
│   │       ├── commands.ts                    # TypeScript typings
│   │       └── e2e.js                         # Global setup & error handlers
└── web/ / mobile/ / scenarios/                # Sub-module references & stubs
```

---

## 🚀 Running the Tests

All tests are managed via npm scripts within `tests/03-e2e/cypress`:

```bash
cd tests/03-e2e/cypress
npm install
```

### Execution Profiles

| Command | Target Scope | Description |
| :--- | :--- | :--- |
| `npm run cy:run` | **All Specs** | Executes all E2E specs in headless mode |
| `npm run cy:run:web` | **Frontend + Gateway** | Executes Web Portal UI and Gateway tests |
| `npm run cy:run:api` | **Microservices** | Validates all backend APIs via Gateway |
| `npm run cy:run:db` | **All Databases** | Verifies PostgreSQL, MongoDB, Redis, and RabbitMQ |
| `npm run cy:run:mongo` | **MongoDB Telematics**| Tests unstructured IoT, GPS, and cold-chain collections |
| `npm run cy:run:redis-rabbit`| **Redis & RabbitMQ** | Tests caching, token blacklist, and corridor AMQP broker |
| `npm run cy:run:mobile` | **Driver Mobile** | Tests mobile viewport, telematics & e-POD |
| `npm run cy:run:full` | **Full Scenarios** | Runs full corridor trade & multi-tenant suites |
| `npm run cy:open` | **Interactive UI** | Opens the interactive Cypress Test Runner GUI |
| `npm run cy:report` | **HTML Report** | Compiles Mochawesome HTML report in `cypress/reports` |

---

## ⚙️ Environment Variables

The test runner auto-detects local development vs CI containers using environment variables:

| Variable | Default Value | Purpose |
| :--- | :--- | :--- |
| `CYPRESS_BASE_URL` | `http://localhost:5173` | Frontend Web Portal URL |
| `API_GATEWAY` / `GATEWAY_URL` | `http://localhost:8080` | Ocelot API Gateway URL |
| `IDENTITY_API` / `API_URL` | `http://localhost:5000` | Identity & Auth Service URL |
| `MOBILE_API` | `http://localhost:5000` | Mobile Driver API endpoint |
| `DB_HOST` | `localhost` | PostgreSQL Server Host |
| `DB_PORT` | `5432` | PostgreSQL Server Port |
| `DB_NAME` | `smart_trade` | Database Name |
| `DB_USER` | `postgres` | Database Username |
| `DB_PASS` | `postgres` | Database Password |
| `MONGO_URL` | `mongodb://localhost:27017/smart_trade` | MongoDB Connection URI (Telematics) |
| `REDIS_URL` | `redis://localhost:6379` | Redis In-Memory Cache URI |
| `RABBITMQ_URL` | `amqp://guest:guest@localhost:5672` | RabbitMQ AMQP Broker URI |
| `RABBITMQ_API` | `http://localhost:15672/api` | RabbitMQ Management HTTP API |
| `RABBITMQ_USER` / `RABBITMQ_PASS` | `guest` / `guest` | RabbitMQ Credentials |

---

## 🛠️ Specialized Custom Commands

In addition to standard Cypress commands, the test suite provides native commands:
- **MongoDB**: `cy.mongoPing()`, `cy.mongoInsert(col, doc/docs)`, `cy.mongoQuery(col, filter, options)`, `cy.mongoDeleteMany(col, filter)`, `cy.mongoCount(col, filter)`
- **Redis**: `cy.redisPing()`, `cy.redisSet(key, val, ttl)`, `cy.redisGet(key)`, `cy.redisDel(key)`, `cy.redisExists(key)`, `cy.redisFlushPattern(pattern)`
- **RabbitMQ**: `cy.rabbitCheckHealth()`, `cy.rabbitPublish(exchange, routingKey, msg)`, `cy.rabbitCheckQueue(queue)`, `cy.rabbitPurgeQueue(queue)`
- **PostgreSQL**: `cy.dbQuery(sql, params)`, `cy.dbSeed(table, data)`, `cy.dbCleanup(table, where)`, `cy.dbVerifyRecord(table, where, expected)`

---

## 🛡️ CI/CD Integration

Integrated into GitHub Actions in `.github/workflows/enterprise-pipeline.yml`:

```yaml
- name: Run E2E Tests (Cypress)
  run: |
    cd tests/03-e2e/cypress
    npm install
    npm run cy:run
```

All test runs capture automatic failure screenshots and WebM/MP4 session videos for rapid debugging and compliance auditing.



.\tests\04-security\dast\zap-scan.ps1 -TargetUrl "http://localhost:5000" -Mode "baseline"
