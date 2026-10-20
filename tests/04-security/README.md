# Smart Trade Logistics — Security Testing Suite (OWASP Top 10 & OWASP ZAP)

Welcome to the **Enterprise Security Testing Suite** (`tests/04-security`) for the Smart Trade Logistics Platform.

This suite delivers 100% automated security auditing, negative boundary verification, and dynamic vulnerability assessment based on the **OWASP Top 10 (2021)** standard and **OWASP ZAP (Zed Attack Proxy)**.

---

## 📁 Architecture Overview

```
tests/04-security/
├── README.md                                  # This security testing documentation
├── package.json                               # NPM scripts and testing dependencies
├── security-helper.js                         # Shared security probe, token craft, and fuzzing payloads
├── api/                                       # OWASP Top 10 API Security Tests
│   ├── a01-broken-access-control.test.js      # BOLA, BFLA, IDOR, unauthenticated access
│   ├── a02-cryptographic-failures.test.js     # Password leak prevention, PII masking, token secrecy
│   ├── a03-injection.test.js                  # SQLi, NoSQLi (MongoDB), XSS, Path Traversal
│   ├── a04-insecure-design.test.js            # Rate limiting, parameter tampering, workflow state bypass
│   ├── a05-security-misconfiguration.test.js  # Security headers, CORS audits, stack trace suppression
│   ├── a07-auth-failures.test.js              # JWT forgery, alg:none confusion, expired tokens, weak passwords
│   ├── a08-data-integrity-failures.test.js    # Payload size limits, Content-Type spoofing, e-seal integrity
│   ├── a09-logging-monitoring-failures.test.js# Audit trail recording, CRLF log injection prevention
│   └── a10-ssrf.test.js                       # Server-Side Request Forgery, loopback/cloud metadata blocks
├── authentication/
│   └── auth-security.test.js                  # Refresh token rotation, token blacklist, password policy
├── authorization/
│   └── rbac-matrix.test.js                    # Role-Based Access Control matrix across 5 platform personas
├── tenant-isolation/
│   └── tenant-isolation.test.js               # Multi-tenant boundary verification & header spoofing defense
├── dast/                                      # Dynamic Application Security Testing (OWASP ZAP)
│   ├── zap-ci-runner.js                       # Automated ZAP runner (Docker + Native DAST fallback)
│   ├── zap-baseline.conf                      # ZAP passive scanning rule configurations & thresholds
│   ├── zap-scan.ps1                           # PowerShell ZAP runner for Windows developers
│   ├── zap-scan.sh                            # Bash ZAP runner for Linux / CI/CD pipelines
│   └── generate-security-report.js            # Master security audit report compiler
└── dependency-scanning/
    └── dependency-audit.js                    # Software Composition Analysis (SCA) for npm & .NET packages
```

---

## 🎯 OWASP Top 10 (2021) Mapping

| OWASP Category | Threat Description | Automated Test Implementation |
| :--- | :--- | :--- |
| **A01: Broken Access Control** | Unauthorized access to data, horizontal (BOLA/IDOR) & vertical (BFLA) escalation | [`api/a01-broken-access-control.test.js`](file:///d:/Enterprise%20Applications/smart-trade-logistics-platform/tests/04-security/api/a01-broken-access-control.test.js) |
| **A02: Cryptographic Failures** | Exposure of plaintext secrets, missing PII redaction, token in query strings | [`api/a02-cryptographic-failures.test.js`](file:///d:/Enterprise%20Applications/smart-trade-logistics-platform/tests/04-security/api/a02-cryptographic-failures.test.js) |
| **A03: Injection** | SQL injection on PostgreSQL, NoSQL injection on MongoDB telematics, XSS & LFI | [`api/a03-injection.test.js`](file:///d:/Enterprise%20Applications/smart-trade-logistics-platform/tests/04-security/api/a03-injection.test.js) |
| **A04: Insecure Design** | Rate limit bypass, negative financial values, state-machine skip | [`api/a04-insecure-design.test.js`](file:///d:/Enterprise%20Applications/smart-trade-logistics-platform/tests/04-security/api/a04-insecure-design.test.js) |
| **A05: Security Misconfiguration** | Missing security headers (CSP, HSTS, X-Frame-Options), CORS wildcards, stack trace dumps | [`api/a05-security-misconfiguration.test.js`](file:///d:/Enterprise%20Applications/smart-trade-logistics-platform/tests/04-security/api/a05-security-misconfiguration.test.js) |
| **A06: Vulnerable Components** | Outdated libraries, unpatched CVEs across npm and .NET NuGet packages | [`dependency-scanning/dependency-audit.js`](file:///d:/Enterprise%20Applications/smart-trade-logistics-platform/tests/04-security/dependency-scanning/dependency-audit.js) |
| **A07: Auth Failures** | Forged JWT signatures, `alg: none` attack, expired tokens, credential stuffing | [`api/a07-auth-failures.test.js`](file:///d:/Enterprise%20Applications/smart-trade-logistics-platform/tests/04-security/api/a07-auth-failures.test.js) |
| **A08: Data Integrity Failures** | Body parser DoS (>10MB), Content-Type mismatch, e-seal tamper payload detection | [`api/a08-data-integrity-failures.test.js`](file:///d:/Enterprise%20Applications/smart-trade-logistics-platform/tests/04-security/api/a08-data-integrity-failures.test.js) |
| **A09: Logging & Monitoring** | Missing audit logs for high-value actions, CRLF log injection attempts | [`api/a09-logging-monitoring-failures.test.js`](file:///d:/Enterprise%20Applications/smart-trade-logistics-platform/tests/04-security/api/a09-logging-monitoring-failures.test.js) |
| **A10: SSRF** | Webhook URL manipulation targeting loopback (127.0.0.1) or cloud metadata (169.254.169.254) | [`api/a10-ssrf.test.js`](file:///d:/Enterprise%20Applications/smart-trade-logistics-platform/tests/04-security/api/a10-ssrf.test.js) |

---

## 🚀 Running Security Tests

Navigate to `tests/04-security`:

```bash
cd tests/04-security
npm install
```

### Execution Commands

```bash
# 1. Run all OWASP Top 10 API Security Tests
npm run test:security

# 2. Run only OWASP Top 10 Core Specs
npm run test:owasp

# 3. Run Authentication & RBAC Matrix Tests
npm run test:auth

# 4. Run Multi-Tenant Isolation Tests
npm run test:tenant

# 5. Run OWASP ZAP Baseline DAST Scan (Docker or Native Fallback)
npm run zap:baseline

# 6. Run OWASP ZAP API Active Scan
npm run zap:api

# 7. Run Dependency Vulnerability Audit (SCA)
npm run scan:dependencies

# 8. Compile Executive Security Audit Report
npm run audit:report
```

---

## ⚡ OWASP ZAP DAST Scanning

The ZAP runner automatically detects your local environment:
1. **Docker Available**: Pulls and executes `ghcr.io/zaproxy/zaproxy:stable` with the custom rules defined in `dast/zap-baseline.conf`.
2. **Docker Not Available**: Activates the automated Native DAST Security Auditor to inspect security headers, CORS policies, information disclosures, and debug error leakage.

All scan artifacts and consolidated executive summaries are output to:
- HTML Report: `tests/09-reports/security/zap-security-report.html`
- JSON Report: `tests/09-reports/security/zap-security-report.json`
- Executive Markdown: `tests/09-reports/security/SECURITY_AUDIT_REPORT.md`

---

## 🛡️ CI/CD Pipeline Integration

Added to `.github/workflows/enterprise-pipeline.yml` for automated security gates:

```yaml
- name: Run OWASP Top 10 Security Tests
  run: |
    cd tests/04-security
    npm install
    npm run test:security
    npm run zap:baseline
    npm run audit:report
```
