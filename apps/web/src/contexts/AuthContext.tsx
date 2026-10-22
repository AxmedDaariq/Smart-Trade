import React, { createContext, useContext, useState, useEffect } from 'react';

interface AuthState {
  isAuthenticated: boolean;
  tenantCode: string | null;
  permissions: string[];
}

interface AuthContextType extends AuthState {
  login: (tenant: string, perms: string[]) => void;
  logout: () => void;
  hasPermission: (permission: string) => boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [authState, setAuthState] = useState<AuthState>({
    isAuthenticated: false,
    tenantCode: null,
    permissions: [],
  });

  const login = (tenant: string, perms: string[]) => {
    // Tokens are now handled by the backend via Secure, HttpOnly cookies.
    // The frontend only maintains the session state and user identity.
    setAuthState({
      isAuthenticated: true,
      tenantCode: tenant,
      permissions: perms,
    });
  };

  const logout = async () => {
    try {
      // Call backend to clear the HttpOnly cookie
      await fetch('/api/v1/identity/auth/logout', { method: 'POST' });
    } catch (e) {
      console.error('Logout request failed', e);
    } finally {
      setAuthState({
        isAuthenticated: false,
        tenantCode: null,
        permissions: [],
      });
    }
  };

  const hasPermission = (permission: string) => {
    return authState.permissions.includes(permission);
  };

  return (
    <AuthContext.Provider value={{ ...authState, login, logout, hasPermission }}>
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (context === undefined) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};
