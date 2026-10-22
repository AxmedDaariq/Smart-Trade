describe('Logistics Dashboard', () => {
  beforeEach(() => {
    // Intercept shipments API call to ensure stable test execution
    cy.intercept('GET', '**/shipments*', {
      statusCode: 200,
      body: {
        total: 2,
        page: 1,
        pageSize: 10,
        items: [
          {
            id: 'shp-101',
            shipmentNumber: 'SHP-2026-001',
            shipperReference: 'REF-001',
            consigneeReference: 'East Africa Importers',
            origin: 'Berbera Port',
            destination: 'Addis Ababa',
            status: 'In-Transit',
            transportMode: 'Road',
            estimatedArrivalUtc: new Date(Date.now() + 86400000).toISOString(),
            createdAtUtc: new Date().toISOString()
          },
          {
            id: 'shp-102',
            shipmentNumber: 'SHP-2026-002',
            shipperReference: 'REF-002',
            consigneeReference: 'Addis Commodities',
            origin: 'Berbera Port',
            destination: 'Dire Dawa',
            status: 'Border Gate',
            transportMode: 'Road',
            estimatedArrivalUtc: new Date(Date.now() + 172800000).toISOString(),
            createdAtUtc: new Date().toISOString()
          }
        ]
      }
    }).as('getShipments');

    cy.login();
    cy.visit('/logistics/dashboard');
  });

  it('should successfully load the logistics dashboard', () => {
    cy.get('[data-cy=btn-export-manifest]').should('be.visible');
    cy.get('.dashboard-hero-title').should('contain', 'Logistics & Freight Hub');
  });

  it('should display correct KPI values', () => {
    cy.get('[data-cy=kpi-active-cargo]').should('be.visible');
    cy.get('[data-cy=kpi-on-time-delivery]').should('contain', '98.4%');
  });

  it('should verify strategic trade corridor progress', () => {
    cy.get('[data-cy=corridor-node-berbera]').should('have.class', 'passed');
    cy.get('[data-cy=corridor-node-togwajaale]').should('have.class', 'current');
    cy.get('[data-cy=corridor-node-addis]').should('have.class', 'upcoming');
  });

  it('should filter shipments correctly', () => {
    // All
    cy.get('[data-cy=filter-all]').click();
    cy.get('[data-cy=shipment-table-row]').should('have.length.at.least', 1);

    // In Transit
    cy.get('[data-cy=filter-in-transit]').click();
    cy.get('[data-cy=shipment-status-badge]').each(($el) => {
      cy.wrap($el).should('contain', 'In Transit');
    });

    // Border Gate
    cy.get('[data-cy=filter-border]').click();
    cy.get('[data-cy=shipment-status-badge]').each(($el) => {
      cy.wrap($el).should('contain', 'Border Inspection');
    });
  });

  it('should verify action buttons are interactable', () => {
    cy.get('[data-cy=btn-export-manifest]').should('not.be.disabled').click();
    cy.get('[data-cy=btn-dispatch-consignment]').should('not.be.disabled').click();
  });
});
