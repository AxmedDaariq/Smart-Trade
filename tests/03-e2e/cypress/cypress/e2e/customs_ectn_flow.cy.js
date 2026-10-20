describe('Customs and ECTN (Electronic Cargo Tracking Note) Flow', () => {
  beforeEach(() => {
    cy.intercept('POST', '**/api/v1/ectn/applications*', {
      statusCode: 201,
      body: {
        id: 'ectn-app-001',
        certificateNumber: 'ECTN-2026-9921',
        status: 'Issued',
        vesselName: 'MV Ocean Star',
        voyageNo: 'V-2026-10'
      }
    }).as('applyEctn');

    cy.intercept('POST', '**/api/v1/customs/declarations*', {
      statusCode: 201,
      body: {
        id: 'decl-001',
        declarationNumber: 'DEC-2026-0881',
        status: 'Green Channel',
        assessedDuties: 12500,
        currency: 'USD'
      }
    }).as('submitDeclaration');

    cy.login();
    cy.visit('/government/dashboard');
  });

  it('should verify customs authority portal and declaration workflow', () => {
    // 1. Verify Customs Portal Header & WCO compliance
    cy.get('.customs-hero-title').should('contain', 'Customs Authority');
    cy.get('.risk-channels-panel').should('be.visible');

    // 2. Risk channel matrix inspection
    cy.get('.channel-green').should('contain', 'Green Lane');
    cy.get('.channel-yellow').should('contain', 'Yellow Lane');
    cy.get('.channel-red').should('contain', 'Red Lane');

    // 3. Search and filter active customs declaration cases
    cy.get('.panel-toolbar input').clear().type('DEC');
    cy.get('.enterprise-table').should('be.visible');
  });

  it('should interact with inward declaration filing action', () => {
    cy.contains('File Inward Declaration').click();
    cy.get('body').should('be.visible');
  });
});
