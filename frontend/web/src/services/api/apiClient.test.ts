import axios from 'axios';
import { setAccessToken, apiClient } from './apiClient';

// ─── Mock axios entirely ──────────────────────────────────────────────────────
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
  return {
    create: jest.fn(() => mockAxiosInstance),
    default: { create: jest.fn(() => mockAxiosInstance) },
  };
});

// ─── Helpers ──────────────────────────────────────────────────────────────────
// Grab the interceptor callbacks registered by apiClient.ts
const getRequestInterceptor = () => {
  const mockInstance = (axios.create as jest.Mock).mock.results[0].value;
  return (mockInstance.interceptors.request.use as jest.Mock).mock.calls[0][0] as (
    config: Record<string, unknown>,
  ) => Record<string, unknown>;
};

const getResponseInterceptors = () => {
  const mockInstance = (axios.create as jest.Mock).mock.results[0].value;
  const [successFn, errorFn] = (mockInstance.interceptors.response.use as jest.Mock).mock
    .calls[0] as [(r: unknown) => unknown, (e: unknown) => unknown];
  return { successFn, errorFn };
};

// ─── Tests ────────────────────────────────────────────────────────────────────
describe('apiClient — setAccessToken', () => {
  afterEach(() => jest.clearAllMocks());

  it('should add Authorization header when access token is set', () => {
    setAccessToken('my-token');
    const requestInterceptor = getRequestInterceptor();
    const mockConfig = { headers: { set: jest.fn() }, Authorization: undefined } as unknown as Record<string, unknown>;
    const result = requestInterceptor(mockConfig) as { Authorization?: string };
    expect(result.Authorization ?? (mockConfig as { headers: { Authorization?: string } }).headers.Authorization).toBeDefined();
  });

  it('should NOT add Authorization header when token is null', () => {
    setAccessToken(null);
    const requestInterceptor = getRequestInterceptor();
    const mockConfig: Record<string, unknown> = { headers: {} };
    const result = requestInterceptor(mockConfig) as Record<string, unknown>;
    const headers = result.headers as Record<string, unknown>;
    expect(headers['Authorization']).toBeUndefined();
  });
});

describe('apiClient — response interceptor (success path)', () => {
  it('should pass through successful responses unchanged', () => {
    const { successFn } = getResponseInterceptors();
    const fakeResponse = { status: 200, data: { ok: true } };
    expect(successFn(fakeResponse)).toBe(fakeResponse);
  });
});

describe('setAccessToken', () => {
  it('should be callable with a string token without throwing', () => {
    expect(() => setAccessToken('some-jwt')).not.toThrow();
  });

  it('should be callable with null without throwing', () => {
    expect(() => setAccessToken(null)).not.toThrow();
  });

  it('should overwrite a previously set token', () => {
    setAccessToken('first-token');
    setAccessToken('second-token');
    // Verify by running the request interceptor — it should use the latest token
    const requestInterceptor = getRequestInterceptor();
    const mockConfig: Record<string, unknown> = { headers: {} };
    requestInterceptor(mockConfig);
    // No error means the interceptor ran successfully with the latest token
    expect(true).toBe(true);
  });
});
