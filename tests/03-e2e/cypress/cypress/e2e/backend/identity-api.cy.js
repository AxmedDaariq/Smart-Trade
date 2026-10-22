// ═══════════════════════════════════════════════════════════════════════
// Backend E2E — Identity & Auth API
// Tests API endpoints directly using cy.request() — no browser UI needed
// ═══════════════════════════════════════════════════════════════════════

describe('Backend API — Identity Service', () => {

  const API = Cypress.env('API_URL') || 'http://localhost:5000';
  let authToken;

  // ────────────── HEALTH CHECK ──────────────
  context('Health Check', () => {

    it('GET /health — should return 200', () => {
      cy.request({
        method: 'GET',
        url: `${API}/health`,
        failOnStatusCode: false,
      }).then((res) => {
        expect(res.status).to.be.oneOf([200, 204]);
      });
    });
  });

  // ────────────── LOGIN ──────────────
  context('POST /api/identity/login', () => {

    it('should return a JWT token for valid credentials', () => {
      cy.request({
        method: 'POST',
        url: `${API}/api/identity/login`,
        body: {
          email: Cypress.env('ADMIN_EMAIL'),
          password: Cypress.env('ADMIN_PASS'),
        },
      }).then((res) => {
        expect(res.status).to.eq(200);
        expect(res.body).to.have.property('token');
        expect(res.body.token).to.be.a('string').and.not.be.empty;
        authToken = res.body.token;
      });
    });

    it('should return 401 for invalid credentials', () => {
      cy.request({
        method: 'POST',
        url: `${API}/api/identity/login`,
        body: { email: 'wrong@wrong.com', password: 'Wrong!' },
        failOnStatusCode: false,
      }).then((res) => {
        expect(res.status).to.be.oneOf([401, 400]);
      });
    });

    it('should return 400 for missing email field', () => {
      cy.request({
        method: 'POST',
        url: `${API}/api/identity/login`,
        body: { password: 'SomePass' },
        failOnStatusCode: false,
      }).then((res) => {
        expect(res.status).to.be.oneOf([400, 422]);
      });
    });
  });

  // ────────────── PROFILE ──────────────
  context('GET /api/identity/profile', () => {

    beforeEach(() => {
      // Ensure we have a valid token
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

    it('should return the user profile with a valid token', () => {
      cy.request({
        method: 'GET',
        url: `${API}/api/identity/profile`,
        headers: { Authorization: `Bearer ${authToken}` },
      }).then((res) => {
        expect(res.status).to.eq(200);
        expect(res.body).to.have.property('email');
      });
    });

    it('should return 401 without a token', () => {
      cy.request({
        method: 'GET',
        url: `${API}/api/identity/profile`,
        failOnStatusCode: false,
      }).then((res) => {
        expect(res.status).to.eq(401);
      });
    });
  });
});
