import { render, screen } from '@testing-library/react';
import { MemoryRouter, Route, Routes } from 'react-router-dom';
import { RequirePermission } from './RequirePermission';
import * as AuthContextModule from './AuthContext';
import { AuthSession } from './AuthContext';

const baseSession: AuthSession = {
  accessToken: 'tok_test',
  tenantCode: 'ACME',
  permissions: ['customs.configuration.read'],
  roles: ['CUSTOMS_OFFICER'],
  tenantType: 'LOGISTICS_COMPANY',
};

const mockUseAuth = (override: Partial<ReturnType<typeof AuthContextModule.useAuth>>) =>
  jest.spyOn(AuthContextModule, 'useAuth').mockReturnValue({
    session: null,
    isInitializing: false,
    login: jest.fn(),
    logout: jest.fn(),
    ...override,
  });

interface GuardProps {
  requiredPermission?: string;
  requiredRole?: string;
  requiredTenantAccess?: string;
}

const renderGuard = (props: GuardProps) =>
  render(
    <MemoryRouter initialEntries={['/secured']}>
      <Routes>
        <Route
          path="/secured"
          element={
            <RequirePermission {...props}>
              <div>Guarded Content</div>
            </RequirePermission>
          }
        />
        <Route path="/login" element={<div>Login Page</div>} />
        <Route path="/unauthorized" element={<div>Unauthorized Page</div>} />
      </Routes>
    </MemoryRouter>,
  );

describe('RequirePermission', () => {
  afterEach(() => jest.restoreAllMocks());

  it('shows a loading indicator while the session is initializing', () => {
    mockUseAuth({ isInitializing: true });
    renderGuard({});
    expect(screen.getByRole('status')).toHaveTextContent('Restoring session…');
  });

  it('redirects to /login when there is no session', () => {
    mockUseAuth({ session: null });
    renderGuard({});
    expect(screen.getByText('Login Page')).toBeInTheDocument();
  });

  it('renders children when no requirement is set and a session exists', () => {
    mockUseAuth({ session: baseSession });
    renderGuard({});
    expect(screen.getByText('Guarded Content')).toBeInTheDocument();
  });

  it('renders children when the required permission is present', () => {
    mockUseAuth({ session: baseSession });
    renderGuard({ requiredPermission: 'customs.configuration.read' });
    expect(screen.getByText('Guarded Content')).toBeInTheDocument();
  });

  it('redirects to /unauthorized when the required permission is missing', () => {
    mockUseAuth({ session: baseSession });
    renderGuard({ requiredPermission: 'controltower.read' });
    expect(screen.getByText('Unauthorized Page')).toBeInTheDocument();
  });

  it('renders children when the required role is present', () => {
    mockUseAuth({ session: baseSession });
    renderGuard({ requiredRole: 'CUSTOMS_OFFICER' });
    expect(screen.getByText('Guarded Content')).toBeInTheDocument();
  });

  it('redirects to /unauthorized when the required role is missing', () => {
    mockUseAuth({ session: baseSession });
    renderGuard({ requiredRole: 'ADMIN' });
    expect(screen.getByText('Unauthorized Page')).toBeInTheDocument();
  });

  it('renders children when the tenant access matches', () => {
    mockUseAuth({ session: baseSession });
    renderGuard({ requiredTenantAccess: 'LOGISTICS_COMPANY' });
    expect(screen.getByText('Guarded Content')).toBeInTheDocument();
  });

  it('redirects to /unauthorized when the tenant access does not match', () => {
    mockUseAuth({ session: baseSession });
    renderGuard({ requiredTenantAccess: 'TRADER' });
    expect(screen.getByText('Unauthorized Page')).toBeInTheDocument();
  });
});
