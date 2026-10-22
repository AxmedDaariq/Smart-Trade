// ═══════════════════════════════════════════════════════════════════════
// OWASP A03:2021 — Injection Test Suite
// Covers:
// - SQL Injection (SQLi tautologies, union selects, stacked queries)
// - NoSQL Injection (MongoDB $gt, $ne, $where object injections in telematics)
// - Cross-Site Scripting (XSS input sanitization)
// - Path Traversal / LFI (Local File Inclusion in document endpoints)
// ═══════════════════════════════════════════════════════════════════════

const { expect } = require('chai');
const { CONFIG, probe, TokenCraft, SecurityPayloads } = require('../security-helper');

describe('OWASP A03:2021 — Injection (SQLi, NoSQLi, XSS, Path Traversal)', () => {

  const validToken = TokenCraft.custom(CONFIG.TENANT_A, 'Administrator');

  // ──────────────────────── SQL INJECTION ────────────────────────

  describe('1. Relational SQL Injection Prevention', () => {
    SecurityPayloads.SQL_INJECTION.forEach((payload) => {
      it(`sanitizes SQL payload '${payload}' in tracking search queries without syntax errors`, async () => {
        const res = await probe('GET', `${CONFIG.GATEWAY_URL}/api/shipping/orders/search`, {
          params: { query: payload },
          headers: {
            Authorization: `Bearer ${validToken}`,
            'X-Tenant-Code': CONFIG.TENANT_A,
          },
        });

        if (!res.isNetworkError) {
          // Should never return 500 internal server error with SQL syntax crashes
          expect(res.status).to.not.eq(500);
          expect([200, 400, 404]).to.include(res.status);

          const bodyStr = JSON.stringify(res.data).toLowerCase();
          expect(bodyStr).to.not.include('syntax error');
          expect(bodyStr).to.not.include('pg_catalog');
          expect(bodyStr).to.not.include('sqlite_');
        }
      });

      it(`prevents SQL auth bypass on login with payload '${payload}'`, async () => {
        const res = await probe('POST', `${CONFIG.IDENTITY_URL}/api/identity/login`, {
          headers: { 'Content-Type': 'application/json' },
          data: {
            email: payload,
            password: 'AnyPassword123!',
          },
        });

        if (!res.isNetworkError) {
          // Auth bypass should NEVER yield 200 OK
          expect(res.status).to.not.eq(200);
          expect([400, 401, 403]).to.include(res.status);
        }
      });
    });
  });

  // ─────────────────────── NOSQL INJECTION ───────────────────────

  describe('2. MongoDB NoSQL Telematics Injection Prevention', () => {
    SecurityPayloads.NOSQL_INJECTION.forEach((payload) => {
      it(`rejects or sanitizes NoSQL operator injection in telematics query ${JSON.stringify(payload)}`, async () => {
        const res = await probe('POST', `${CONFIG.GATEWAY_URL}/api/v1/fleet/telemetry/query`, {
          headers: {
            Authorization: `Bearer ${validToken}`,
            'X-Tenant-Code': CONFIG.TENANT_A,
            'Content-Type': 'application/json',
          },
          data: {
            truck_id: payload, // Injecting object operator instead of primitive string
          },
        });

        if (!res.isNetworkError) {
          expect(res.status).to.not.eq(500);
          expect([200, 400, 422]).to.include(res.status);
        }
      });
    });
  });

  // ──────────────────────────── XSS ────────────────────────────

  describe('3. Cross-Site Scripting (XSS) Sanitization', () => {
    SecurityPayloads.XSS_VECTORS.forEach((xss) => {
      it(`neutralizes script tag '${xss}' submitted in consignment remarks`, async () => {
        const res = await probe('POST', `${CONFIG.GATEWAY_URL}/api/shipping/orders`, {
          headers: {
            Authorization: `Bearer ${validToken}`,
            'X-Tenant-Code': CONFIG.TENANT_A,
            'Content-Type': 'application/json',
          },
          data: {
            origin: 'Berbera Container Terminal',
            destination: 'Addis Ababa Dry Port',
            remarks: xss,
          },
        });

        if (!res.isNetworkError && (res.status === 200 || res.status === 201)) {
          const bodyStr = JSON.stringify(res.data);
          // Raw unescaped executable script should not be returned un-sanitized
          expect(bodyStr).to.not.include("<script>alert(");
        }
      });
    });
  });

  // ─────────────────────── PATH TRAVERSAL ───────────────────────

  describe('4. Path Traversal & Local File Inclusion (LFI)', () => {
    SecurityPayloads.PATH_TRAVERSAL.forEach((path) => {
      it(`blocks directory traversal path '${path}' on document download endpoints`, async () => {
        const res = await probe('GET', `${CONFIG.GATEWAY_URL}/api/customs/documents/download`, {
          params: { file: path },
          headers: {
            Authorization: `Bearer ${validToken}`,
            'X-Tenant-Code': CONFIG.TENANT_A,
          },
        });

        if (!res.isNetworkError) {
          expect([400, 403, 404]).to.include(res.status);
          const bodyStr = JSON.stringify(res.data).toLowerCase();
          expect(bodyStr).to.not.include('root:');
          expect(bodyStr).to.not.include('[extensions]');
        }
      });
    });
  });
});
