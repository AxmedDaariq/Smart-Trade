import { createContext, ReactNode, useContext, useEffect, useMemo, useState } from 'react';
import { setAccessToken } from '../../services/api/apiClient';

export interface AuthSession {
  accessToken: string;
  tenantCode: string;
  permissions: string[];
  roles: string[];
  tenantType: string;
}

interface AuthContextValue {
  session: AuthSession | null;
  isInitializing: boolean;
  login: (session: AuthSession) => void;
  logout: () => void;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);
let sessionRestoration: Promise<AuthSession | null> | null = null;

function restoreSessionOnce(): Promise<AuthSession | null> {
  if (!sessionRestoration) {
    sessionRestoration = fetch('/api/v1/identity/auth/refresh', { method: 'POST', credentials: 'same-origin' })
      .then(async (response) => {
        if (!response.ok) return null;
        const result = await response.json();
        if (typeof result.accessToken !== 'string' || typeof result.tenantCode !== 'string' || !Array.isArray(result.permissions)) return null;
        return {
          accessToken: result.accessToken,
          tenantCode: result.tenantCode,
          tenantType: typeof result.tenantType === 'string' ? result.tenantType : '',
          roles: Array.isArray(result.roles) ? result.roles : [],
          permissions: result.permissions,
        } satisfies AuthSession;
      })
      .catch(() => null);
  }
  return sessionRestoration;
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [session, setSession] = useState<AuthSession | null>(null);
  const [isInitializing, setIsInitializing] = useState(true);
  useEffect(() => {
    let active = true;
    void restoreSessionOnce().then((restoredSession) => {
      if (!active) return;
      if (restoredSession) {
        setAccessToken(restoredSession.accessToken);
        setSession(restoredSession);
      }
      setIsInitializing(false);
    });
    return () => { active = false; };
  }, []);
  const login = (nextSession: AuthSession) => {
    sessionRestoration = null;
    setAccessToken(nextSession.accessToken);
    setSession(nextSession);
  };
  const logout = async () => {
    try {
      await fetch('/api/v1/identity/auth/logout', { method: 'POST', credentials: 'same-origin' });
    } finally {
      sessionRestoration = null;
      setAccessToken(null);
      setSession(null);
    }
  };
  const value = useMemo(() => ({ session, isInitializing, login, logout }), [session, isInitializing]);
  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) throw new Error('useAuth must be used within AuthProvider');
  return context;
}
