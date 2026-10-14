describe('User Onboarding and Identity Flow', () => {
  
  it('should complete a full user registration and identity verification process', () => {
    // 1. Registration Page
    cy.visit('/register');
    
    cy.get('[data-cy=reg-username]').type('testuser_smarttrade');
    cy.get('[data-cy=reg-email]').type('testuser@smarttrade.com');
    cy.get('[data-cy=reg-password]').type('SecurePassword123!');
    cy.get('[data-cy=reg-password-confirm]').type('SecurePassword123!');
    cy.get('[data-cy=reg-submit]').click();
    
    cy.contains('Registration Successful').should('be.visible');
    cy.contains('Please verify your identity').should('be.visible');

    // 2. Identity Verification (KYC Process)
    cy.get('[data-cy=verify-identity-btn]').click();
    
    // Simulating upload of ID documents
    cy.get('[data-cy=id-upload]').selectFile('cypress/fixtures/sample-passport.pdf', { force: true });
    cy.get('[data-cy=tax-id]').type('TAX-99887766');
    cy.get('[data-cy=submit-verification]').click();
    
    cy.contains('Documents Submitted').should('be.visible');
    cy.contains('Verification in Progress').should('be.visible');

    // 3. Login with new credentials
    cy.visit('/login');
    cy.get('[data-cy=login-username]').type('testuser_smarttrade');
    cy.get('[data-cy=login-password]').type('SecurePassword123!');
    cy.get('[data-cy=login-submit]').click();
    
    // 4. Profile Completion
    cy.visit('/profile/complete');
    cy.get('[data-cy=profile-full-name]').type('Test User One');
    cy.get('[data-cy=profile-phone]').type('+1234567890');
    cy.get('[data-cy=profile-company]').type('Test Logistics Ltd');
    cy.get('[data-cy=save-profile]').click();
    
    cy.contains('Profile Updated').should('be.visible');
    
    // 5. Final Landing Page Verification
    cy.visit('/dashboard');
    cy.get('[data-cy=user-welcome-msg]').should('contain', 'Welcome, Test User One');
  });
});
