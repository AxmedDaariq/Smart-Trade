/**
 * Global k6 Thresholds Configuration
 * Import and spread these into any k6 options.thresholds object to
 * enforce consistent SLOs across all performance test suites.
 */

export const DEFAULT_THRESHOLDS = {
  // HTTP
  http_req_duration:   ['p(95)<800', 'p(99)<1500', 'max<5000'],
  http_req_failed:     ['rate<0.02'],
  http_req_connecting: ['p(95)<200'],
  http_req_tls_handshaking: ['p(95)<300'],

  // Custom error rate metric used in all scripts
  errors:              ['rate<0.02'],
};

/** Tighter thresholds for auth endpoints (higher scrutiny). */
export const AUTH_THRESHOLDS = {
  http_req_duration:  ['p(95)<500', 'p(99)<1000'],
  http_req_failed:    ['rate<0.01'],
  errors:             ['rate<0.01'],
};

/** Relaxed thresholds for report-generation or file-upload endpoints. */
export const BATCH_THRESHOLDS = {
  http_req_duration:  ['p(95)<5000', 'p(99)<10000'],
  http_req_failed:    ['rate<0.05'],
  errors:             ['rate<0.05'],
};
