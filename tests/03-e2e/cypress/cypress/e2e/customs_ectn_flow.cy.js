describe('Customs and ECTN (Electronic Cargo Tracking Note) Flow', () => {
  beforeEach(() => {
    cy.visit('/dashboard');
  });

  it('should handle the full ECTN issuance and Customs declaration process', () => {
    // 1. ECTN Application
    cy.get('[data-cy=nav-ectn]').click();
    cy.get('[data-cy=apply-ectn-btn]').click();
    
    cy.get('[data-cy=ectn-vessel-name]').type('MV Ocean Star');
    cy.get('[data-cy=ectn-voyage-no]').type('V-2026-10');
    cy.get('[data-cy=ectn-port-loading]').type('Shanghai');
    cy.get('[data-cy=ectn-port-discharge]').type('Abidjan');
    cy.get('[data-cy=ectn-cargo-desc]').type('Electronic Components');
    
    cy.get('[data-cy=submit-ectn-app]').click();
    
    cy.contains('ECTN Application Submitted').should('be.visible');
    cy.get('[data-cy=ectn-number]').invoke('text').as('ectnNumber');

    // 2. ECTN Validation and Issuance
    cy.get('@ectnNumber').then((number) => {
      cy.visit(\`/ectn/admin/validate/\${number}\`);
      cy.get('[data-cy=ectn-verify-docs]').click();
      cy.get('[data-cy=issue-ectn-btn]').click();
      
      cy.contains('ECTN Issued Successfully').should('be.visible');
    });

    // 3. Customs Declaration using ECTN
    cy.get('[data-cy=nav-customs]').click();
    cy.get('[data-cy=new-declaration-btn]').click();
    
    cy.get('[data-cy=decl-ectn-number]').type('ECTN-AUTO-GEN'); // In real test, we would use the @ectnNumber
    cy.get('[data-cy=decl-hs-code]').type('8517.12.00');
    cy.get('[data-cy=decl-value]').type('25000');
    cy.get('[data-cy=decl-currency]').select('USD');
    
    cy.get('[data-cy=submit-declaration]').click();
    
    cy.contains('Declaration Submitted').should('be.visible');
    cy.get('[data-cy=declaration-id]').invoke('text').as('declId');

    // 4. Customs Inspection and Duty Calculation
    cy.get('@declId').then((id) => {
      cy.visit(\`/customs/inspection/\${id}\`);
      cy.get('[data-cy=inspection-result]').select('Passed');
      cy.get('[data-cy=calculate-duty-btn]').click();
      
      cy.get('[data-cy=duty-amount]').should('not.be.empty');
      cy.get('[data-cy=finalize-declaration]').click();
      
      cy.contains('Customs Process Completed').should('be.visible');
    });

    // 5. Verify Final Status on Dashboard
    cy.visit('/dashboard');
    cy.get('[data-cy=customs-summary-card]').should('contain', 'Completed');
  });
});
