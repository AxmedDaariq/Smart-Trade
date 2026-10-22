import { render, screen } from '@testing-library/react';
import { MemoryRouter, Route, Routes } from 'react-router-dom';
import { RequireAuth, RequirePermission } from './RequireAuth';
import * as AuthContextModule from './auth/AuthContext';
import { AuthSession } from './auth/AuthContext';

// ─── Shared fixtures ──────────────────────────────────────────────────────────
const baseSession: AuthSession = {
  accessToken: 'tok_xyz',
  tenantCode: 'ACME',
  permissions: ['control-tower.read', 'shipment.read'],
  roles: ['admin'],
  tenantType: 'logistics',
};

// ─── Helper: mock useAuth with a given return value ───────────────────────────
const mockUseAuth = (override: Partial<ReturnType<typeof AuthContextModule.useAuth>>) => {
  jest.spyOn(AuthContextModule, 'useAuth').mockReturnValue({
    session: null,
    isInitializing: false,
    login: jest.fn(),
    logout: jest.fn(),
    ...override,
  });
};

// ─── Helper: render inside MemoryRouter ──────────────────────────────────────
const renderRequireAuth = (initialPath = '/dashboard') =>
  render(
    <MemoryRouter initialEntries={[initialPath]}>
      <Routes>
        <Route element={<RequireAuth />}>
          <Route path="/dashboard" element={<div>Protected Content</div>} />
        </Route>
        <Route path="/login" element={<div>Login Page</div>} />
      </Routes>
    </MemoryRouter>,
  );

const renderRequirePermission = (permission: string, initialPath = '/control-tower') =>
  render(
    <MemoryRouter initialEntries={[initialPath]}>
      <Routes>
        <Route element={<RequirePermission permission={permission} />}>
          <Route path="/control-tower" element={<div>Control Tower</div>} />
        </Route>
        <Route path="/403" element={<div>403 Forbidden</div>} />
      </Routes>
    </MemoryRouter>,
  );

// ─── Tests ───────────────────────────────────────────────────────────────────
describe('RequireAuth', () => {
  afterEach(() => jest.restoreAllMocks());

  it('should render loading indicator while initializing', () => {
    mockUseAuth({ isInitializing: true, session: null });
    renderRequireAuth();
    expect(screen.getByRole('status')).toHaveTextContent('Restoring session…');
  });

  it('should redirect to /login when session is null and not initializing', () => {
    mockUseAuth({ isInitializing: false, session: null });
    renderRequireAuth();
    expect(screen.getByText('Login Page')).toBeInTheDocument();
  });

  it('should render protected content when session exists', () => {
    mockUseAuth({ isInitializing: false, session: baseSession });
    renderRequireAuth();
    expect(screen.getByText('Protected Content')).toBeInTheDocument();
  });
});

describe('RequirePermission', () => {
  afterEach(() => jest.restoreAllMocks());

  it('should render protected route when user has the required permission', () => {
    mockUseAuth({ session: baseSession });
    renderRequirePermission('control-tower.read');
    expect(screen.getByText('Control Tower')).toBeInTheDocument();
  });

  it('should redirect to /403 when user lacks the required permission', () => {
    mockUseAuth({ session: { ...baseSession, permissions: ['shipment.read'] } });
    renderRequirePermission('control-tower.read');
    expect(screen.getByText('403 Forbidden')).toBeInTheDocument();
  });

  it('should redirect to /403 when session is null (unauthenticated)', () => {
    mockUseAuth({ session: null });
    renderRequirePermission('control-tower.read');
    expect(screen.getByText('403 Forbidden')).toBeInTheDocument();
  });
});
