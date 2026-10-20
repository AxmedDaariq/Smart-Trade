// ═══════════════════════════════════════════════════════════════════════
// OWASP A10:2021 — Server-Side Request Forgery (SSRF) Test Suite
// Covers:
// - Prevention of intranet / loopback address probing (127.0.0.1, localhost)
// - Prevention of Cloud Metadata exploitation (169.254.169.254)
// - Prevention of private RFC 1918 internal network scanning (10.0.0.0/8, 192.168.0.0/16)
// - Restriction to safe HTTP/HTTPS URL protocols (blocking file://, gopher://)
// ═══════════════════════════════════════════════════════════════════════

const { expect } = require('chai');
const { CONFIG, probe, TokenCraft, SecurityPayloads } = require('../security-helper');

describe('OWASP A10:2021 — Server-Side Request Forgery (SSRF)', () => {

  const validToken = TokenCraft.custom(CONFIG.TENANT_A, 'Administrator');

  const webhookEndpoints = [
    '/api/notification/webhooks',
    '/api/payment/webhooks/register',
    '/api/customs/callbacks/register',
  ];

  describe('1. Intranet & Cloud Metadata Protection', () => {
    webhookEndpoints.forEach((endpoint) => {
      SecurityPayloads.SSRF_TARGETS.forEach((targetUrl) => {
        it(`blocks internal SSRF destination '${targetUrl}' on ${endpoint}`, async () => {
          const res = await probe('POST', `${CONFIG.GATEWAY_URL}${endpoint}`, {
            headers: {
              Authorization: `Bearer ${validToken}`,
              'X-Tenant-Code': CONFIG.TENANT_A,
              'Content-Type': 'application/json',
            },
            data: {
              webhook_url: targetUrl,
              events: ['shipment.created', 'payment.completed'],
            },
          });

          if (!res.isNetworkError) {
            // Must reject internal intranet / cloud metadata IPs
            expect([400, 403, 404, 422]).to.include(res.status, `SSRF target ${targetUrl} should be rejected`);
          }
        });
      });
    });
  });

  describe('2. Protocol Scheme Restrictions', () => {
    const dangerousSchemes = [
      'file:///etc/passwd',
      'file:///c:/windows/win.ini',
      'gopher://127.0.0.1:6379/_INFO',
      'ftp://internal-server.local',
    ];

    dangerousSchemes.forEach((schemeUrl) => {
      it(`rejects non-HTTP dangerous URI scheme '${schemeUrl}'`, async () => {
        const res = await probe('POST', `${CONFIG.GATEWAY_URL}/api/notification/webhooks`, {
          headers: {
            Authorization: `Bearer ${validToken}`,
            'X-Tenant-Code': CONFIG.TENANT_A,
            'Content-Type': 'application/json',
          },
          data: {
            webhook_url: schemeUrl,
          },
        });

        if (!res.isNetworkError) {
          expect([400, 422, 404]).to.include(res.status);
        }
      });
    });
  });
});
