import React from 'react';
import { render, screen, act } from '@testing-library/react';
import { AuthProvider, useAuth } from '../AuthContext';
import '@testing-library/jest-dom';

// Test component that allows us to trigger login and check permissions
const TestUserSession = ({ permissionToTest }: { permissionToTest: string }) => {
  const { login, hasPermission, isAuthenticated } = useAuth();

  return (
    <div>
      <div data-testid="auth-status">{isAuthenticated ? 'true' : 'false'}</div>
      <div data-testid="perm-status">{hasPermission(permissionToTest) ? 'true' : 'false'}</div>
      <button
        data-testid="login-btn"
        onClick={() => login('TENT001', ['users.read', 'customs.read'])}
      >
        Login
      </button>
    </div>
  );
};

describe('AuthContext Security Suite', () => {
  test('Initial State: Should be unauthenticated and have no permissions', () => {
    render(
      <AuthProvider>
        <TestUserSession permissionToTest="users.read" />
      </AuthProvider>
    );

    expect(screen.getByTestId('auth-status').textContent).toBe('false');
    expect(screen.getByTestId('perm-status').textContent).toBe('false');
  });

  test('Post-Login: Should grant specific permissions and deny others', async () => {
    render(
      <AuthProvider>
        <TestUserSession permissionToTest="users.read" />
      </AuthProvider>
    );

    // Trigger login
    const loginBtn = screen.getByTestId('login-btn');
    act(() => {
      loginBtn.click();
    });

    // Verify authentication status
    expect(screen.getByTestId('auth-status').textContent).toBe('true');

    // Verify granted permission
    expect(screen.getByTestId('perm-status').textContent).toBe('true');
  });

  test('Security Boundary: Should deny permissions not present in token', () => {
    render(
      <AuthProvider>
        <TestUserSession permissionToTest="admin.all" />
      </AuthProvider>
    );

    const loginBtn = screen.getByTestId('login-btn');
    act(() => {
      loginBtn.click();
    });

    // Authenticated, but should NOT have admin.all permission
    expect(screen.getByTestId('auth-status').textContent).toBe('true');
    expect(screen.getByTestId('perm-status').textContent).toBe('false');
  });
});
