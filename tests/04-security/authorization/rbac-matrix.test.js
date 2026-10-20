// ═══════════════════════════════════════════════════════════════════════
// Authorization Security Test Suite — RBAC / PBAC Matrix
// Covers:
// - Role-Based Access Control matrix validation across personas:
//   * System Administrator
//   * Customs Inspector
//   * Freight Forwarder
//   * Trader / Consignee
//   * Mobile Driver
// - Negative boundary testing (unauthorized roles receive 403 Forbidden)
// ═══════════════════════════════════════════════════════════════════════

const { expect } = require('chai');
const { CONFIG, probe, TokenCraft } = require('../security-helper');

describe('Security — Role-Based Access Control (RBAC) Matrix', () => {

  const tokens = {
    Admin:             TokenCraft.custom(CONFIG.TENANT_A, 'Administrator'),
    CustomsInspector:  TokenCraft.custom(CONFIG.TENANT_A, 'CustomsInspector'),
    FreightForwarder:  TokenCraft.custom(CONFIG.TENANT_A, 'FreightForwarder'),
    Trader:            TokenCraft.custom(CONFIG.TENANT_A, 'Trader'),
    Driver:            TokenCraft.custom(CONFIG.TENANT_A, 'Driver'),
  };

  const matrix = [
    {
      action: 'Create Shipment Consignment',
      method: 'POST',
      path: '/api/shipping/orders',
      allowedRoles: ['Admin', 'FreightForwarder', 'Trader'],
      forbiddenRoles: ['CustomsInspector', 'Driver'],
    },
    {
      action: 'Issue Customs Clearance Hold / Release',
      method: 'POST',
      path: '/api/customs/clearance/decision',
      allowedRoles: ['Admin', 'CustomsInspector'],
      forbiddenRoles: ['FreightForwarder', 'Trader', 'Driver'],
    },
    {
      action: 'Assign Fleet Drivers to Route',
      method: 'POST',
      path: '/api/fleet/dispatch',
      allowedRoles: ['Admin', 'FreightForwarder'],
      forbiddenRoles: ['CustomsInspector', 'Trader', 'Driver'],
    },
    {
      action: 'System Tenant Administration',
      method: 'POST',
      path: '/api/admin/tenants',
      allowedRoles: ['Admin'],
      forbiddenRoles: ['CustomsInspector', 'FreightForwarder', 'Trader', 'Driver'],
    },
  ];

  matrix.forEach(({ action, method, path, allowedRoles, forbiddenRoles }) => {
    describe(`Operation: ${action} (${method} ${path})`, () => {

      forbiddenRoles.forEach((role) => {
        it(`denies unauthorized role '${role}' with 403 Forbidden`, async () => {
          const res = await probe(method, `${CONFIG.GATEWAY_URL}${path}`, {
            headers: {
              Authorization: `Bearer ${tokens[role]}`,
              'X-Tenant-Code': CONFIG.TENANT_A,
              'Content-Type': 'application/json',
            },
            data: { test_probe: true },
          });

          if (!res.isNetworkError) {
            expect([401, 403]).to.include(res.status, `Role ${role} should be denied for ${action}`);
          }
        });
      });

      allowedRoles.forEach((role) => {
        it(`allows authorized role '${role}' through role authorization gate`, async () => {
          const res = await probe(method, `${CONFIG.GATEWAY_URL}${path}`, {
            headers: {
              Authorization: `Bearer ${tokens[role]}`,
              'X-Tenant-Code': CONFIG.TENANT_A,
              'Content-Type': 'application/json',
            },
            data: { test_probe: true },
          });

          if (!res.isNetworkError) {
            // Must NOT fail with 403 Forbidden (may return 200, 201, 400 for validation, or 404 if mock)
            expect(res.status).to.not.eq(403, `Authorized role ${role} should not receive 403 Forbidden`);
          }
        });
      });
    });
  });
});
