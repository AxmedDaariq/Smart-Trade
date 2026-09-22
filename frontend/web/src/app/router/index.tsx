import { createBrowserRouter } from 'react-router-dom';
import { RequirePermission } from '../auth/RequirePermission';

export const router = createBrowserRouter([
  {
    path: '/login',
    element: <div>Login Page</div>,
  },
  {
    path: '/unauthorized',
    element: <div>403 - Unauthorized. Frontend authorization is not the security boundary.</div>,
  },
  {
    path: '/',
    element: <div>Global App Shell Placeholder</div>,
    children: [
      {
        path: 'trader',
        element: (
          <RequirePermission requiredTenantAccess="TRADER">
            <div>Trader Portal</div>
          </RequirePermission>
        ),
      },
      {
        path: 'customs',
        element: (
          <RequirePermission requiredRole="CUSTOMS_OFFICER">
            <div>Customs Portal</div>
          </RequirePermission>
        ),
      },
      {
        path: 'government/customs/configuration',
        element: (
          <RequirePermission requiredPermission="customs.configuration.read">
            <div>Government Customs Configuration UI</div>
          </RequirePermission>
        ),
      },
      {
        path: 'logistics',
        element: (
          <RequirePermission requiredTenantAccess="LOGISTICS_COMPANY">
            <div>Logistics Portal (Trips, Tracking, Fleet)</div>
          </RequirePermission>
        ),
      },
      {
        path: 'control-tower',
        element: (
          <RequirePermission requiredPermission="controltower.read">
            <div>Control Tower Main Dashboard (Real-time Map, KPIs, Timeline)</div>
          </RequirePermission>
        ),
      },
    ],
  },
]);
