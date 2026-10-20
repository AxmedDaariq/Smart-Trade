describe('User Onboarding and Identity Flow', () => {
  beforeEach(() => {
    // Intercept registration and login endpoints
    cy.intercept('POST', '**/api/v1/identity/auth/register-tenant', {
      statusCode: 200,
      body: { message: 'Organization successfully registered' }
    }).as('registerTenant');

    cy.intercept('POST', '**/api/v1/identity/auth/login', {
      statusCode: 200,
      body: {
        accessToken: 'mock_onboard_token_123',
        tenantCode: 'NEW-CORP',
        permissions: ['control-tower.read'],
        roles: ['EnterpriseAdmin'],
        tenantType: 'logistics'
      }
    }).as('loginUser');
  });

  it('should complete a full tenant registration and sign in process', () => {
    // 1. Visit Auth Page and switch to registration tab
    cy.visit('/login');
    cy.contains('Register Organization').click();

    // 2. Fill enterprise registration form
    cy.get('[data-cy=reg-org-name]').type('East African Cross-Border Express');
    cy.get('[data-cy=reg-tenant-code]').clear().type('EASTAFR-EXP');
    cy.get('[data-cy=reg-admin-name]').type('Ahmed Ali Daariq');
    cy.get('[data-cy=reg-email]').type('ahmed@eastafricolog.com');
    cy.get('[data-cy=reg-password]').type('P@ssword123!');
    cy.get('[data-cy=reg-submit]').click();

    // 3. Verify success message and transition to login form
    cy.get('.auth-alert').should('be.visible');

    // 4. Log in with registered credentials
    cy.get('[data-cy=login-tenant]').clear().type('EASTAFR-EXP');
    cy.get('[data-cy=login-username]').clear().type('ahmed@eastafricolog.com');
    cy.get('[data-cy=login-password]').clear().type('P@ssword123!');
    cy.get('[data-cy=login-submit]').click();

    // 5. Lands on authenticated operations hub
    cy.url().should('match', /\/(dashboard|logistics)/);
  });
});
