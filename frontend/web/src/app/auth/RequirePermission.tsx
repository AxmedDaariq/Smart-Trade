import React from 'react';
import { Navigate, useLocation } from 'react-router-dom';

interface RequirePermissionProps {
  children: JSX.Element;
  requiredPermission?: string;
  requiredRole?: string;
  requiredTenantAccess?: string;
}

// Mock auth hook for scaffolding
const useAuth = () => {
  return {
    isAuthenticated: true,
    user: {
      roles: ['CUSTOMS_OFFICER'],
      permissions: ['customs.configuration.read'],
      tenantType: 'GOVERNMENT',
    },
    isLoading: false,
  };
};

export const RequirePermission: React.FC<RequirePermissionProps> = ({
  children,
  requiredPermission,
  requiredRole,
  requiredTenantAccess,
}) => {
  const { isAuthenticated, user, isLoading } = useAuth();
  const location = useLocation();

  if (isLoading) return <div>Loading session...</div>;

  if (!isAuthenticated) {
    return <Navigate to="/login" state={{ from: location }} replace />;
  }

  if (requiredRole && !user.roles.includes(requiredRole)) {
    return <Navigate to="/unauthorized" replace />;
  }

  if (requiredPermission && !user.permissions.includes(requiredPermission)) {
    return <Navigate to="/unauthorized" replace />;
  }

  if (requiredTenantAccess && user.tenantType !== requiredTenantAccess) {
    return <Navigate to="/unauthorized" replace />;
  }

  return children;
};
