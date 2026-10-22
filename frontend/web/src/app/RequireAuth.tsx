import React from 'react';
import { Navigate, Outlet, useLocation } from 'react-router-dom';
import { useAuth } from './auth/AuthContext';

export const RequireAuth: React.FC = () => {
  const { session, isInitializing } = useAuth();
  const location = useLocation();

  if (isInitializing) return <div role="status">Restoring session…</div>;
  if (!session) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  return <Outlet />;
};

export const RequirePermission: React.FC<{ permission: string }> = ({ permission }) => {
  const { session } = useAuth();
  const hasPermission = session?.permissions.includes(permission) ?? false;

  if (!hasPermission) {
    return <Navigate to="/403" replace />;
  }

  return <Outlet />;
};
