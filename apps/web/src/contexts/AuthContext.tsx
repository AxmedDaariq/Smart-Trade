import React, { createContext, useContext, useState, useEffect } from 'react';

interface AuthState {
  isAuthenticated: boolean;
  accessToken: string | null;
  tenantCode: string | null;
  permissions: string[];
}

interface AuthContextType extends AuthState {
  login: (token: string, tenant: string, perms: string[]) => void;
  logout: () => void;
  hasPermission: (permission: string) => boolean;
}

const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider: React.FC<{ children: React.ReactNode }> = ({ children }) => {
  const [authState, setAuthState] = useState<AuthState>({
    isAuthenticated: false,
    accessToken: null,
    tenantCode: null,
    permissions: [],
  });

  const login = (token: string, tenant: string, perms: string[]) => {
    // In a real app, securely store the token (e.g. secure HTTP-only cookies preferred, 
    // or memory + refresh token rotation)
    setAuthState({
      isAuthenticated: true,
      accessToken: token,
      tenantCode: tenant,
      permissions: perms,
    });
  };

  const logout = () => {
    setAuthState({
      isAuthenticated: false,
      accessToken: null,
      tenantCode: null,
      permissions: [],
    });
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
