/**
 * k6 Load Test – Identity Auth API
 * Simulates realistic concurrent load over a ramp-up/steady/ramp-down cycle.
 *
 * Run:
 *   k6 run tests/05-performance/k6/load/auth-load.js \
 *       -e API_URL=http://localhost:5000
 */
import http from 'k6/http';
import { check, sleep } from 'k6';
import { Rate, Trend } from 'k6/metrics';

const errorRate      = new Rate('errors');
const loginDuration  = new Trend('login_duration', true);

export const options = {
  stages: [
    { duration: '2m',  target: 50  },   // ramp up  → 50 VUs
    { duration: '5m',  target: 50  },   // hold      @ 50 VUs
    { duration: '2m',  target: 100 },   // ramp up  → 100 VUs
    { duration: '5m',  target: 100 },   // hold      @ 100 VUs
    { duration: '2m',  target: 0   },   // ramp down → 0 VUs
  ],
  thresholds: {
    http_req_duration: ['p(95)<800', 'p(99)<1500'],
    http_req_failed:   ['rate<0.02'],
    errors:            ['rate<0.02'],
    login_duration:    ['p(95)<800'],
  },
};

const BASE_URL = __ENV.API_URL ?? 'http://localhost:5000';

export default function () {
  const payload = JSON.stringify({
    tenantCode: 'ACME',
    email:      `user${Math.floor(Math.random() * 100)}@acme.com`,
    password:   'Admin@123456!',
  });

  const params = { headers: { 'Content-Type': 'application/json' } };

  const res = http.post(`${BASE_URL}/api/v1/identity/auth/login`, payload, params);
  loginDuration.add(res.timings.duration);

  const ok = check(res, {
    'status 200 or 401': (r) => r.status === 200 || r.status === 401,
    'response time < 1s': (r) => r.timings.duration < 1000,
  });

  errorRate.add(!ok);
  sleep(Math.random() * 2 + 1); // think time 1–3 s
}
