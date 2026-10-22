// ═══════════════════════════════════════════════════════════════════════
// Frontend E2E — Customs & ECTN Module
// Tests: ECTN Application, Validation, Customs Declaration, Duty
// ═══════════════════════════════════════════════════════════════════════

describe('Customs & ECTN Module', () => {

  beforeEach(() => {
    cy.login();
  });

  // ────────────── CUSTOMS DASHBOARD ──────────────
  context('Customs Dashboard', () => {

    it('should load the government/customs dashboard', () => {
      cy.visit('/government/dashboard');
      cy.url().should('include', '/government/dashboard');
    });
  });

  // ────────────── ECTN APPLICATION ──────────────
  context('ECTN Application Flow', () => {

    it('should submit a new ECTN application', () => {
      cy.fixture('shipments').then((data) => {
        const ectn = data.ectnApplication;

        cy.visit('/dashboard');
        cy.get('[data-cy=nav-ectn]').click();
        cy.get('[data-cy=apply-ectn-btn]').click();

        cy.get('[data-cy=ectn-vessel-name]').type(ectn.vesselName);
        cy.get('[data-cy=ectn-voyage-no]').type(ectn.voyageNo);
        cy.get('[data-cy=ectn-port-loading]').type(ectn.portOfLoading);
        cy.get('[data-cy=ectn-port-discharge]').type(ectn.portOfDischarge);
        cy.get('[data-cy=ectn-cargo-desc]').type(ectn.cargoDescription);

        cy.get('[data-cy=submit-ectn-app]').click();
        cy.contains('ECTN Application Submitted').should('be.visible');

        // Capture the generated ECTN number
        cy.get('[data-cy=ectn-number]').invoke('text').as('ectnNumber');
      });
    });

    it('should validate and issue an ECTN (admin perspective)', () => {
      // First submit an application
      cy.fixture('shipments').then((data) => {
        const ectn = data.ectnApplication;

        cy.visit('/dashboard');
        cy.get('[data-cy=nav-ectn]').click();
        cy.get('[data-cy=apply-ectn-btn]').click();

        cy.get('[data-cy=ectn-vessel-name]').type(ectn.vesselName);
        cy.get('[data-cy=ectn-voyage-no]').type(ectn.voyageNo);
        cy.get('[data-cy=ectn-port-loading]').type(ectn.portOfLoading);
        cy.get('[data-cy=ectn-port-discharge]').type(ectn.portOfDischarge);
        cy.get('[data-cy=ectn-cargo-desc]').type(ectn.cargoDescription);

        cy.get('[data-cy=submit-ectn-app]').click();
        cy.contains('ECTN Application Submitted').should('be.visible');

        cy.get('[data-cy=ectn-number]').invoke('text').then((number) => {
          // Navigate to admin validation page
          cy.visit(`/ectn/admin/validate/${number}`);
          cy.get('[data-cy=ectn-verify-docs]').click();
          cy.get('[data-cy=issue-ectn-btn]').click();

          cy.contains('ECTN Issued Successfully').should('be.visible');
        });
      });
    });
  });

  // ────────────── CUSTOMS DECLARATION ──────────────
  context('Customs Declaration Flow', () => {

    it('should create and submit a customs declaration', () => {
      cy.fixture('customs-and-payments').then((data) => {
        const decl = data.customsDeclaration;

        cy.visit('/dashboard');
        cy.get('[data-cy=nav-customs]').click();
        cy.get('[data-cy=new-declaration-btn]').click();

        cy.get('[data-cy=decl-ectn-number]').type('ECTN-E2E-TEST');
        cy.get('[data-cy=decl-hs-code]').type(decl.hsCode);
        cy.get('[data-cy=decl-value]').type(String(decl.declaredValue));
        cy.get('[data-cy=decl-currency]').select(decl.currency);

        cy.get('[data-cy=submit-declaration]').click();
        cy.contains('Declaration Submitted').should('be.visible');

        cy.get('[data-cy=declaration-id]').invoke('text').as('declId');
      });
    });

    it('should process customs inspection and calculate duty', () => {
      cy.fixture('customs-and-payments').then((data) => {
        const decl = data.customsDeclaration;

        // Create declaration first
        cy.visit('/dashboard');
        cy.get('[data-cy=nav-customs]').click();
        cy.get('[data-cy=new-declaration-btn]').click();
        cy.get('[data-cy=decl-ectn-number]').type('ECTN-INSPECT-E2E');
        cy.get('[data-cy=decl-hs-code]').type(decl.hsCode);
        cy.get('[data-cy=decl-value]').type(String(decl.declaredValue));
        cy.get('[data-cy=decl-currency]').select(decl.currency);
        cy.get('[data-cy=submit-declaration]').click();

        cy.get('[data-cy=declaration-id]').invoke('text').then((id) => {
          cy.visit(`/customs/inspection/${id}`);
          cy.get('[data-cy=inspection-result]').select('Passed');
          cy.get('[data-cy=calculate-duty-btn]').click();

          cy.get('[data-cy=duty-amount]').should('not.be.empty');
          cy.get('[data-cy=finalize-declaration]').click();

          cy.contains('Customs Process Completed').should('be.visible');
        });
      });
    });
  });

  // ────────────── DRYPORT MODULE ──────────────
  context('Dryport Dashboard', () => {

    it('should load the dryport dashboard', () => {
      cy.visit('/dryport/dashboard');
      cy.url().should('include', '/dryport/dashboard');
    });

    it('should display container yard information', () => {
      cy.visit('/dryport/dashboard');
      // The dryport dashboard should show container data
      cy.get('body').should('not.be.empty');
    });
  });
});
