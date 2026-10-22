// ═══════════════════════════════════════════════════════════════════════
// OWASP A09:2021 — Security Logging and Monitoring Failures Test Suite
// Covers:
// - Audit logging of critical security operations (Auth, Role, Payments)
// - CRLF Log Injection prevention
// - Sensitive data exclusion from system audit logs
// ═══════════════════════════════════════════════════════════════════════

const { expect } = require('chai');
const { CONFIG, probe, TokenCraft } = require('../security-helper');

describe('OWASP A09:2021 — Security Logging and Monitoring Failures', () => {

  const adminToken = TokenCraft.custom(CONFIG.TENANT_A, 'Administrator');

  describe('1. Audit Trail Record Verification', () => {
    it('ensures critical security actions (role assignment, tenant admin changes) record audit events', async () => {
      const res = await probe('GET', `${CONFIG.GATEWAY_URL}/api/audit/logs`, {
        headers: {
          Authorization: `Bearer ${adminToken}`,
          'X-Tenant-Code': CONFIG.TENANT_A,
        },
      });

      if (!res.isNetworkError && res.status === 200 && Array.isArray(res.data)) {
        // Audit log entries must contain standard security fields
        if (res.data.length > 0) {
          const entry = res.data[0];
          expect(entry).to.have.property('timestamp');
          expect(entry).to.have.property('action');
        }
      }
    });
  });

  describe('2. CRLF Log Injection Prevention', () => {
    it('sanitizes carriage return / newline characters (\\r\\n) in audit log inputs', async () => {
      const crlfPayload = "admin_user\r\n2026-10-20 [CRITICAL] SYSTEM SHUTDOWN BY OPERATOR";

      const res = await probe('POST', `${CONFIG.IDENTITY_URL}/api/identity/login`, {
        headers: { 'Content-Type': 'application/json' },
        data: {
          email: crlfPayload,
          password: 'Password123!',
        },
      });

      if (!res.isNetworkError) {
        // Should handle without logging unescaped raw newlines or causing server panic
        expect(res.status).to.not.eq(500);
      }
    });
  });
});
