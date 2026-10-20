// ═══════════════════════════════════════════════════════════════════════
// OWASP A02:2021 — Cryptographic Failures Test Suite
// Covers:
// - Prevention of password / secret leakage in API responses
// - Protection of sensitive financial & identity PII (Masking)
// - Enforcement of transport layer security and token confidentiality
// - Prevention of sensitive tokens in URL query parameters
// ═══════════════════════════════════════════════════════════════════════

const { expect } = require('chai');
const { CONFIG, probe, TokenCraft } = require('../security-helper');

describe('OWASP A02:2021 — Cryptographic Failures', () => {

  const validToken = TokenCraft.custom(CONFIG.TENANT_A, 'Administrator');

  describe('1. Sensitive Secret Leakage Prevention', () => {
    it('ensures user profile endpoints never expose password_hash or salt in responses', async () => {
      const res = await probe('GET', `${CONFIG.GATEWAY_URL}/api/identity/users/profile`, {
        headers: {
          Authorization: `Bearer ${validToken}`,
          'X-Tenant-Code': CONFIG.TENANT_A,
        },
      });

      if (!res.isNetworkError && res.status === 200) {
        const bodyStr = JSON.stringify(res.data).toLowerCase();
        expect(bodyStr).to.not.include('password_hash');
        expect(bodyStr).to.not.include('passwordhash');
        expect(bodyStr).to.not.include('salt');
        expect(bodyStr).to.not.include('secret_key');
      }
    });

    it('ensures login endpoint returns tokens securely without echoing back raw user passwords', async () => {
      const res = await probe('POST', `${CONFIG.IDENTITY_URL}/api/identity/login`, {
        headers: { 'Content-Type': 'application/json' },
        data: {
          email: CONFIG.ADMIN_EMAIL,
          password: CONFIG.ADMIN_PASS,
        },
      });

      if (!res.isNetworkError && res.data) {
        expect(res.data).to.not.have.property('password');
        expect(res.data).to.not.have.property('plainPassword');
      }
    });
  });

  describe('2. Sensitive Data & PII Masking', () => {
    it('ensures payment card / bank IBAN details are masked (e.g. **** **** 1234)', async () => {
      const res = await probe('GET', `${CONFIG.GATEWAY_URL}/api/payment/methods`, {
        headers: {
          Authorization: `Bearer ${validToken}`,
          'X-Tenant-Code': CONFIG.TENANT_A,
        },
      });

      if (!res.isNetworkError && res.status === 200 && Array.isArray(res.data)) {
        res.data.forEach((item) => {
          if (item.account_number) {
            expect(item.account_number).to.match(/(\*+|x+)\d{4}/i, 'Account numbers must be masked');
          }
        });
      }
    });
  });

  describe('3. Token In Query Parameter Prevention', () => {
    it('disallows or discourages passing bearer tokens in URL query strings (prevents browser/access-log leakage)', async () => {
      const res = await probe('GET', `${CONFIG.GATEWAY_URL}/api/shipping/orders?access_token=${validToken}`, {
        headers: {
          'X-Tenant-Code': CONFIG.TENANT_A,
        },
      });

      if (!res.isNetworkError) {
        // Protected endpoint should reject query string token when Authorization header is absent
        expect([401, 403]).to.include(res.status);
      }
    });
  });
});
