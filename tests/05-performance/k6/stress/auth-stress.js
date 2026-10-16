/**
 * k6 Stress Test – Identity Auth API
 * Pushes load beyond normal capacity to find the breaking point.
 *
 * Run:
 *   k6 run tests/05-performance/k6/stress/auth-stress.js \
 *       -e API_URL=http://localhost:5000
 */
import http from 'k6/http';
import { check, sleep } from 'k6';
import { Rate } from 'k6/metrics';

const errorRate = new Rate('errors');

export const options = {
  stages: [
    { duration: '2m',  target: 100 },
    { duration: '5m',  target: 200 },
    { duration: '2m',  target: 300 },
    { duration: '5m',  target: 300 },
    { duration: '2m',  target: 0   },
  ],
  thresholds: {
    http_req_duration: ['p(95)<2000'],
    http_req_failed:   ['rate<0.10'],
    errors:            ['rate<0.10'],
  },
};

const BASE_URL = __ENV.API_URL ?? 'http://localhost:5000';

export default function () {
  const payload = JSON.stringify({
    tenantCode: 'ACME',
    email:      'admin@acme.com',
    password:   __ENV.K6_TEST_PASSWORD ?? 'USE_ENV_VAR_K6_TEST_PASSWORD',
  });

  const res = http.post(`${BASE_URL}/api/v1/identity/auth/login`, payload, {
    headers: { 'Content-Type': 'application/json' },
  });

  const ok = check(res, { 'not server error': (r) => r.status < 500 });
  errorRate.add(!ok);
  sleep(0.5);
}
