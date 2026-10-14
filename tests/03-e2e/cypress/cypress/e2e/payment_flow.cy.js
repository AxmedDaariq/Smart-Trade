describe('Payment and Financial Flow', () => {
  beforeEach(() => {
    // Assume user is already logged in and has an order ready for payment
    cy.visit('/dashboard');
  });

  it('should complete a full payment process for a shipment', () => {
    // 1. Navigate to Payments/Invoices
    cy.get('[data-cy=nav-payments]').click();
    
    // 2. Select a pending invoice for a shipment
    cy.get('[data-cy=payment-row-pending]').first().click();
    cy.get('[data-cy=invoice-id]').invoke('text').as('invoiceId');
    
    // 3. Initiate Payment
    cy.get('[data-cy=pay-now-btn]').click();
    
    // 4. Select Payment Method (e.g., Credit Card)
    cy.get('[data-cy=payment-method-card]').click();
    cy.get('[data-cy=card-number]').type('4242424242424242');
    cy.get('[data-cy=card-expiry]').type('12/28');
    cy.get('[data-cy=card-cvv]').type('123');
    
    cy.get('[data-cy=confirm-payment-btn]').click();
    
    // 5. Verify Payment Success
    cy.contains('Payment Processed Successfully').should('be.visible');
    cy.get('[data-cy=transaction-id]').should('not.be.empty');
    
    // 6. Verify Invoice Status Update
    cy.visit('/payments/invoices');
    cy.get('@invoiceId').then((id) => {
      cy.contains(id).parents('[data-cy=invoice-row]').within(() => {
        cy.get('[data-cy=invoice-status]').should('contain', 'Paid');
      });
    });

    // 7. Verify Shipment Release (Integration check with Shipping/Customs)
    cy.visit('/dashboard');
    cy.contains('Payment Verified').should('be.visible');
    cy.get('[data-cy=shipment-release-status]').should('contain', 'Released for Delivery');
  });
});
