import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { QueryClientProvider } from '@tanstack/react-query';
import { queryClient } from './api/queryClient';
import { RequireAuth, RequirePermission } from './app/RequireAuth';

// Placeholder Pages
const Login = () => <div>Login Page</div>;
const Forbidden = () => <div>403 Forbidden</div>;
const LogisticsDashboard = () => <div>Logistics Company Dashboard</div>;
const GovernmentDashboard = () => <div>Government Operations Dashboard</div>;
const ControlTower = () => <div>Enterprise Control Tower (Map + Alerts)</div>;

const App = () => {
  return (
    <QueryClientProvider client={queryClient}>
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<Login />} />
          <Route path="/403" element={<Forbidden />} />

          {/* Secure Routes */}
          <Route element={<RequireAuth />}>
            
            <Route path="/" element={<Navigate to="/logistics/dashboard" replace />} />
            
            {/* Logistics Portal */}
            <Route path="/logistics">
              <Route path="dashboard" element={<LogisticsDashboard />} />
              {/* Other logistics routes... */}
            </Route>

            {/* Government Portal */}
            <Route path="/government">
              <Route path="dashboard" element={<GovernmentDashboard />} />
              {/* Other gov routes... */}
            </Route>

            {/* Control Tower - Protected by specific permission */}
            <Route element={<RequirePermission permission="control-tower.read" />}>
              <Route path="/control-tower" element={<ControlTower />} />
            </Route>

          </Route>
        </Routes>
      </BrowserRouter>
    </QueryClientProvider>
  );
};

export default App;
