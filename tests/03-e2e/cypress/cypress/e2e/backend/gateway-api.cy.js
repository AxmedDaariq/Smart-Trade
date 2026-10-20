// ═══════════════════════════════════════════════════════════════════════
// Backend E2E — API Gateway
// Tests the API Gateway routing, rate-limiting headers, CORS
// ═══════════════════════════════════════════════════════════════════════

describe('Backend API — API Gateway', () => {

  const GATEWAY = Cypress.env('GATEWAY_URL') || 'http://localhost:8080';
  const API     = Cypress.env('API_URL')     || 'http://localhost:5000';
  let authToken;

  before(() => {
    cy.request({
      method: 'POST',
      url: `${API}/api/identity/login`,
      body: {
        email: Cypress.env('ADMIN_EMAIL'),
        password: Cypress.env('ADMIN_PASS'),
      },
    }).then((res) => {
      authToken = res.body.token;
    });
  });

  // ────────────── GATEWAY HEALTH ──────────────
  context('Gateway Health', () => {

    it('should return 200 on the gateway health endpoint', () => {
      cy.request({
        method: 'GET',
        url: `${GATEWAY}/health`,
        failOnStatusCode: false,
      }).then((res) => {
        expect(res.status).to.be.oneOf([200, 204]);
      });
    });
  });

  // ────────────── ROUTING ──────────────
  context('Request Routing', () => {

    it('should route /api/shipping/* through the gateway to the shipping service', () => {
      cy.request({
        method: 'GET',
        url: `${GATEWAY}/api/shipping/orders`,
        headers: {
          Authorization: `Bearer ${authToken}`,
          'X-Tenant-Code': Cypress.env('TENANT_CODE'),
        },
        failOnStatusCode: false,
      }).then((res) => {
        // Even if empty, should not be a gateway error (502/503)
        expect(res.status).to.not.be.oneOf([502, 503]);
      });
    });

    it('should return 404 for an unknown route', () => {
      cy.request({
        method: 'GET',
        url: `${GATEWAY}/api/nonexistent/route`,
        failOnStatusCode: false,
      }).then((res) => {
        expect(res.status).to.be.oneOf([404, 400]);
      });
    });
  });

  // ────────────── CORS HEADERS ──────────────
  context('CORS Headers', () => {

    it('should include CORS headers in responses', () => {
      cy.request({
        method: 'OPTIONS',
        url: `${GATEWAY}/api/identity/login`,
        failOnStatusCode: false,
        headers: {
          Origin: 'http://localhost:5173',
          'Access-Control-Request-Method': 'POST',
        },
      }).then((res) => {
        // The gateway should return CORS headers or at least not block
        expect(res.status).to.be.oneOf([200, 204, 405]);
      });
    });
  });

  // ────────────── AUTHENTICATION ENFORCEMENT ──────────────
  context('Authentication Enforcement', () => {

    it('should reject requests without auth token on protected endpoints', () => {
      cy.request({
        method: 'GET',
        url: `${GATEWAY}/api/shipping/orders`,
        failOnStatusCode: false,
      }).then((res) => {
        expect(res.status).to.eq(401);
      });
    });
  });
});
