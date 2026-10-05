import '@testing-library/jest-dom';

// jsdom does not implement `fetch`. Components (e.g. AuthProvider) call it on
// mount, so provide a safe default that individual tests can override with
// their own `global.fetch = jest.fn()`.
if (typeof globalThis.fetch !== 'function') {
  globalThis.fetch = jest.fn(() =>
    Promise.resolve({ ok: false, json: async () => ({}) } as Response),
  ) as unknown as typeof fetch;
}
