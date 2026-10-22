// ═══════════════════════════════════════════════════════════════════════
// Frontend E2E — Authentication Flow
// Tests: Login, Registration, Logout, Token Refresh, Role-based Access
// ═══════════════════════════════════════════════════════════════════════

describe('Authentication Flow', () => {

  beforeEach(() => {
    cy.clearLocalStorage();
    cy.clearCookies();
  });

  // ────────────── LOGIN ──────────────
  context('Login', () => {

    it('should display the login page correctly', () => {
      cy.visit('/login');
      cy.get('[data-cy=login-username]').should('be.visible');
      cy.get('[data-cy=login-password]').should('be.visible');
      cy.get('[data-cy=login-submit]').should('be.visible');
    });

    it('should login with valid admin credentials and redirect to dashboard', () => {
      cy.fixture('users').then((users) => {
        cy.visit('/login');
        cy.get('[data-cy=login-username]').type(users.admin.email);
        cy.get('[data-cy=login-password]').type(users.admin.password);
        cy.get('[data-cy=login-submit]').click();

        cy.url().should('include', '/dashboard', { timeout: 15000 });
        cy.window().its('localStorage.auth_token').should('exist');
      });
    });

    it('should show an error message for invalid credentials', () => {
      cy.visit('/login');
      cy.get('[data-cy=login-username]').type('wrong@email.com');
      cy.get('[data-cy=login-password]').type('WrongPassword!');
      cy.get('[data-cy=login-submit]').click();

      cy.contains(/invalid|incorrect|failed/i).should('be.visible');
      cy.url().should('include', '/login');
    });

    it('should validate that email field is required', () => {
      cy.visit('/login');
      cy.get('[data-cy=login-password]').type('SomePassword');
      cy.get('[data-cy=login-submit]').click();

      // Should remain on login page
      cy.url().should('include', '/login');
    });
  });

  // ────────────── REGISTRATION ──────────────
  context('Registration', () => {

    it('should register a new user and show verification prompt', () => {
      cy.fixture('users').then((users) => {
        const u = users.new_user;
        cy.visit('/register');

        cy.get('[data-cy=reg-username]').type(u.username);
        cy.get('[data-cy=reg-email]').type(u.email);
        cy.get('[data-cy=reg-password]').type(u.password);
        cy.get('[data-cy=reg-password-confirm]').type(u.password);
        cy.get('[data-cy=reg-submit]').click();

        cy.contains('Registration Successful').should('be.visible');
      });
    });

    it('should reject registration with mismatched passwords', () => {
      cy.visit('/register');
      cy.get('[data-cy=reg-username]').type('mismatch_user');
      cy.get('[data-cy=reg-email]').type('mismatch@test.com');
      cy.get('[data-cy=reg-password]').type('Password123!');
      cy.get('[data-cy=reg-password-confirm]').type('DifferentPassword!');
      cy.get('[data-cy=reg-submit]').click();

      cy.contains(/match|mismatch/i).should('be.visible');
    });
  });

  // ────────────── LOGOUT ──────────────
  context('Logout', () => {

    it('should logout and redirect to login page', () => {
      cy.login();
      cy.visit('/dashboard');
      cy.logout();

      cy.url().should('include', '/login');
      cy.window().then((win) => {
        expect(win.localStorage.getItem('auth_token')).to.be.null;
      });
    });
  });

  // ────────────── ROLE-BASED ACCESS ──────────────
  context('Role-Based Access Control', () => {

    it('should deny access to Control Tower for users without permission', () => {
      cy.fixture('users').then((users) => {
        cy.login(users.operator.email, users.operator.password);
        cy.visit('/control-tower');

        // Should either redirect to 403 or show forbidden message
        cy.url().then((url) => {
          if (url.includes('/403')) {
            cy.contains('403').should('be.visible');
          } else {
            cy.contains(/forbidden|unauthorized|permission/i).should('be.visible');
          }
        });
      });
    });

    it('should allow admin to access all protected routes', () => {
      cy.login();

      const protectedRoutes = [
        '/logistics/dashboard',
        '/government/dashboard',
        '/dryport/dashboard',
      ];

      protectedRoutes.forEach((route) => {
        cy.visit(route);
        cy.url().should('include', route);
        // Should not be redirected to login or 403
        cy.url().should('not.include', '/login');
        cy.url().should('not.include', '/403');
      });
    });
  });

  // ────────────── UNAUTHENTICATED REDIRECT ──────────────
  context('Protected Route Redirect', () => {

    it('should redirect to login when visiting a protected route unauthenticated', () => {
      cy.visit('/logistics/dashboard');
      cy.url().should('include', '/login');
    });
  });
});
