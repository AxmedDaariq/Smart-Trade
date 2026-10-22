/**
 * k6 Smoke Test – Identity Auth API
 * Validates the /login endpoint returns 200 with fewer than 5 VUs.
 * Purpose: quick sanity check that the service is alive before heavier runs.
 *
 * Run:
 *   k6 run tests/05-performance/k6/smoke/auth-smoke.js \
 *       -e API_URL=http://localhost:5000
 */
import http from 'k6/http';
import { check, sleep } from 'k6';
import { Rate } from 'k6/metrics';

const errorRate = new Rate('errors');

export const options = {
  vus: 3,
  duration: '30s',
  thresholds: {
    http_req_duration: ['p(95)<500'],   // 95th percentile < 500 ms
    http_req_failed:   ['rate<0.01'],   // error rate < 1 %
    errors:            ['rate<0.01'],
  },
};

const BASE_URL = __ENV.API_URL ?? 'http://localhost:5000';

export default function () {
  const payload = JSON.stringify({
    tenantCode: 'ACME',
    email:      'admin@acme.com',
    password:   __ENV.K6_TEST_PASSWORD ?? 'USE_ENV_VAR_K6_TEST_PASSWORD',
  });

  const params = {
    headers: { 'Content-Type': 'application/json' },
  };

  const res = http.post(`${BASE_URL}/api/v1/identity/auth/login`, payload, params);

  const ok = check(res, {
    'status is 200':           (r) => r.status === 200,
    'has accessToken':         (r) => JSON.parse(r.body)?.accessToken !== undefined,
    'response time < 500ms':   (r) => r.timings.duration < 500,
  });

  errorRate.add(!ok);
  sleep(1);
}
