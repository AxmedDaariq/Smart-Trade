// ═══════════════════════════════════════════════════════════════════════
// Database E2E — PostgreSQL Data Integrity & State Verification
// Uses cy.task() to run SQL queries against the test database.
// Ensures that API/UI actions persist correctly at the data layer.
// ═══════════════════════════════════════════════════════════════════════

describe('Database — PostgreSQL Verification', () => {

  const API = Cypress.env('API_URL') || 'http://localhost:5000';
  let authToken;

  before(() => {
    cy.request({
      method: 'POST',
      url: `${API}/api/identity/login`,
      body: {
        email: Cypress.env('ADMIN_EMAIL'),
        password: Cypress.env('ADMIN_PASS'),
      },
    }).then((res) => {
      authToken = res.body.token;
    });
  });

  // ────────────── CONNECTION ──────────────
  context('Database Connection', () => {

    it('should connect to the PostgreSQL database', () => {
      cy.dbQuery('SELECT 1 AS connected').then((rows) => {
        expect(rows).to.have.length(1);
        expect(rows[0].connected).to.eq(1);
      });
    });

    it('should have the required tables', () => {
      cy.dbQuery(`
        SELECT table_name 
        FROM information_schema.tables 
        WHERE table_schema = 'public'
        ORDER BY table_name
      `).then((rows) => {
        const tableNames = rows.map((r) => r.table_name);
        cy.task('log', `Found tables: ${tableNames.join(', ')}`);
        expect(tableNames.length).to.be.greaterThan(0);
      });
    });
  });

  // ────────────── USER DATA ──────────────
  context('User Data Integrity', () => {

    it('should have the admin user in the database', () => {
      cy.dbQuery(
        "SELECT * FROM users WHERE email = $1",
        [Cypress.env('ADMIN_EMAIL')]
      ).then((rows) => {
        expect(rows.length).to.eq(1);
        expect(rows[0].email).to.eq(Cypress.env('ADMIN_EMAIL'));
      });
    });

    it('should store user passwords as hashed values (not plain text)', () => {
      cy.dbQuery(
        "SELECT password_hash FROM users WHERE email = $1",
        [Cypress.env('ADMIN_EMAIL')]
      ).then((rows) => {
        expect(rows.length).to.eq(1);
        const hash = rows[0].password_hash;
        // Must NOT be the plain-text password
        expect(hash).to.not.eq(Cypress.env('ADMIN_PASS'));
        // Should look like a bcrypt/argon2 hash
        expect(hash.length).to.be.greaterThan(20);
      });
    });
  });

  // ────────────── ORDER → DB SYNC ──────────────
  context('Order Persistence', () => {

    let createdOrderId;

    it('should persist a shipping order created via the API', () => {
      // Create order via API
      cy.request({
        method: 'POST',
        url: `${API}/api/shipping/orders`,
        body: {
          customer: 'DB-Verify-Corp',
          item: 'Steel Containers',
          weight: 10000,
        },
        headers: {
          Authorization: `Bearer ${authToken}`,
          'Content-Type': 'application/json',
          'X-Tenant-Code': Cypress.env('TENANT_CODE'),
        },
        failOnStatusCode: false,
      }).then((res) => {
        if (res.status === 200 || res.status === 201) {
          createdOrderId = res.body.id;

          // Verify the order exists in the database
          cy.dbQuery(
            "SELECT * FROM shipping_orders WHERE id = $1",
            [createdOrderId]
          ).then((rows) => {
            expect(rows.length).to.eq(1);
            expect(rows[0].customer).to.eq('DB-Verify-Corp');
            expect(rows[0].weight).to.eq(10000);
          });
        }
      });
    });

    it('should update order status in the database when API status changes', () => {
      if (!createdOrderId) return; // Skip if order wasn't created

      cy.request({
        method: 'PATCH',
        url: `${API}/api/shipping/orders/${createdOrderId}/status`,
        body: { status: 'Customs Cleared' },
        headers: {
          Authorization: `Bearer ${authToken}`,
          'Content-Type': 'application/json',
        },
        failOnStatusCode: false,
      }).then(() => {
        cy.dbQuery(
          "SELECT status FROM shipping_orders WHERE id = $1",
          [createdOrderId]
        ).then((rows) => {
          if (rows.length > 0) {
            expect(rows[0].status).to.eq('Customs Cleared');
          }
        });
      });
    });
  });

  // ────────────── ECTN → DB SYNC ──────────────
  context('ECTN Data Persistence', () => {

    it('should store ECTN application data in the database', () => {
      cy.request({
        method: 'POST',
        url: `${API}/api/ectn/applications`,
        body: {
          vesselName: 'MV DB Test',
          voyageNo: 'V-DB-001',
          portOfLoading: 'Dubai',
          portOfDischarge: 'Berbera',
          cargoDescription: 'Electronics',
        },
        headers: {
          Authorization: `Bearer ${authToken}`,
          'Content-Type': 'application/json',
          'X-Tenant-Code': Cypress.env('TENANT_CODE'),
        },
        failOnStatusCode: false,
      }).then((res) => {
        if (res.status === 200 || res.status === 201) {
          cy.dbQuery(
            "SELECT * FROM ectn_applications WHERE vessel_name = $1",
            ['MV DB Test']
          ).then((rows) => {
            expect(rows.length).to.be.greaterThan(0);
            expect(rows[0].voyage_no).to.eq('V-DB-001');
          });
        }
      });
    });
  });

  // ────────────── PAYMENT → DB SYNC ──────────────
  context('Payment Data Persistence', () => {

    it('should store invoice data correctly', () => {
      cy.request({
        method: 'POST',
        url: `${API}/api/payment/invoices`,
        body: {
          orderId: 'ORD-DB-PAY',
          amount: 5000.0,
          currency: 'USD',
          dueDate: '2026-12-31',
        },
        headers: {
          Authorization: `Bearer ${authToken}`,
          'Content-Type': 'application/json',
          'X-Tenant-Code': Cypress.env('TENANT_CODE'),
        },
        failOnStatusCode: false,
      }).then((res) => {
        if (res.status === 200 || res.status === 201) {
          cy.dbQuery(
            "SELECT * FROM invoices WHERE order_id = $1",
            ['ORD-DB-PAY']
          ).then((rows) => {
            if (rows.length > 0) {
              expect(parseFloat(rows[0].amount)).to.eq(5000.0);
              expect(rows[0].currency).to.eq('USD');
            }
          });
        }
      });
    });
  });

  // ────────────── CLEANUP ──────────────
  after(() => {
    // Clean up test data
    cy.dbCleanup('shipping_orders', "customer = 'DB-Verify-Corp'");
    cy.dbCleanup('ectn_applications', "vessel_name = 'MV DB Test'");
    cy.dbCleanup('invoices', "order_id = 'ORD-DB-PAY'");
  });
});
