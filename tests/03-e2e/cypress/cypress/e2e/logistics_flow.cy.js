describe('Logistics End-to-End Flow', () => {
  beforeEach(() => {
    // We assume the application is running and the user is logged in
    // In a real scenario, we would use a custom command like cy.login()
    cy.visit('/dashboard'); 
  });

  it('should complete the full logistics chain: Order -> Customs -> Fleet', () => {
    // 1. Create Shipping Order
    cy.get('[data-cy=create-order-btn]').click();
    cy.get('[data-cy=order-customer]').type('Global Trade Corp');
    cy.get('[data-cy=order-item]').type('Industrial Machinery');
    cy.get('[data-cy=order-weight]').type('5000');
    cy.get('[data-cy=submit-order]').click();
    
    cy.contains('Order Created Successfully').should('be.visible');
    cy.get('[data-cy=order-id]').invoke('text').as('orderId');

    // 2. Process Customs Clearance
    cy.get('@orderId').then((id) => {
      cy.visit(`/customs/clearance/${id}`);
      cy.get('[data-cy=customs-status-dropdown]').select('Cleared');
      cy.get('[data-cy=customs-officer-note]').type('All documents verified');
      cy.get('[data-cy=approve-customs]').click();
      
      cy.contains('Customs Cleared').should('be.visible');
    });

    // 3. Assign to Fleet and Dispatch
    cy.get('@orderId').then((id) => {
      cy.visit(`/fleet/assign/${id}`);
      cy.get('[data-cy=truck-select]').select('Truck-A102');
      cy.get('[data-cy=driver-select]').select('John Doe');
      cy.get('[data-cy=dispatch-btn]').click();
      
      cy.contains('Shipment Dispatched').should('be.visible');
    });

    // 4. Final Verification on Dashboard
    cy.visit('/dashboard');
    cy.get('[data-cy=shipment-status-table]').should('contain', 'Dispatched');
  });
});
