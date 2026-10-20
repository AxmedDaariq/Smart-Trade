// ═══════════════════════════════════════════════════════════════════════
// Smart Trade Logistics – Cypress Custom Commands
// Covers: Authentication, API, Database, Mobile, Utilities
// ═══════════════════════════════════════════════════════════════════════

// ────────────────────────────── AUTH ──────────────────────────────────

/**
 * cy.login() — Log in via the UI form.
 * Optionally pass email/password; defaults come from cypress.config.
 */
Cypress.Commands.add('login', (email, password) => {
  const userEmail = email || Cypress.env('ADMIN_EMAIL');
  const userPass  = password || Cypress.env('ADMIN_PASS');

  cy.visit('/login');
  cy.get('[data-cy=login-username]').clear().type(userEmail);
  cy.get('[data-cy=login-password]').clear().type(userPass);
  cy.get('[data-cy=login-submit]').click();

  // Wait for the dashboard to confirm a successful login
  cy.url().should('match', /\/(dashboard|logistics)/, { timeout: 15000 });
});

/**
 * cy.loginViaApi() — Fast programmatic login that bypasses the UI.
 * Sets the auth token directly so subsequent cy.visit() calls are
 * already authenticated. Use this when the login form itself is NOT
 * the subject of the test.
 */
Cypress.Commands.add('loginViaApi', (email, password) => {
  const userEmail = email || Cypress.env('ADMIN_EMAIL');
  const userPass  = password || Cypress.env('ADMIN_PASS');
  const apiUrl    = Cypress.env('API_URL');

  cy.request({
    method: 'POST',
    url: `${apiUrl}/api/identity/login`,
    body: { email: userEmail, password: userPass },
    failOnStatusCode: false,
  }).then((response) => {
    if (response.status === 200 && response.body.token) {
      // Store token in localStorage so the SPA picks it up
      window.localStorage.setItem('auth_token', response.body.token);
      if (response.body.refreshToken) {
        window.localStorage.setItem('refresh_token', response.body.refreshToken);
      }
    }
  });
});

/**
 * cy.logout() — Click the UI logout button or clear the session.
 */
Cypress.Commands.add('logout', () => {
  window.localStorage.removeItem('auth_token');
  window.localStorage.removeItem('refresh_token');
  cy.clearCookies();
  cy.visit('/login');
});


// ─────────────────────────── API HELPERS ──────────────────────────────

/**
 * cy.apiRequest(method, path, body) — Convenience wrapper for backend
 * API calls with the auth header automatically attached.
 *
 * @example cy.apiRequest('POST', '/api/shipping/orders', { ... })
 */
Cypress.Commands.add('apiRequest', (method, path, body = null) => {
  const apiUrl = Cypress.env('API_URL');
  const token  = window.localStorage.getItem('auth_token');

  return cy.request({
    method,
    url: `${apiUrl}${path}`,
    body,
    headers: {
      Authorization: token ? `Bearer ${token}` : undefined,
      'Content-Type': 'application/json',
      'X-Tenant-Code': Cypress.env('TENANT_CODE'),
    },
    failOnStatusCode: false,
  });
});

/**
 * cy.apiHealthCheck() — Verify the backend is alive.
 */
Cypress.Commands.add('apiHealthCheck', () => {
  const apiUrl = Cypress.env('API_URL');
  cy.request({
    method: 'GET',
    url: `${apiUrl}/health`,
    failOnStatusCode: false,
  }).its('status').should('eq', 200);
});


// ──────────────────────────── DATABASE ────────────────────────────────

/**
 * cy.dbQuery(sql, params) — Execute a raw SQL query against PostgreSQL.
 * Returns the resulting rows.
 *
 * @example cy.dbQuery('SELECT * FROM shipments WHERE id = $1', ['SH-001'])
 */
Cypress.Commands.add('dbQuery', (query, params = []) => {
  return cy.task('queryDatabase', { query, params });
});

/**
 * cy.dbSeed(table, data) — Insert a row and return the full record.
 *
 * @example cy.dbSeed('users', { email: 'test@test.com', role: 'admin' })
 */
Cypress.Commands.add('dbSeed', (table, data) => {
  return cy.task('seedRecord', { table, data });
});

/**
 * cy.dbCleanup(table, where) — Delete test data.
 *
 * @example cy.dbCleanup('users', "email LIKE '%@test.com'")
 */
Cypress.Commands.add('dbCleanup', (table, where) => {
  return cy.task('cleanupDatabase', { table, where });
});

/**
 * cy.dbVerifyRecord(table, where, expectedFields) — Assert that a row
 * exists in the database and matches the expected field values.
 */
Cypress.Commands.add('dbVerifyRecord', (table, where, expectedFields) => {
  cy.dbQuery(`SELECT * FROM ${table} WHERE ${where}`).then((rows) => {
    expect(rows.length).to.be.greaterThan(0, `Expected record in ${table} WHERE ${where}`);
    const row = rows[0];
    Object.entries(expectedFields).forEach(([key, value]) => {
      expect(row[key]).to.equal(value, `Field ${key} in ${table}`);
    });
  });
});

// ──────────────────────── MONGODB (TELEMATICS) ────────────────────────

/**
 * cy.mongoPing() — Check MongoDB connectivity.
 */
Cypress.Commands.add('mongoPing', () => {
  return cy.task('mongoPing');
});

/**
 * cy.mongoInsert(collection, docOrDocs) — Insert unstructured IoT / telematics documents.
 * @example cy.mongoInsert('telematics_gps', { truck_id: 'TRK-001', coords: [45.0, 9.5], speed_kmh: 72 })
 */
Cypress.Commands.add('mongoInsert', (collection, docOrDocs) => {
  if (Array.isArray(docOrDocs)) {
    return cy.task('mongoInsert', { collection, docs: docOrDocs });
  }
  return cy.task('mongoInsert', { collection, doc: docOrDocs });
});

/**
 * cy.mongoQuery(collection, filter, options) — Query unstructured telemetry documents.
 * @example cy.mongoQuery('telematics_gps', { truck_id: 'TRK-001' })
 */
Cypress.Commands.add('mongoQuery', (collection, filter = {}, options = {}) => {
  return cy.task('mongoQuery', {
    collection,
    filter,
    limit: options.limit ?? 50,
    sort: options.sort ?? {},
  });
});

/**
 * cy.mongoDeleteMany(collection, filter) — Clean up test telematics documents.
 */
Cypress.Commands.add('mongoDeleteMany', (collection, filter = {}) => {
  return cy.task('mongoDeleteMany', { collection, filter });
});

/**
 * cy.mongoCount(collection, filter) — Count documents matching a query.
 */
Cypress.Commands.add('mongoCount', (collection, filter = {}) => {
  return cy.task('mongoCount', { collection, filter });
});

// ──────────────────────────── REDIS CACHE ─────────────────────────────

/**
 * cy.redisPing() — Verify Redis connection status.
 */
Cypress.Commands.add('redisPing', () => {
  return cy.task('redisPing');
});

/**
 * cy.redisSet(key, value, ttlSeconds) — Set a cached key with optional TTL.
 */
Cypress.Commands.add('redisSet', (key, value, ttlSeconds = null) => {
  return cy.task('redisSet', { key, value, ttlSeconds });
});

/**
 * cy.redisGet(key) — Get a cached value.
 */
Cypress.Commands.add('redisGet', (key) => {
  return cy.task('redisGet', { key });
});

/**
 * cy.redisDel(key) — Delete a key from Redis.
 */
Cypress.Commands.add('redisDel', (key) => {
  return cy.task('redisDel', { key });
});

/**
 * cy.redisExists(key) — Check if key exists in Redis cache.
 */
Cypress.Commands.add('redisExists', (key) => {
  return cy.task('redisExists', { key });
});

/**
 * cy.redisFlushPattern(pattern) — Clear cache keys matching pattern.
 */
Cypress.Commands.add('redisFlushPattern', (pattern) => {
  return cy.task('redisFlushPattern', { pattern });
});

// ─────────────────────── RABBITMQ MESSAGING ───────────────────────────

/**
 * cy.rabbitCheckHealth() — Verify RabbitMQ broker health and metrics.
 */
Cypress.Commands.add('rabbitCheckHealth', () => {
  return cy.task('rabbitCheckHealth');
});

/**
 * cy.rabbitPublish(exchange, routingKey, message) — Publish message to RabbitMQ exchange.
 */
Cypress.Commands.add('rabbitPublish', (exchange, routingKey, message) => {
  return cy.task('rabbitPublish', { exchange, routingKey, message });
});

/**
 * cy.rabbitCheckQueue(queue) — Inspect queue message count and consumer status.
 */
Cypress.Commands.add('rabbitCheckQueue', (queue) => {
  return cy.task('rabbitCheckQueue', { queue });
});

/**
 * cy.rabbitPurgeQueue(queue) — Purge queue contents.
 */
Cypress.Commands.add('rabbitPurgeQueue', (queue) => {
  return cy.task('rabbitPurgeQueue', { queue });
});


// ───────────────────────── GATEWAY / MOBILE ──────────────────────────

/**
 * cy.mobileApiRequest(method, path, body) — Hit the backend from the
 * "mobile" perspective (different base URL / tenant header).
 */
Cypress.Commands.add('mobileApiRequest', (method, path, body = null) => {
  const mobileApi = Cypress.env('MOBILE_API');
  const token     = Cypress.env('_mobileToken'); // set during mobile login

  return cy.request({
    method,
    url: `${mobileApi}${path}`,
    body,
    headers: {
      Authorization: token ? `Bearer ${token}` : undefined,
      'Content-Type': 'application/json',
      'X-Client-Type': 'mobile-driver',
      'X-Tenant-Code': Cypress.env('TENANT_CODE'),
    },
    failOnStatusCode: false,
  });
});

/**
 * cy.mobileLogin(email, pass) — Programmatic login as the mobile driver.
 * Stores the token in Cypress.env so subsequent mobileApiRequest calls
 * pick it up.
 */
Cypress.Commands.add('mobileLogin', (email, password) => {
  const driverEmail = email || Cypress.env('DRIVER_EMAIL');
  const driverPass  = password || Cypress.env('DRIVER_PASS');
  const mobileApi   = Cypress.env('MOBILE_API');

  cy.request({
    method: 'POST',
    url: `${mobileApi}/api/identity/login`,
    body: { email: driverEmail, password: driverPass },
    headers: { 'X-Client-Type': 'mobile-driver' },
  }).then((response) => {
    expect(response.status).to.eq(200);
    Cypress.env('_mobileToken', response.body.token);
  });
});


// ───────────────────────── UI UTILITIES ──────────────────────────────

/**
 * cy.dataCy(name) — Shortcut for selecting [data-cy=...] elements.
 */
Cypress.Commands.add('dataCy', (value) => {
  return cy.get(`[data-cy=${value}]`);
});

/**
 * cy.assertToast(text) — Assert that a toast notification appears
 * with the given text.
 */
Cypress.Commands.add('assertToast', (text) => {
  cy.contains(text, { timeout: 10000 }).should('be.visible');
});

/**
 * cy.waitForApi(method, pathPattern) — Intercept + wait for a
 * specific API call to resolve.
 *
 * @example cy.waitForApi('POST', '/api/shipping/orders')
 */
Cypress.Commands.add('waitForApi', (method, pathPattern) => {
  const alias = `api_${method}_${pathPattern.replace(/\//g, '_')}`;
  cy.intercept(method, `**${pathPattern}**`).as(alias);
  return cy.wait(`@${alias}`, { timeout: 15000 });
});

/**
 * cy.interceptApiResponse(method, pathPattern, statusCode, fixture) —
 * Stub an API response with a fixture file.
 */
Cypress.Commands.add('interceptApiResponse', (method, pathPattern, statusCode, fixture) => {
  cy.intercept(method, `**${pathPattern}**`, {
    statusCode,
    fixture,
  });
});

/**
 * cy.setMobileViewport() — Set viewport to standard mobile device dimensions (iPhone/Android handheld)
 */
Cypress.Commands.add('setMobileViewport', () => {
  cy.viewport(390, 844);
});