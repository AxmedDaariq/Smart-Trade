/**
 * k6 Soak Test – Identity Auth API
 * Runs at moderate load for an extended period to surface memory leaks
 * and connection pool exhaustion.
 *
 * Run:
 *   k6 run tests/05-performance/k6/soak/auth-soak.js \
 *       -e API_URL=http://localhost:5000
 */
import http from 'k6/http';
import { check, sleep } from 'k6';
import { Rate, Trend } from 'k6/metrics';

const errorRate     = new Rate('errors');
const loginDuration = new Trend('login_duration', true);

export const options = {
  stages: [
    { duration: '5m',  target: 50 },   // ramp up
    { duration: '4h',  target: 50 },   // soak
    { duration: '5m',  target: 0  },   // ramp down
  ],
  thresholds: {
    http_req_duration: ['p(95)<1000'],
    http_req_failed:   ['rate<0.01'],
    errors:            ['rate<0.01'],
    login_duration:    ['p(95)<1000'],
  },
};

const BASE_URL = __ENV.API_URL ?? 'http://localhost:5000';

export default function () {
  const res = http.post(`${BASE_URL}/api/v1/identity/auth/login`,
    JSON.stringify({ tenantCode: 'ACME', email: 'admin@acme.com', password: __ENV.K6_TEST_PASSWORD ?? 'USE_ENV_VAR_K6_TEST_PASSWORD' }),
    { headers: { 'Content-Type': 'application/json' } });

  loginDuration.add(res.timings.duration);
  errorRate.add(!check(res, { 'status 200': (r) => r.status === 200 }));
  sleep(2);
}
