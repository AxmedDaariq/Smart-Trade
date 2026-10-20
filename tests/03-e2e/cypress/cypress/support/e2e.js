// ═══════════════════════════════════════════════════════════════════════
// Smart Trade Logistics Platform — Cypress E2E Support File
// Loads all custom commands + global hooks
// ═══════════════════════════════════════════════════════════════════════

import './commands';

// ─── Global hooks ─────────────────────────────────────────────────────

// Suppress uncaught exceptions from the application itself (prevents
// false negatives when the SPA throws an unrelated runtime error).
Cypress.on('uncaught:exception', (err) => {
  // Returning false prevents Cypress from failing the test
  if (err.message.includes('ResizeObserver loop')) return false;
  if (err.message.includes('Navigation cancelled')) return false;
  // Let all other errors fail the test
  return true;
});

// Log the spec file name at the start of every run
before(() => {
  cy.task('log', `▶ Running spec: ${Cypress.spec.name}`);
});
