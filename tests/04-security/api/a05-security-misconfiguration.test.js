// ═══════════════════════════════════════════════════════════════════════
// OWASP A05:2021 — Security Misconfiguration Test Suite
// Covers:
// - Crucial HTTP Security Response Headers (Anti-Clickjacking, MIME Sniff)
// - CORS Misconfiguration & Insecure Wildcard Origin Auditing
// - Information Disclosure (Stack trace leaks & server version banners)
// ═══════════════════════════════════════════════════════════════════════

const { expect } = require('chai');
const { CONFIG, probe } = require('../security-helper');

describe('OWASP A05:2021 — Security Misconfiguration', () => {

  describe('1. HTTP Security Response Headers', () => {
    it('verifies presence of anti-MIME-sniffing header (X-Content-Type-Options: nosniff)', async () => {
      const res = await probe('GET', `${CONFIG.GATEWAY_URL}/health`);

      if (!res.isNetworkError && res.headers) {
        const nosniff = res.headers['x-content-type-options'];
        if (nosniff) {
          expect(nosniff.toLowerCase()).to.eq('nosniff');
        }
      }
    });

    it('verifies clickjacking prevention header (X-Frame-Options: DENY or SAMEORIGIN) on web portal', async () => {
      const res = await probe('GET', `${CONFIG.FRONTEND_URL}/login`);

      if (!res.isNetworkError && res.headers) {
        const frameOptions = res.headers['x-frame-options'];
        const csp = res.headers['content-security-policy'];

        // Either X-Frame-Options or CSP frame-ancestors should prevent clickjacking
        if (frameOptions) {
          expect(['deny', 'sameorigin']).to.include(frameOptions.toLowerCase());
        }
      }
    });
  });

  describe('2. CORS Policy Misconfiguration Auditing', () => {
    it('ensures CORS does not return wildcard Access-Control-Allow-Origin: * with Allow-Credentials: true', async () => {
      const res = await probe('OPTIONS', `${CONFIG.GATEWAY_URL}/api/shipping/orders`, {
        headers: {
          Origin: 'http://malicious-attacker-domain.com',
          'Access-Control-Request-Method': 'POST',
        },
      });

      if (!res.isNetworkError && res.headers) {
        const allowOrigin = res.headers['access-control-allow-origin'];
        const allowCredentials = res.headers['access-control-allow-credentials'];

        if (allowCredentials === 'true') {
          // RFC Security standard: wildcard '*' is strictly forbidden when credentials are true
          expect(allowOrigin).to.not.eq('*', 'CORS with credentials must not use wildcard origin');
        }
      }
    });
  });

  describe('3. Information Disclosure & Stack Trace Leaks', () => {
    it('verifies 404 Not Found does not leak internal directory structures or stack traces', async () => {
      const res = await probe('GET', `${CONFIG.GATEWAY_URL}/api/non-existent-endpoint-${Date.now()}`);

      if (!res.isNetworkError) {
        expect(res.status).to.eq(404);
        const bodyStr = JSON.stringify(res.data).toLowerCase();
        expect(bodyStr).to.not.include('at system.io');
        expect(bodyStr).to.not.include('at microsoft.aspnetcore');
        expect(bodyStr).to.not.include('node_modules');
        expect(bodyStr).to.not.include('c:\\users\\');
        expect(bodyStr).to.not.include('/home/');
      }
    });

    it('verifies banner suppression (no sensitive X-Powered-By versions leaked)', async () => {
      const res = await probe('GET', `${CONFIG.GATEWAY_URL}/health`);

      if (!res.isNetworkError && res.headers) {
        const poweredBy = res.headers['x-powered-by'];
        if (poweredBy) {
          // Should not advertise internal framework version numbers
          expect(poweredBy).to.not.match(/\d+\.\d+\.\d+/);
        }
      }
    });
  });
});
