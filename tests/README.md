# Smart Trade Logistics Platform - Test Suite

Halkan waxa ku yaal qaab-dhismeedka tijaabooyinka (testing architecture) ee mashruuca:

```
tests/
├── 01-unit/                     # Unit Tests (Controllers, Services, Domain, Repositories)
├── 02-integration/              # Integration & Database/API Tests
├── 03-e2e/                      # End-to-End Tests (Cypress Web, Mobile & Multi-service Scenarios)
├── 04-security/                 # Security Tests (Auth, RBAC, Tenant Isolation, DAST)
├── 05-performance/              # Performance Tests (k6 smoke, load, stress, spike, soak)
├── 06-chaos/                    # Chaos Engineering & Resilience (Failures, Recovery)
├── 07-contract/                 # Consumer-Driven Contracts (Pact / OpenAPI)
├── 08-test-infrastructure/      # Test Containers, Fixtures, Builders & Mock Data
└── 09-reports/                  # Test Results, Code Coverage & Audit Reports
```

## How to Run Tests

### Unit Tests
```bash
dotnet test tests/01-unit/Identity.UnitTests/Identity.UnitTests.csproj
```

### All Tests
```bash
dotnet test tests/
```
