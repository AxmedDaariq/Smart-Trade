// ═══════════════════════════════════════════════════════════════════════
// Frontend E2E — Shipping, Fleet & Payment Flows
// Tests: Order Creation, Fleet Assignment, Payment Processing
// ═══════════════════════════════════════════════════════════════════════

describe('Shipping, Fleet & Payment Flows', () => {

  beforeEach(() => {
    cy.login();
  });

  // ────────────── SHIPPING ORDER ──────────────
  context('Shipping Order Creation', () => {

    it('should create a new shipping order', () => {
      cy.fixture('shipments').then((data) => {
        const s = data.newShipment;

        cy.visit('/dashboard');
        cy.get('[data-cy=create-order-btn]').click();

        cy.get('[data-cy=order-customer]').type(s.customer);
        cy.get('[data-cy=order-item]').type(s.item);
        cy.get('[data-cy=order-weight]').type(String(s.weight));
        cy.get('[data-cy=submit-order]').click();

        cy.contains('Order Created Successfully').should('be.visible');
        cy.get('[data-cy=order-id]').invoke('text').should('not.be.empty');
      });
    });

    it('should display the newly created order in the shipment table', () => {
      cy.fixture('shipments').then((data) => {
        const s = data.newShipment;

        cy.visit('/dashboard');
        cy.get('[data-cy=create-order-btn]').click();
        cy.get('[data-cy=order-customer]').type(s.customer);
        cy.get('[data-cy=order-item]').type(s.item);
        cy.get('[data-cy=order-weight]').type(String(s.weight));
        cy.get('[data-cy=submit-order]').click();
        cy.contains('Order Created Successfully').should('be.visible');

        // Navigate back to dashboard and verify
        cy.visit('/dashboard');
        cy.get('[data-cy=shipment-status-table]').should('contain', s.customer);
      });
    });
  });

  // ────────────── CUSTOMS CLEARANCE ──────────────
  context('Customs Clearance for Order', () => {

    it('should process customs clearance for a shipping order', () => {
      cy.visit('/dashboard');
      cy.get('[data-cy=create-order-btn]').click();
      cy.get('[data-cy=order-customer]').type('Clearance Test Corp');
      cy.get('[data-cy=order-item]').type('Steel Rods');
      cy.get('[data-cy=order-weight]').type('8000');
      cy.get('[data-cy=submit-order]').click();

      cy.get('[data-cy=order-id]').invoke('text').then((orderId) => {
        cy.visit(`/customs/clearance/${orderId}`);
        cy.get('[data-cy=customs-status-dropdown]').select('Cleared');
        cy.get('[data-cy=customs-officer-note]').type('All documents verified — E2E test');
        cy.get('[data-cy=approve-customs]').click();

        cy.contains('Customs Cleared').should('be.visible');
      });
    });
  });

  // ────────────── FLEET ASSIGNMENT & DISPATCH ──────────────
  context('Fleet Assignment & Dispatch', () => {

    it('should assign a truck and driver then dispatch', () => {
      // Create order first
      cy.visit('/dashboard');
      cy.get('[data-cy=create-order-btn]').click();
      cy.get('[data-cy=order-customer]').type('Fleet Test Inc');
      cy.get('[data-cy=order-item]').type('Construction Materials');
      cy.get('[data-cy=order-weight]').type('15000');
      cy.get('[data-cy=submit-order]').click();

      cy.get('[data-cy=order-id]').invoke('text').then((orderId) => {
        cy.visit(`/fleet/assign/${orderId}`);
        cy.get('[data-cy=truck-select]').select('Truck-A102');
        cy.get('[data-cy=driver-select]').select('John Doe');
        cy.get('[data-cy=dispatch-btn]').click();

        cy.contains('Shipment Dispatched').should('be.visible');
      });
    });

    it('should show dispatched status on the dashboard', () => {
      cy.visit('/dashboard');
      cy.get('[data-cy=shipment-status-table]').should('contain', 'Dispatched');
    });
  });

  // ────────────── PAYMENT PROCESSING ──────────────
  context('Payment Flow', () => {

    it('should process a credit card payment for a pending invoice', () => {
      cy.fixture('customs-and-payments').then((data) => {
        const card = data.paymentMethods.creditCard;

        cy.visit('/dashboard');
        cy.get('[data-cy=nav-payments]').click();

        cy.get('[data-cy=payment-row-pending]').first().click();
        cy.get('[data-cy=invoice-id]').invoke('text').as('invoiceId');

        cy.get('[data-cy=pay-now-btn]').click();
        cy.get('[data-cy=payment-method-card]').click();
        cy.get('[data-cy=card-number]').type(card.cardNumber);
        cy.get('[data-cy=card-expiry]').type(card.expiry);
        cy.get('[data-cy=card-cvv]').type(card.cvv);
        cy.get('[data-cy=confirm-payment-btn]').click();

        cy.contains('Payment Processed Successfully').should('be.visible');
        cy.get('[data-cy=transaction-id]').should('not.be.empty');
      });
    });

    it('should update invoice status to Paid after payment', () => {
      cy.fixture('customs-and-payments').then((data) => {
        const card = data.paymentMethods.creditCard;

        cy.visit('/dashboard');
        cy.get('[data-cy=nav-payments]').click();
        cy.get('[data-cy=payment-row-pending]').first().click();
        cy.get('[data-cy=invoice-id]').invoke('text').as('invoiceId');

        cy.get('[data-cy=pay-now-btn]').click();
        cy.get('[data-cy=payment-method-card]').click();
        cy.get('[data-cy=card-number]').type(card.cardNumber);
        cy.get('[data-cy=card-expiry]').type(card.expiry);
        cy.get('[data-cy=card-cvv]').type(card.cvv);
        cy.get('[data-cy=confirm-payment-btn]').click();
        cy.contains('Payment Processed Successfully').should('be.visible');

        // Navigate to invoices and verify status
        cy.visit('/payments/invoices');
        cy.get('@invoiceId').then((id) => {
          cy.contains(id).parents('[data-cy=invoice-row]').within(() => {
            cy.get('[data-cy=invoice-status]').should('contain', 'Paid');
          });
        });
      });
    });

    it('should verify shipment release after successful payment', () => {
      cy.visit('/dashboard');
      cy.get('[data-cy=shipment-release-status]').should('contain', 'Released for Delivery');
    });
  });
});
