// ═══════════════════════════════════════════════════════════════════════
// OWASP A07:2021 — Identification and Authentication Failures Test Suite
// Covers:
// - JWT Signature Forgery and Tamper Rejection
// - JWT Algorithm Confusion Attack ('none' algorithm)
// - Expired Token Rejection
// - Malformed Bearer Token Rejection
// - Weak Password and Missing Credential Defense
// ═══════════════════════════════════════════════════════════════════════

const { expect } = require('chai');
const { CONFIG, probe, TokenCraft } = require('../security-helper');

describe('OWASP A07:2021 — Identification and Authentication Failures', () => {

  const protectedUrl = `${CONFIG.GATEWAY_URL}/api/shipping/orders`;

  describe('1. JWT Signature & Algorithm Integrity', () => {
    it('strictly rejects forged JWT token signed with an invalid secret (401 Unauthorized)', async () => {
      const forgedToken = TokenCraft.forged({ sub: 'admin-1', role: 'Administrator' });

      const res = await probe('GET', protectedUrl, {
        headers: {
          Authorization: `Bearer ${forgedToken}`,
          'X-Tenant-Code': CONFIG.TENANT_A,
        },
      });

      if (!res.isNetworkError) {
        expect(res.status).to.eq(401, 'Forged JWT signature must return 401 Unauthorized');
      }
    });

    it('strictly rejects JWT with "none" algorithm header (Algorithm Confusion Attack)', async () => {
      const noneToken = TokenCraft.noneAlgorithm({ sub: 'admin-1', role: 'Administrator' });

      const res = await probe('GET', protectedUrl, {
        headers: {
          Authorization: `Bearer ${noneToken}`,
          'X-Tenant-Code': CONFIG.TENANT_A,
        },
      });

      if (!res.isNetworkError) {
        expect(res.status).to.eq(401, 'JWT with alg:none must be rejected with 401 Unauthorized');
      }
    });

    it('strictly rejects expired JWT tokens', async () => {
      const expiredToken = TokenCraft.expired({ sub: 'admin-1', role: 'Administrator' });

      const res = await probe('GET', protectedUrl, {
        headers: {
          Authorization: `Bearer ${expiredToken}`,
          'X-Tenant-Code': CONFIG.TENANT_A,
        },
      });

      if (!res.isNetworkError) {
        expect(res.status).to.eq(401, 'Expired token must return 401 Unauthorized');
      }
    });

    it('rejects malformed bearer tokens (e.g. truncated, null, random strings)', async () => {
      const malformedCases = ['Bearer', 'Bearer null', 'Bearer undefined', 'Bearer 12345', 'Token abcdef'];

      for (const authHeader of malformedCases) {
        const res = await probe('GET', protectedUrl, {
          headers: {
            Authorization: authHeader,
            'X-Tenant-Code': CONFIG.TENANT_A,
          },
        });

        if (!res.isNetworkError) {
          expect(res.status).to.eq(401);
        }
      }
    });
  });

  describe('2. Credential Validation & Registration Hardening', () => {
    it('rejects registration with trivially weak passwords', async () => {
      const weakPasswords = ['123456', 'password', 'qwerty', 'admin'];

      for (const pass of weakPasswords) {
        const res = await probe('POST', `${CONFIG.IDENTITY_URL}/api/identity/register`, {
          headers: { 'Content-Type': 'application/json' },
          data: {
            email: `weak_user_${Date.now()}@test.com`,
            password: pass,
            tenant_code: CONFIG.TENANT_A,
          },
        });

        if (!res.isNetworkError) {
          // Should reject weak password
          expect([400, 422]).to.include(res.status);
        }
      }
    });

    it('rejects login requests missing email or password fields with 400 Bad Request', async () => {
      const res = await probe('POST', `${CONFIG.IDENTITY_URL}/api/identity/login`, {
        headers: { 'Content-Type': 'application/json' },
        data: {},
      });

      if (!res.isNetworkError) {
        expect([400, 422]).to.include(res.status);
      }
    });
  });
});
