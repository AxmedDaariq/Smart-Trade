import React from 'react';
import { Navigate, useLocation } from 'react-router-dom';
import { useAuth } from './AuthContext';

interface RequirePermissionProps {
  children: JSX.Element;
  requiredPermission?: string;
  requiredRole?: string;
  requiredTenantAccess?: string;
}

export const RequirePermission: React.FC<RequirePermissionProps> = ({
  children,
  requiredPermission,
  requiredRole,
  requiredTenantAccess,
}) => {
  const { session, isInitializing } = useAuth();
  const location = useLocation();

  if (isInitializing) return <div role="status">Restoring session…</div>;
  if (!session) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  if (requiredRole && !session.roles.includes(requiredRole)) {
    return <Navigate to="/unauthorized" replace />;
  }

  if (requiredPermission && !session.permissions.includes(requiredPermission)) {
    return <Navigate to="/unauthorized" replace />;
  }

  if (requiredTenantAccess && session.tenantType !== requiredTenantAccess) {
    return <Navigate to="/unauthorized" replace />;
  }

  return children;
};
