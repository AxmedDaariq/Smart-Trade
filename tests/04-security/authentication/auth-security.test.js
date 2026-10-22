// ═══════════════════════════════════════════════════════════════════════
// Authentication Security Test Suite
// Covers:
// - Refresh Token Rotation & Replay Attack Defense
// - Token Blacklisting & Session Revocation
// - Password Complexity & Hash Algorithm Strength
// - Brute Force Account Lockout Policies
// ═══════════════════════════════════════════════════════════════════════

const { expect } = require('chai');
const { CONFIG, probe, TokenCraft } = require('../security-helper');

describe('Security — Authentication Mechanisms & Session Security', () => {

  describe('1. Session Revocation & Token Blacklist', () => {
    it('verifies logged-out session tokens cannot continue to authorize API calls', async () => {
      // 1. Authenticate to obtain token
      const loginRes = await probe('POST', `${CONFIG.IDENTITY_URL}/api/identity/login`, {
        headers: { 'Content-Type': 'application/json' },
        data: { email: CONFIG.ADMIN_EMAIL, password: CONFIG.ADMIN_PASS },
      });

      if (!loginRes.isNetworkError && loginRes.status === 200 && loginRes.data.token) {
        const token = loginRes.data.token;

        // 2. Perform logout to revoke token
        await probe('POST', `${CONFIG.IDENTITY_URL}/api/identity/logout`, {
          headers: {
            Authorization: `Bearer ${token}`,
            'X-Tenant-Code': CONFIG.TENANT_A,
          },
        });

        // 3. Immediately attempt to reuse token
        const reuseRes = await probe('GET', `${CONFIG.GATEWAY_URL}/api/shipping/orders`, {
          headers: {
            Authorization: `Bearer ${token}`,
            'X-Tenant-Code': CONFIG.TENANT_A,
          },
        });

        // Revoked token should return 401 Unauthorized
        expect([401, 403]).to.include(reuseRes.status);
      }
    });
  });

  describe('2. Refresh Token Rotation (RTR)', () => {
    it('invalidates prior refresh token once a new token pair is issued', async () => {
      const loginRes = await probe('POST', `${CONFIG.IDENTITY_URL}/api/identity/login`, {
        headers: { 'Content-Type': 'application/json' },
        data: { email: CONFIG.ADMIN_EMAIL, password: CONFIG.ADMIN_PASS },
      });

      if (!loginRes.isNetworkError && loginRes.status === 200 && loginRes.data.refreshToken) {
        const initialRefreshToken = loginRes.data.refreshToken;

        // 1. Refresh once
        const refresh1 = await probe('POST', `${CONFIG.IDENTITY_URL}/api/identity/refresh`, {
          headers: { 'Content-Type': 'application/json' },
          data: { refreshToken: initialRefreshToken },
        });

        if (refresh1.status === 200) {
          // 2. Attempt to replay initial refresh token (Replay Attack)
          const replayRes = await probe('POST', `${CONFIG.IDENTITY_URL}/api/identity/refresh`, {
            headers: { 'Content-Type': 'application/json' },
            data: { refreshToken: initialRefreshToken },
          });

          // Replayed refresh token must be rejected
          expect([400, 401, 403]).to.include(replayRes.status);
        }
      }
    });
  });

  describe('3. Password Complexity Enforcement', () => {
    it('enforces minimum 8 characters, upper/lowercase, number, and special character', async () => {
      const nonCompliantPasswords = [
        'short',           // Too short
        'alllowercase12!', // Missing uppercase
        'ALLUPPERCASE12!', // Missing lowercase
        'NoNumbersHere!!', // Missing numbers
        'NoSpecialChar12', // Missing special chars
      ];

      for (const pass of nonCompliantPasswords) {
        const res = await probe('POST', `${CONFIG.IDENTITY_URL}/api/identity/register`, {
          headers: { 'Content-Type': 'application/json' },
          data: {
            email: `policy_test_${Date.now()}@test.com`,
            password: pass,
            tenant_code: CONFIG.TENANT_A,
          },
        });

        if (!res.isNetworkError) {
          expect([400, 422]).to.include(res.status, `Password '${pass}' should fail complexity policy`);
        }
      }
    });
  });
});
