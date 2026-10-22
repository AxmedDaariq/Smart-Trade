import type { AuthSession } from './AuthContext';

// ─── RTL auto-cleanup opt-out (must run before @testing-library/react loads) ──
// Babel compiles this file to CommonJS, so `beforeEach(async () => { await
// import('@testing-library/react') })` loads RTL lazily *inside a hook*. RTL's
// module body registers its own `afterEach`/`beforeAll` hooks on load, and
// jest-circus rejects hooks declared while a test is running:
//   "Hooks cannot be defined inside tests. Hook of type "afterEach" is nested
//    within ...".
// Skipping RTL's auto-registration lets us re-import RTL from the fresh module
// registry (so React/react-dom resolve to one consistent copy after
// jest.resetModules()), and we call `rtl.cleanup()` manually in `afterEach`.
process.env.RTL_SKIP_AUTO_CLEANUP = 'true';

// ─── Mock external dependency ─────────────────────────────────────────────────
jest.mock('../../services/api/apiClient', () => ({ setAccessToken: jest.fn() }));

type Rtl = typeof import('@testing-library/react');
type AuthModule = typeof import('./AuthContext');

const validSession: AuthSession = {
  accessToken: 'tok_abc123',
  tenantCode: 'ACME',
  permissions: ['shipment.read', 'shipment.write'],
  roles: ['logistics-manager'],
  tenantType: 'logistics',
};

// AuthContext caches its session-restoration promise at module scope, so we reload
// it before every test. We also re-import @testing-library/react from the same
// fresh module registry so React/react-dom resolve to a single consistent copy.
let rtl: Rtl;
let AuthProvider: AuthModule['AuthProvider'];
let useAuth: AuthModule['useAuth'];
let setAccessTokenMock: jest.Mock;

beforeEach(async () => {
  jest.resetModules();
  rtl = await import('@testing-library/react');
  const auth = await import('./AuthContext');
  const api = await import('../../services/api/apiClient');
  AuthProvider = auth.AuthProvider;
  useAuth = auth.useAuth;
  setAccessTokenMock = api.setAccessToken as unknown as jest.Mock;
});

afterEach(() => {
  // Unmount rendered trees manually (RTL auto-cleanup is disabled above).
  rtl?.cleanup();
  jest.restoreAllMocks();
});

// Component that exposes context values via data-testid attributes.
// `logout()` uses try/finally without a catch, so a failing logout endpoint
// still clears the session but the error propagates to the caller. The test
// consumer must therefore handle the rejection instead of firing and
// forgetting it (which Jest reports as an unhandled rejection).
let lastLogoutError: unknown = null;

const makeConsumer = (hook: AuthModule['useAuth']) => () => {
  const { session, isInitializing, login, logout } = hook();
  return (
    <div>
      <span data-testid="initializing">{String(isInitializing)}</span>
      <span data-testid="session">{session ? session.tenantCode : 'null'}</span>
      <span data-testid="token">{session?.accessToken ?? 'none'}</span>
      <button onClick={() => login(validSession)}>Login</button>
      <button
        onClick={() => {
          lastLogoutError = null;
          // `AuthContextValue.logout` is declared `() => void`, but the
          // implementation is `async`, so it really returns a Promise that
          // rejects when the endpoint fails. Cast to reach the runtime shape.
          (logout as unknown as () => Promise<void>)().catch((error: unknown) => {
            lastLogoutError = error;
          });
        }}
      >
        Logout
      </button>
    </div>
  );
};

const renderProvider = () => {
  const Consumer = makeConsumer(useAuth);
  return rtl.render(
    <AuthProvider>
      <Consumer />
    </AuthProvider>,
  );
};

// ─── Test Suite ───────────────────────────────────────────────────────────────
describe('AuthContext', () => {
  // ── 1. Session restoration ──────────────────────────────────────────────────
  describe('session restoration on mount', () => {
    it('should show isInitializing=true then false after refresh resolves', async () => {
      global.fetch = jest.fn().mockResolvedValueOnce({
        ok: true,
        json: async () => validSession,
      } as Response);

      renderProvider();

      expect(rtl.screen.getByTestId('initializing').textContent).toBe('true');

      await rtl.waitFor(() =>
        expect(rtl.screen.getByTestId('initializing').textContent).toBe('false'),
      );
    });

    it('should restore session when refresh endpoint returns a valid payload', async () => {
      global.fetch = jest.fn().mockResolvedValueOnce({
        ok: true,
        json: async () => validSession,
      } as Response);

      renderProvider();

      await rtl.waitFor(() =>
        expect(rtl.screen.getByTestId('session').textContent).toBe('ACME'),
      );
      expect(setAccessTokenMock).toHaveBeenCalledWith('tok_abc123');
    });

    it('should leave session null when refresh endpoint returns 401', async () => {
      global.fetch = jest.fn().mockResolvedValueOnce({
        ok: false,
        json: async () => ({}),
      } as Response);

      renderProvider();

      await rtl.waitFor(() =>
        expect(rtl.screen.getByTestId('initializing').textContent).toBe('false'),
      );
      expect(rtl.screen.getByTestId('session').textContent).toBe('null');
      expect(setAccessTokenMock).not.toHaveBeenCalled();
    });

    it('should leave session null when refresh response is missing accessToken', async () => {
      global.fetch = jest.fn().mockResolvedValueOnce({
        ok: true,
        json: async () => ({ tenantCode: 'X', permissions: [] }),
      } as Response);

      renderProvider();

      await rtl.waitFor(() =>
        expect(rtl.screen.getByTestId('initializing').textContent).toBe('false'),
      );
      expect(rtl.screen.getByTestId('session').textContent).toBe('null');
    });

    it('should leave session null when fetch throws (network error)', async () => {
      global.fetch = jest.fn().mockRejectedValueOnce(new Error('Network error'));

      renderProvider();

      await rtl.waitFor(() =>
        expect(rtl.screen.getByTestId('initializing').textContent).toBe('false'),
      );
      expect(rtl.screen.getByTestId('session').textContent).toBe('null');
    });
  });

  // ── 2. login() ──────────────────────────────────────────────────────────────
  describe('login()', () => {
    it('should set session and call setAccessToken with the new token', async () => {
      global.fetch = jest.fn().mockResolvedValueOnce({
        ok: false,
        json: async () => ({}),
      } as Response);

      renderProvider();
      await rtl.waitFor(() =>
        expect(rtl.screen.getByTestId('initializing').textContent).toBe('false'),
      );

      rtl.act(() => rtl.screen.getByText('Login').click());

      expect(rtl.screen.getByTestId('session').textContent).toBe('ACME');
      expect(rtl.screen.getByTestId('token').textContent).toBe('tok_abc123');
      expect(setAccessTokenMock).toHaveBeenLastCalledWith('tok_abc123');
    });
  });

  // ── 3. logout() ─────────────────────────────────────────────────────────────
  describe('logout()', () => {
    it('should clear session and call setAccessToken(null)', async () => {
      global.fetch = jest
        .fn()
        .mockResolvedValueOnce({ ok: true, json: async () => validSession } as Response)
        .mockResolvedValueOnce({ ok: true, json: async () => ({}) } as Response);

      renderProvider();
      await rtl.waitFor(() =>
        expect(rtl.screen.getByTestId('session').textContent).toBe('ACME'),
      );

      await rtl.act(async () => {
        rtl.screen.getByText('Logout').click();
      });

      expect(rtl.screen.getByTestId('session').textContent).toBe('null');
      expect(setAccessTokenMock).toHaveBeenLastCalledWith(null);
      expect(lastLogoutError).toBeNull();
    });

    it('should clear session even when logout endpoint throws', async () => {
      global.fetch = jest
        .fn()
        .mockResolvedValueOnce({ ok: true, json: async () => validSession } as Response)
        .mockRejectedValueOnce(new Error('Network error'));

      renderProvider();
      await rtl.waitFor(() =>
        expect(rtl.screen.getByTestId('session').textContent).toBe('ACME'),
      );

      await rtl.act(async () => {
        rtl.screen.getByText('Logout').click();
      });

      // The `finally` block still clears the session...
      expect(rtl.screen.getByTestId('session').textContent).toBe('null');
      expect(setAccessTokenMock).toHaveBeenLastCalledWith(null);
      // ...but the endpoint failure is surfaced to the caller.
      expect((lastLogoutError as Error).message).toBe('Network error');
    });
  });

  // ── 4. useAuth guard ────────────────────────────────────────────────────────
  describe('useAuth() guard', () => {
    it('should throw when used outside AuthProvider', () => {
      const consoleError = jest.spyOn(console, 'error').mockImplementation(() => {});
      const BadComponent = () => {
        useAuth();
        return null;
      };
      expect(() => rtl.render(<BadComponent />)).toThrow(
        'useAuth must be used within AuthProvider',
      );
      consoleError.mockRestore();
    });
  });
});
