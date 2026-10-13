import { defineConfig } from 'cypress';

export default defineConfig({
  e2e: {
    baseUrl: 'http://localhost:4200',
    specPattern: 'e2e/**/*.cy.ts',
    supportFile: 'e2e/support/commands.ts',
    fixturesFolder: 'fixtures',
    video: true,
    screenshotOnRunFailure: true,
    viewportWidth: 1440,
    viewportHeight: 900,
    defaultCommandTimeout: 8000,
    requestTimeout: 10000,

    setupNodeEvents(on, config) {
      // Task: read environment overrides from CI env vars
      config.env.API_URL      = process.env.API_URL      ?? 'http://localhost:5000';
      config.env.TENANT_CODE  = process.env.TENANT_CODE  ?? 'ACME';
      config.env.ADMIN_EMAIL  = process.env.ADMIN_EMAIL  ?? 'admin@acme.com';
      config.env.ADMIN_PASS   = process.env.ADMIN_PASS   ?? 'Admin@123456!';
      return config;
    },
  },
});
