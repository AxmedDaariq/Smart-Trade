import { render, screen, waitFor, act } from '@testing-library/react';
import { AuthProvider, useAuth, AuthSession } from './AuthContext';
import { setAccessToken } from '../../services/api/apiClient';

// ─── Mock external dependencies ──────────────────────────────────────────────
jest.mock('../../services/api/apiClient', () => ({
  setAccessToken: jest.fn(),
}));

const mockSetAccessToken = setAccessToken as jest.Mock;

// Shared valid session fixture
const validSession: AuthSession = {
  accessToken: 'tok_abc123',
  tenantCode: 'ACME',
  permissions: ['shipment.read', 'shipment.write'],
  roles: ['logistics-manager'],
  tenantType: 'logistics',
};

// Helper: component that exposes context values via data-testid attributes
const TestConsumer = () => {
  const { session, isInitializing, login, logout } = useAuth();
  return (
    <div>
      <span data-testid="initializing">{String(isInitializing)}</span>
      <span data-testid="session">{session ? session.tenantCode : 'null'}</span>
      <span data-testid="token">{session?.accessToken ?? 'none'}</span>
      <button onClick={() => login(validSession)}>Login</button>
      <button onClick={() => void logout()}>Logout</button>
    </div>
  );
};

// Helper: wrap component in AuthProvider with pre-configured fetch mock
const renderWithProvider = () =>
  render(
    <AuthProvider>
      <TestConsumer />
    </AuthProvider>,
  );

// ─── Test Suite ───────────────────────────────────────────────────────────────
describe('AuthContext', () => {
  beforeEach(() => {
    jest.clearAllMocks();
    jest.resetModules();
  });

  afterEach(() => {
    jest.restoreAllMocks();
  });

  // ── 1. Session restoration ──────────────────────────────────────────────────
  describe('session restoration on mount', () => {
    it('should show isInitializing=true then false after refresh resolves', async () => {
      global.fetch = jest.fn().mockResolvedValueOnce({
        ok: true,
        json: async () => validSession,
      } as Response);

      renderWithProvider();

      expect(screen.getByTestId('initializing').textContent).toBe('true');

      await waitFor(() =>
        expect(screen.getByTestId('initializing').textContent).toBe('false'),
      );
    });

    it('should restore session when refresh endpoint returns valid payload', async () => {
      global.fetch = jest.fn().mockResolvedValueOnce({
        ok: true,
        json: async () => validSession,
      } as Response);

      renderWithProvider();

      await waitFor(() =>
        expect(screen.getByTestId('session').textContent).toBe('ACME'),
      );
      expect(mockSetAccessToken).toHaveBeenCalledWith('tok_abc123');
    });

    it('should leave session null when refresh endpoint returns 401', async () => {
      global.fetch = jest.fn().mockResolvedValueOnce({
        ok: false,
        json: async () => ({}),
      } as Response);

      renderWithProvider();

      await waitFor(() =>
        expect(screen.getByTestId('initializing').textContent).toBe('false'),
      );
      expect(screen.getByTestId('session').textContent).toBe('null');
      expect(mockSetAccessToken).not.toHaveBeenCalled();
    });

    it('should leave session null when refresh response is missing accessToken', async () => {
      global.fetch = jest.fn().mockResolvedValueOnce({
        ok: true,
        json: async () => ({ tenantCode: 'X', permissions: [] }),
      } as Response);

      renderWithProvider();

      await waitFor(() =>
        expect(screen.getByTestId('initializing').textContent).toBe('false'),
      );
      expect(screen.getByTestId('session').textContent).toBe('null');
    });

    it('should leave session null when fetch throws (network error)', async () => {
      global.fetch = jest.fn().mockRejectedValueOnce(new Error('Network error'));

      renderWithProvider();

      await waitFor(() =>
        expect(screen.getByTestId('initializing').textContent).toBe('false'),
      );
      expect(screen.getByTestId('session').textContent).toBe('null');
    });
  });

  // ── 2. login() ──────────────────────────────────────────────────────────────
  describe('login()', () => {
    it('should set session and call setAccessToken with the new token', async () => {
      global.fetch = jest.fn().mockResolvedValueOnce({
        ok: false,
        json: async () => ({}),
      } as Response);

      renderWithProvider();
      await waitFor(() =>
        expect(screen.getByTestId('initializing').textContent).toBe('false'),
      );

      act(() => screen.getByText('Login').click());

      expect(screen.getByTestId('session').textContent).toBe('ACME');
      expect(screen.getByTestId('token').textContent).toBe('tok_abc123');
      expect(mockSetAccessToken).toHaveBeenLastCalledWith('tok_abc123');
    });
  });

  // ── 3. logout() ─────────────────────────────────────────────────────────────
  describe('logout()', () => {
    it('should clear session and call setAccessToken(null)', async () => {
      global.fetch = jest
        .fn()
        .mockResolvedValueOnce({ ok: true, json: async () => validSession } as Response)
        .mockResolvedValueOnce({ ok: true, json: async () => ({}) } as Response);

      renderWithProvider();
      await waitFor(() =>
        expect(screen.getByTestId('session').textContent).toBe('ACME'),
      );

      await act(async () => {
        screen.getByText('Logout').click();
      });

      expect(screen.getByTestId('session').textContent).toBe('null');
      expect(mockSetAccessToken).toHaveBeenLastCalledWith(null);
    });

    it('should clear session even when logout endpoint throws', async () => {
      global.fetch = jest
        .fn()
        .mockResolvedValueOnce({ ok: true, json: async () => validSession } as Response)
        .mockRejectedValueOnce(new Error('Network error'));

      renderWithProvider();
      await waitFor(() =>
        expect(screen.getByTestId('session').textContent).toBe('ACME'),
      );

      await act(async () => {
        screen.getByText('Logout').click();
      });

      expect(screen.getByTestId('session').textContent).toBe('null');
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
      expect(() => render(<BadComponent />)).toThrow(
        'useAuth must be used within AuthProvider',
      );
      consoleError.mockRestore();
    });
  });
});
