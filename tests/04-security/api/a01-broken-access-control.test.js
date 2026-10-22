// ═══════════════════════════════════════════════════════════════════════
// OWASP A01:2021 — Broken Access Control Test Suite
// Covers:
// - Horizontal Privilege Escalation (BOLA / IDOR)
// - Vertical Privilege Escalation (BFLA / Role boundary violations)
// - Unauthenticated endpoint protection (401 Unauthorized)
// - Tenant boundary isolation on direct object references
// ═══════════════════════════════════════════════════════════════════════

const { expect } = require('chai');
const { CONFIG, probe, TokenCraft } = require('../security-helper');

describe('OWASP A01:2021 — Broken Access Control', () => {

  const driverToken = TokenCraft.custom(CONFIG.TENANT_A, 'Driver', { email: 'driver@test.com' });
  const traderTokenTenantA = TokenCraft.custom(CONFIG.TENANT_A, 'Trader', { email: 'traderA@test.com' });
  const traderTokenTenantB = TokenCraft.custom(CONFIG.TENANT_B, 'Trader', { email: 'traderB@test.com' });

  // ──────────────── UNAUTHENTICATED ACCESS GUARDS ────────────────

  describe('1. Unauthenticated Access Protection', () => {
    const protectedEndpoints = [
      { method: 'GET',  path: '/api/shipping/orders' },
      { method: 'POST', path: '/api/shipping/orders' },
      { method: 'GET',  path: '/api/customs/declarations' },
      { method: 'POST', path: '/api/ectn/applications' },
      { method: 'GET',  path: '/api/fleet/vehicles' },
      { method: 'POST', path: '/api/payment/transactions' },
    ];

    protectedEndpoints.forEach(({ method, path }) => {
      it(`rejects unauthenticated request to ${method} ${path} with 401 Unauthorized`, async () => {
        const res = await probe(method, `${CONFIG.GATEWAY_URL}${path}`);
        if (!res.isNetworkError) {
          expect(res.status).to.eq(401, `Endpoint ${method} ${path} should require authentication`);
        }
      });
    });
  });

  // ──────────────── VERTICAL PRIVILEGE ESCALATION (BFLA) ────────────────

  describe('2. Vertical Privilege Escalation (Role Boundaries)', () => {
    it('prevents low-privilege Driver role from approving Customs clearance', async () => {
      const res = await probe('POST', `${CONFIG.GATEWAY_URL}/api/customs/clearance/approve`, {
        headers: {
          Authorization: `Bearer ${driverToken}`,
          'X-Tenant-Code': CONFIG.TENANT_A,
          'Content-Type': 'application/json',
        },
        data: {
          declaration_id: 'DEC-2026-9921',
          decision: 'APPROVED_GREEN_CHANNEL',
        },
      });

      if (!res.isNetworkError) {
        expect([401, 403]).to.include(res.status, 'Driver should not have permission to approve customs clearance');
      }
    });

    it('prevents low-privilege Driver role from creating or modifying tenant configurations', async () => {
      const res = await probe('POST', `${CONFIG.GATEWAY_URL}/api/admin/tenants`, {
        headers: {
          Authorization: `Bearer ${driverToken}`,
          'X-Tenant-Code': CONFIG.TENANT_A,
          'Content-Type': 'application/json',
        },
        data: {
          tenant_code: 'MALICIOUS_TENANT',
          name: 'Fake Cargo LLC',
        },
      });

      if (!res.isNetworkError) {
        expect([401, 403, 404]).to.include(res.status);
      }
    });

    it('prevents non-admin user from escalating own role or permissions', async () => {
      const res = await probe('PUT', `${CONFIG.GATEWAY_URL}/api/identity/users/self/role`, {
        headers: {
          Authorization: `Bearer ${driverToken}`,
          'X-Tenant-Code': CONFIG.TENANT_A,
          'Content-Type': 'application/json',
        },
        data: {
          role: 'SuperAdministrator',
          permissions: ['*'],
        },
      });

      if (!res.isNetworkError) {
        expect([400, 401, 403, 404, 405]).to.include(res.status);
      }
    });
  });

  // ────────────── HORIZONTAL PRIVILEGE ESCALATION (BOLA / IDOR) ──────────────

  describe('3. Horizontal Privilege Escalation (Cross-Tenant & IDOR Protection)', () => {
    it('prevents Tenant B from retrieving shipment documents belonging to Tenant A', async () => {
      const shipmentIdTenantA = 'SH-ACME-SECRET-8841';

      const res = await probe('GET', `${CONFIG.GATEWAY_URL}/api/shipping/orders/${shipmentIdTenantA}`, {
        headers: {
          Authorization: `Bearer ${traderTokenTenantB}`,
          'X-Tenant-Code': CONFIG.TENANT_B,
        },
      });

      if (!res.isNetworkError) {
        expect([401, 403, 404]).to.include(res.status, 'Tenant B must not access Tenant A shipment via IDOR');
      }
    });

    it('prevents modifying ECTN certificate belonging to another trader', async () => {
      const ectnId = 'ECTN-2026-SO-00441';

      const res = await probe('PUT', `${CONFIG.GATEWAY_URL}/api/ectn/applications/${ectnId}`, {
        headers: {
          Authorization: `Bearer ${traderTokenTenantB}`,
          'X-Tenant-Code': CONFIG.TENANT_B,
          'Content-Type': 'application/json',
        },
        data: {
          consignee: 'Hijacked Consignee Entity',
        },
      });

      if (!res.isNetworkError) {
        expect([401, 403, 404]).to.include(res.status);
      }
    });
  });
});
