import axios from 'axios';
import { setAccessToken } from './apiClient';

// ─── Mock axios entirely ──────────────────────────────────────────────────────
// `create` and `default.create` reference the SAME mock function so both
// `import axios from 'axios'` (esModule interop) and direct `.create` usage
// resolve to the instance that apiClient.ts actually configures.
jest.mock('axios', () => {
  const mockAxiosInstance = {
    interceptors: {
      request: { use: jest.fn() },
      response: { use: jest.fn() },
    },
    post: jest.fn(),
    get: jest.fn(),
    defaults: { headers: { common: {} } },
  };
  const create = jest.fn(() => mockAxiosInstance);
  return { create, default: { create } };
});

// ─── Capture interceptor callbacks ONCE at module load ────────────────────────
// apiClient.ts registers the interceptors when it is imported (above). We grab
// the callbacks here, before any test runs, because jest.clearAllMocks() wipes
// mock.calls/results between tests.
const mockInstance = (axios.create as jest.Mock).mock.results[0].value as {
  interceptors: {
    request: { use: jest.Mock };
    response: { use: jest.Mock };
  };
};

const requestInterceptor = mockInstance.interceptors.request.use.mock.calls[0][0] as (
  config: { headers: Record<string, string> },
) => { headers: Record<string, string> };

const [responseSuccess] = mockInstance.interceptors.response.use.mock.calls[0] as [
  (response: unknown) => unknown,
  (error: unknown) => unknown,
];

// ─── Tests ────────────────────────────────────────────────────────────────────
describe('apiClient request interceptor', () => {
  afterEach(() => setAccessToken(null)); // isolate the module-level token

  it('should add an Authorization header when an access token is set', () => {
    setAccessToken('my-token');

    const config = { headers: {} as Record<string, string> };
    const result = requestInterceptor(config);

    expect(result.headers.Authorization ?? config.headers.Authorization).toBe('Bearer my-token');
  });

  it('should NOT add an Authorization header when the token is null', () => {
    setAccessToken(null);

    const config = { headers: {} as Record<string, string> };
    const result = requestInterceptor(config);

    expect(result.headers.Authorization).toBeUndefined();
  });

  it('should overwrite a previously set token', () => {
    setAccessToken('first-token');
    setAccessToken('second-token');

    const config = { headers: {} as Record<string, string> };
    const result = requestInterceptor(config);

    expect(result.headers.Authorization).toBe('Bearer second-token');
  });
});

describe('apiClient response interceptor (success path)', () => {
  it('should pass through successful responses unchanged', () => {
    const fakeResponse = { status: 200, data: { ok: true } };
    expect(responseSuccess(fakeResponse)).toBe(fakeResponse);
  });
});

describe('setAccessToken', () => {
  afterEach(() => setAccessToken(null));

  it('should be callable with a string token without throwing', () => {
    expect(() => setAccessToken('some-jwt')).not.toThrow();
  });

  it('should be callable with null without throwing', () => {
    expect(() => setAccessToken(null)).not.toThrow();
  });
});
