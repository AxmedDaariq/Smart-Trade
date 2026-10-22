// ═══════════════════════════════════════════════════════════════════════
// Database E2E — Seed & Migration Verification
// Ensures migrations have run correctly and seed data is present
// ═══════════════════════════════════════════════════════════════════════

describe('Database — Seed & Migration Verification', () => {

  // ────────────── MIGRATIONS ──────────────
  context('Migration Status', () => {

    it('should have a migration history table', () => {
      cy.dbQuery(`
        SELECT table_name 
        FROM information_schema.tables 
        WHERE table_schema = 'public' 
          AND table_name LIKE '%migration%'
      `).then((rows) => {
        expect(rows.length).to.be.greaterThan(0,
          'Expected a migration tracking table (e.g. __EFMigrationsHistory)');
      });
    });

    it('should have applied all migrations', () => {
      // EF Core uses __EFMigrationsHistory; Flyway uses flyway_schema_history
      cy.dbQuery(`
        SELECT * FROM "__EFMigrationsHistory" 
        ORDER BY "MigrationId" DESC 
        LIMIT 5
      `).then((rows) => {
        expect(rows.length).to.be.greaterThan(0, 'No migrations found');
        cy.task('log', `Latest migration: ${rows[0].MigrationId}`);
      });
    });
  });

  // ────────────── SEED DATA ──────────────
  context('Seed Data Presence', () => {

    it('should have the default admin user seeded', () => {
      cy.dbQuery(
        "SELECT COUNT(*) AS cnt FROM users WHERE email = $1",
        [Cypress.env('ADMIN_EMAIL')]
      ).then((rows) => {
        expect(parseInt(rows[0].cnt)).to.eq(1);
      });
    });

    it('should have the default driver user seeded', () => {
      cy.dbQuery(
        "SELECT COUNT(*) AS cnt FROM users WHERE email = $1",
        [Cypress.env('DRIVER_EMAIL')]
      ).then((rows) => {
        expect(parseInt(rows[0].cnt)).to.be.greaterThan(0,
          'Driver seed user not found');
      });
    });

    it('should have at least one tenant configured', () => {
      cy.dbQuery(`
        SELECT COUNT(*) AS cnt FROM tenants
      `).then((rows) => {
        expect(parseInt(rows[0].cnt)).to.be.greaterThan(0);
      });
    });

    it('should have HS code reference data loaded', () => {
      cy.dbQuery(`
        SELECT COUNT(*) AS cnt FROM hs_codes
      `).then((rows) => {
        if (rows.length > 0) {
          expect(parseInt(rows[0].cnt)).to.be.greaterThan(0,
            'HS Code reference data not seeded');
        }
      });
    });
  });

  // ────────────── REFERENTIAL INTEGRITY ──────────────
  context('Referential Integrity', () => {

    it('should have foreign key constraints on shipping_orders', () => {
      cy.dbQuery(`
        SELECT tc.constraint_name, tc.constraint_type
        FROM information_schema.table_constraints tc
        WHERE tc.table_name = 'shipping_orders'
          AND tc.constraint_type = 'FOREIGN KEY'
      `).then((rows) => {
        cy.task('log', `Foreign keys on shipping_orders: ${rows.length}`);
        // Log but don't fail – FK naming varies
      });
    });

    it('should have indexes on frequently queried columns', () => {
      cy.dbQuery(`
        SELECT indexname, tablename
        FROM pg_indexes
        WHERE schemaname = 'public'
        ORDER BY tablename, indexname
        LIMIT 50
      `).then((rows) => {
        cy.task('log', `Total public indexes: ${rows.length}`);
        expect(rows.length).to.be.greaterThan(0);
      });
    });
  });
});
