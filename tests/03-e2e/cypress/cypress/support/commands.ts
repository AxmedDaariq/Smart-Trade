Cypress.Commands.add('login', (email = 'admin@acme.com', password = 'Admin@123456!') => {
  cy.visit('/login');
  cy.get('[data-cy=login-username]').type(email);
  cy.get('[data-cy=login-password]').type(password);
  cy.get('[data-cy=login-submit]').click();

  // Verify login success by checking for dashboard presence
  cy.url().should('include', '/dashboard');
});
