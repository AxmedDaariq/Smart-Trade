// ═══════════════════════════════════════════════════════════════════════
// Frontend E2E — Logistics Dashboard
// Tests: Dashboard KPIs, Shipment Table, Corridor Visualization, Filters
// ═══════════════════════════════════════════════════════════════════════

describe('Logistics Dashboard', () => {

  beforeEach(() => {
    cy.login();
    cy.visit('/logistics/dashboard');
  });

  // ────────────── LOADING & LAYOUT ──────────────
  context('Page Load & Layout', () => {

    it('should load the logistics dashboard successfully', () => {
      cy.url().should('include', '/logistics/dashboard');
      cy.get('[data-cy=btn-export-manifest]').should('be.visible');
    });

    it('should display the hero title', () => {
      cy.get('.dashboard-hero-title').should('contain', 'Logistics');
    });

    it('should display the main navigation elements', () => {
      cy.get('[data-cy=nav-ectn]').should('exist');
      cy.get('[data-cy=nav-customs]').should('exist');
      cy.get('[data-cy=nav-payments]').should('exist');
    });
  });

  // ────────────── KPIs ──────────────
  context('KPI Cards', () => {

    it('should display active cargo KPI', () => {
      cy.get('[data-cy=kpi-active-cargo]').should('be.visible');
      cy.get('[data-cy=kpi-active-cargo]').invoke('text').should('not.be.empty');
    });

    it('should display on-time delivery percentage', () => {
      cy.get('[data-cy=kpi-on-time-delivery]').should('be.visible');
      cy.get('[data-cy=kpi-on-time-delivery]').invoke('text').then((text) => {
        expect(text).to.match(/\d+(\.\d+)?%/);
      });
    });
  });

  // ────────────── TRADE CORRIDOR VISUALIZATION ──────────────
  context('Strategic Trade Corridor', () => {

    it('should display corridor nodes with correct statuses', () => {
      cy.get('[data-cy=corridor-node-berbera]').should('exist');
      cy.get('[data-cy=corridor-node-togwajaale]').should('exist');
      cy.get('[data-cy=corridor-node-addis]').should('exist');
    });

    it('should show Berbera as passed node', () => {
      cy.get('[data-cy=corridor-node-berbera]').should('have.class', 'passed');
    });

    it('should show Togwajaale as current node', () => {
      cy.get('[data-cy=corridor-node-togwajaale]').should('have.class', 'current');
    });
  });

  // ────────────── SHIPMENT TABLE & FILTERS ──────────────
  context('Shipment Filtering', () => {

    it('should show all shipments when "All" filter is selected', () => {
      cy.get('[data-cy=filter-all]').click();
      cy.get('[data-cy=shipment-table-row]').should('have.length.at.least', 1);
    });

    it('should filter shipments by "In Transit" status', () => {
      cy.get('[data-cy=filter-in-transit]').click();
      cy.get('[data-cy=shipment-status-badge]').each(($el) => {
        cy.wrap($el).should('contain', 'In Transit');
      });
    });

    it('should filter shipments by "Border Inspection" status', () => {
      cy.get('[data-cy=filter-border]').click();
      cy.get('[data-cy=shipment-status-badge]').each(($el) => {
        cy.wrap($el).should('contain', 'Border Inspection');
      });
    });
  });

  // ────────────── ACTION BUTTONS ──────────────
  context('Action Buttons', () => {

    it('should allow exporting the manifest', () => {
      cy.get('[data-cy=btn-export-manifest]').should('not.be.disabled').click();
    });

    it('should allow dispatching a consignment', () => {
      cy.get('[data-cy=btn-dispatch-consignment]').should('not.be.disabled').click();
    });
  });
});
