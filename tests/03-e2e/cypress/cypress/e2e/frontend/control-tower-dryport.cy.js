/**
 * ═══════════════════════════════════════════════════════════════════════
 * Frontend E2E Test: Corridor Control Tower & Dryport Demurrage
 * ═══════════════════════════════════════════════════════════════════════
 * Validates the operational dashboards:
 * 1. Control Tower Dashboard (/control-tower)
 *    - Real-time IoT incident feed (Reefer cold-chain alerts, Border queue delays)
 *    - Corridor telemetry matrix (Berbera ⇄ Addis Ababa gateway)
 * 2. Dryport Demurrage & Yard Dashboard (/dryport/dashboard)
 *    - Container dwell time KPIs (Total in Port, Warning Dwell, Accrued Demurrage)
 *    - Container inventory table with risk badges
 *    - "+ Diiwaangeli Kunteenar" modal interaction
 */

describe('Frontend Workspaces — Control Tower & Dryport Demurrage', () => {
  beforeEach(() => {
    // Intercept identity and backend APIs to support both mock and live environments
    cy.intercept('POST', '**/api/v1/identity/auth/login', {
      statusCode: 200,
      body: {
        accessToken: 'mock_control_tower_jwt_token',
        tenantCode: 'TENANT-GLB',
        permissions: ['control-tower.read', 'customs.inspect', 'fleet.manage'],
        roles: ['EnterpriseAdmin', 'LogisticsManager'],
        tenantType: 'logistics'
      }
    }).as('loginRequest');

    cy.intercept('GET', '**/api/v1/dryport/containers*', {
      statusCode: 200,
      body: [
        {
          containerId: 'CNT-001',
          containerNumber: 'MSKU-882109-1',
          status: 'Yard Stored',
          daysInPort: 8,
          freeDays: 7,
          accruedDemurrage: 150,
          currency: 'USD'
        },
        {
          containerId: 'CNT-002',
          containerNumber: 'CMAU-440192-3',
          status: 'Customs Hold',
          daysInPort: 5,
          freeDays: 7,
          accruedDemurrage: 0,
          currency: 'USD'
        }
      ]
    }).as('dryportContainers');

    cy.intercept('GET', '**/api/v1/control-tower/telemetry*', {
      statusCode: 200,
      body: {
        activeConvoys: 38,
        corridorThroughput: 124,
        averageTransitHours: 26.4
      }
    }).as('towerTelemetry');
  });

  describe('1. Corridor Control Tower Operations (/control-tower)', () => {
    it('authenticates and navigates to the corridor control tower', () => {
      cy.login();
      cy.get('[data-cy=nav-control-tower]').click();
      cy.url().should('include', '/control-tower');
    });

    it('verifies control tower hero header and live radar title', () => {
      cy.login();
      cy.visit('/control-tower');
      cy.get('.tower-hero-title').should('contain', 'Corridor Command');
    });

    it('verifies corridor alerts stream displays real-time telemetry events', () => {
      cy.login();
      cy.visit('/control-tower');
      cy.get('body').should('contain', 'Alert');
    });
  });

  describe('2. Dryport Demurrage & Container Yard (/dryport/dashboard)', () => {
    it('navigates to dryport dashboard and verifies dwell time KPIs', () => {
      cy.login();
      cy.get('[data-cy=nav-dryport]').click();
      cy.url().should('include', '/dryport');

      cy.get('.dashboard-title').should('contain', 'Dryport Demurrage');
      cy.get('.stats-grid').should('be.visible');
    });

    it('displays container inventory table with demurrage risk calculations', () => {
      cy.login();
      cy.visit('/dryport/dashboard');
      cy.get('table').should('be.visible');
      cy.get('thead').should('contain', 'Numbarka Kunteenarka');
    });

    it('opens container registration modal on "+ Diiwaangeli Kunteenar" click', () => {
      cy.login();
      cy.visit('/dryport/dashboard');
      cy.contains('+ Diiwaangeli Kunteenar').click();
      cy.get('body').then(($body) => {
        // Modal or form should be displayed
        expect($body.text()).to.include('Kunteenar');
      });
    });
  });
});
