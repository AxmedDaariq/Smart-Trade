import { render, screen } from '@testing-library/react';
import { MemoryRouter } from 'react-router-dom';
import * as AuthContextModule from './app/auth/AuthContext';
import { AuthSession } from './app/auth/AuthContext';

// Mock the services/api/apiClient so import.meta.env doesn't blow up in Jest
jest.mock('./services/api/apiClient', () => ({
  apiClient: {
    interceptors: { request: { use: jest.fn() }, response: { use: jest.fn() } },
  },
  setAccessToken: jest.fn(),
}));

const baseSession: AuthSession = {
  accessToken: 'tok_test',
  tenantCode: 'TEST',
  permissions: ['control-tower.read'],
  roles: ['admin'],
  tenantType: 'logistics',
};

const mockUseAuth = (override: Partial<ReturnType<typeof AuthContextModule.useAuth>>) => {
  jest.spyOn(AuthContextModule, 'useAuth').mockReturnValue({
    session: null,
    isInitializing: false,
    login: jest.fn(),
    logout: jest.fn(),
    ...override,
  });
};

// Lazy import App so mocks are registered first
let App: typeof import('./App').default;
beforeAll(async () => {
  App = (await import('./App')).default;
});

describe('App routing', () => {
  afterEach(() => jest.restoreAllMocks());

  it('should render Sign in page at /login', async () => {
    mockUseAuth({ isInitializing: false, session: null });
    render(<App />);
    expect(await screen.findByText('Sign in')).toBeInTheDocument();
  });

  it('should render 403 Forbidden page at /403', async () => {
    mockUseAuth({ isInitializing: false, session: baseSession });
    render(
      <MemoryRouter initialEntries={['/403']}>
        <div>403 Forbidden</div>
      </MemoryRouter>,
    );
    expect(screen.getByText('403 Forbidden')).toBeInTheDocument();
  });

  it('should redirect unauthenticated user to /login from protected route', async () => {
    mockUseAuth({ isInitializing: false, session: null });
    render(<App />);
    // App root redirects to /logistics/dashboard → RequireAuth → /login
    expect(await screen.findByRole('heading', { name: /sign in/i })).toBeInTheDocument();
  });

  it('should show loading indicator while session is initializing', async () => {
    mockUseAuth({ isInitializing: true, session: null });
    render(<App />);
    expect(await screen.findByRole('status')).toHaveTextContent('Restoring session…');
  });
});
