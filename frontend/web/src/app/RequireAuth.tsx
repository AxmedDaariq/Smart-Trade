import React from 'react';
import { Navigate, Outlet, useLocation } from 'react-router-dom';
// import { useAuthStore } from '../stores/authStore'; // Imaginary global state store

export const RequireAuth: React.FC = () => {
  // const { isAuthenticated, isInitializing } = useAuthStore();
  const isAuthenticated = true; // Mocked for scaffolding
  const isInitializing = false;
  const location = useLocation();

  if (isInitializing) {
    return <div>Loading authentication state...</div>;
  }

  if (!isAuthenticated) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  return <Outlet />;
};

export const RequirePermission: React.FC<{ permission: string }> = ({ permission }) => {
  // const { permissions } = useAuthStore();
  const permissions = ['control-tower.read']; // Mocked
  const hasPermission = permissions.includes(permission);

  if (!hasPermission) {
    return <Navigate to="/403" replace />;
  }

  return <Outlet />;
};
