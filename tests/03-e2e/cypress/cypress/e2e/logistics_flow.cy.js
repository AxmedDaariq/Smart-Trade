describe('Logistics End-to-End Flow', () => {
  beforeEach(() => {
    cy.intercept('GET', '**/shipments*', {
      statusCode: 200,
      body: {
        total: 1,
        page: 1,
        pageSize: 10,
        items: [
          {
            id: 'shp-e2e-01',
            shipmentNumber: 'SHP-2026-9901',
            shipperReference: 'REF-EXP-01',
            consigneeReference: 'Addis Importers',
            origin: 'Berbera Port',
            destination: 'Addis Ababa',
            status: 'In-Transit',
            transportMode: 'Multimodal',
            estimatedArrivalUtc: new Date(Date.now() + 86400000).toISOString(),
            createdAtUtc: new Date().toISOString()
          }
        ]
      }
    }).as('getShipments');

    cy.login();
    cy.visit('/logistics/dashboard');
  });

  it('should navigate through logistics freight portal and interact with dispatch and export', () => {
    // 1. Verify freight portal is active
    cy.get('.dashboard-hero-title').should('contain', 'Logistics');

    // 2. Export manifest action
    cy.get('[data-cy=btn-export-manifest]').should('be.visible').click();

    // 3. Dispatch new consignment action
    cy.get('[data-cy=btn-dispatch-consignment]').should('be.visible').click();

    // 4. Inspect active shipment row
    cy.get('[data-cy=shipment-table-row]').first().within(() => {
      cy.get('[data-cy=shipment-status-badge]').should('contain', 'In Transit');
    });
  });
});
