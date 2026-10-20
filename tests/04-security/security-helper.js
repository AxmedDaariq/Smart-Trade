// ═══════════════════════════════════════════════════════════════════════
// Smart Trade Logistics — Security Test Helper & Utilities
// Provides:
// - Centralized endpoint resolution (Gateway, APIs, Frontend)
// - JWT Token crafts (valid, expired, forged signature, 'none' algorithm)
// - Safe HTTP request wrapper (never throws on 4xx/5xx for assert checks)
// - Fuzzing payloads for SQLi, NoSQLi, XSS, SSRF, and Header injections
// ═══════════════════════════════════════════════════════════════════════

require('dotenv').config();
const axios = require('axios');
const jwt = require('jsonwebtoken');

const CONFIG = {
  GATEWAY_URL:   process.env.API_GATEWAY   || process.env.GATEWAY_URL  || 'http://localhost:8080',
  IDENTITY_URL:  process.env.IDENTITY_API  || process.env.API_URL      || 'http://localhost:5000',
  API_URL:       process.env.API_URL       || 'http://localhost:5000',
  FRONTEND_URL:  process.env.FRONTEND_URL  || 'http://localhost:5173',
  TENANT_A:      process.env.TENANT_A      || 'ACME_LOGISTICS',
  TENANT_B:      process.env.TENANT_B      || 'HORN_CARRIERS',
  ADMIN_EMAIL:   process.env.ADMIN_EMAIL   || 'admin@acme.com',
  ADMIN_PASS:    process.env.ADMIN_PASS    || 'Admin@123456!',
  DRIVER_EMAIL:  process.env.DRIVER_EMAIL  || 'driver_test@smarttrade.com',
  DRIVER_PASS:   process.env.DRIVER_PASS   || 'DriverPass123!',
  JWT_SECRET:    process.env.JWT_SECRET    || 'SmartTradeSuperSecretEnterpriseKey2026!#',
};

/**
 * Execute HTTP probe safely without throwing on 4xx / 5xx error statuses.
 */
async function probe(method, url, options = {}) {
  try {
    const response = await axios({
      method,
      url,
      data: options.data || options.body,
      headers: options.headers || {},
      params: options.params || {},
      timeout: options.timeout || 8000,
      validateStatus: () => true, // Don't throw on 400, 401, 403, 404, 500
      maxRedirects: options.maxRedirects ?? 5,
    });
    return response;
  } catch (error) {
    if (error.response) {
      return error.response;
    }
    // Network level failure (e.g. ECONNREFUSED)
    return {
      status: 0,
      statusText: 'CONNECTION_ERROR',
      data: { error: error.message, code: error.code },
      headers: {},
      isNetworkError: true,
    };
  }
}

/**
 * Programmatic login helper to obtain authentic bearer token
 */
async function getAuthToken(email = CONFIG.ADMIN_EMAIL, password = CONFIG.ADMIN_PASS) {
  const res = await probe('POST', `${CONFIG.IDENTITY_URL}/api/identity/login`, {
    data: { email, password },
    headers: { 'Content-Type': 'application/json' },
  });

  if (res.status === 200 && res.data && res.data.token) {
    return res.data.token;
  }
  // Return fallback synthetic signed token if backend server is not running
  return jwt.sign(
    {
      sub: '00000000-0000-0000-0000-000000000001',
      email,
      role: email.includes('admin') ? 'Administrator' : 'Driver',
      tenant_code: CONFIG.TENANT_A,
    },
    CONFIG.JWT_SECRET,
    { expiresIn: '1h' }
  );
}

/**
 * Craft special security test JWT tokens
 */
const TokenCraft = {
  /** Token signed with wrong secret (Forged Signature) */
  forged(payload = { sub: 'admin-1', role: 'Administrator' }) {
    return jwt.sign(payload, 'WRONG_UNAUTHORIZED_SECRET_KEY_9999');
  },

  /** Token with 'none' algorithm (Algorithm Confusion Attack) */
  noneAlgorithm(payload = { sub: 'admin-1', role: 'Administrator' }) {
    const header = Buffer.from(JSON.stringify({ alg: 'none', typ: 'JWT' })).toString('base64url');
    const body   = Buffer.from(JSON.stringify(payload)).toString('base64url');
    return `${header}.${body}.`;
  },

  /** Expired token */
  expired(payload = { sub: 'admin-1', role: 'Administrator' }) {
    return jwt.sign(payload, CONFIG.JWT_SECRET, { expiresIn: '-10m' });
  },

  /** Token for specific tenant and role */
  custom(tenantCode, role, extra = {}) {
    return jwt.sign(
      {
        sub: `test-user-${Date.now()}`,
        tenant_code: tenantCode,
        role: role,
        ...extra,
      },
      CONFIG.JWT_SECRET,
      { expiresIn: '1h' }
    );
  },
};

/**
 * Common security fuzzing test payloads
 */
const SecurityPayloads = {
  SQL_INJECTION: [
    "' OR '1'='1",
    "'; DROP TABLE users; --",
    "' UNION SELECT NULL, email, password_hash FROM users --",
    "1' ORDER BY 1--+",
    "admin'--",
  ],
  NOSQL_INJECTION: [
    { "$gt": "" },
    { "$ne": null },
    { "$where": "sleep(5000)" },
    { "$regex": ".*" },
  ],
  XSS_VECTORS: [
    "<script>alert('XSS-TEST')</script>",
    "<img src=x onerror=alert('XSS-IMG')>",
    "javascript:alert(document.cookie)",
    "<svg/onload=alert('XSS-SVG')>",
  ],
  SSRF_TARGETS: [
    "http://127.0.0.1:8080/admin",
    "http://localhost:5432",
    "http://169.254.169.254/latest/meta-data/", // AWS/Cloud metadata
    "http://10.0.0.1:80",
    "http://192.168.1.1:80",
  ],
  PATH_TRAVERSAL: [
    "../../../../etc/passwd",
    "..\\..\\..\\..\\windows\\win.ini",
    "%2e%2e%2f%2e%2e%2fetc%2fpasswd",
  ],
};

module.exports = {
  CONFIG,
  probe,
  getAuthToken,
  TokenCraft,
  SecurityPayloads,
};
