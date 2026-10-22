/**
 * k6 Spike Test – Identity Auth API
 * Simulates a sudden burst of traffic (e.g. a viral event or DDoS probe).
 */
import http from 'k6/http';
import { check, sleep } from 'k6';
import { Rate } from 'k6/metrics';

const errorRate = new Rate('errors');

export const options = {
  stages: [
    { duration: '10s', target: 10  },   // warm-up
    { duration: '30s', target: 500 },   // spike!
    { duration: '1m',  target: 500 },   // sustain spike
    { duration: '30s', target: 10  },   // recover
    { duration: '30s', target: 0   },   // ramp down
  ],
  thresholds: {
    http_req_failed: ['rate<0.15'],
    errors:          ['rate<0.15'],
  },
};

const BASE_URL = __ENV.API_URL ?? 'http://localhost:5000';

export default function () {
  const res = http.post(`${BASE_URL}/api/v1/identity/auth/login`,
    JSON.stringify({ tenantCode: 'ACME', email: 'admin@acme.com', password: __ENV.K6_TEST_PASSWORD ?? 'USE_ENV_VAR_K6_TEST_PASSWORD' }),
    { headers: { 'Content-Type': 'application/json' } });

  errorRate.add(!check(res, { 'not 5xx': (r) => r.status < 500 }));
  sleep(0.1);
}
