import React from 'react';
import { BrowserRouter, Routes, Route, Navigate } from 'react-router-dom';
import { QueryClientProvider } from '@tanstack/react-query';
import { queryClient } from './api/queryClient';
import { RequireAuth, RequirePermission } from './app/RequireAuth';
import { AuthProvider } from './app/auth/AuthContext';
import { AuthPage } from './features/auth/AuthPage';
import { AppLayout } from './components/layout/AppLayout';
import { LogisticsDashboard } from './features/logistics/LogisticsDashboard';
import { CustomsDashboard } from './features/customs/CustomsDashboard';
import { ControlTowerDashboard } from './features/controlTower/ControlTowerDashboard';
import { DryportDashboard } from './features/dryport/DryportDashboard';
import { ShieldX } from 'lucide-react';

const Forbidden = () => (
  <div style={{
    minHeight: '80vh',
    display: 'flex',
    flexDirection: 'column',
    alignItems: 'center',
    justifyContent: 'center',
    textAlign: 'center',
    gap: '1rem',
    color: '#f8fafc'
  }}>
    <ShieldX size={64} color="#ef4444" />
    <h1 style={{ fontSize: '2rem', fontWeight: 800 }}>403 Forbidden</h1>
    <p style={{ color: '#94a3b8', maxWidth: '420px' }}>
      You do not possess the required tenant security clearance or permissions to inspect this operational node.
    </p>
    <a 
      href="/logistics/dashboard" 
      style={{
        marginTop: '1rem',
        padding: '0.65rem 1.25rem',
        background: '#1e293b',
        color: '#38bdf8',
        borderRadius: '8px',
        textDecoration: 'none',
        fontWeight: 600,
        fontSize: '0.875rem'
      }}
    >
      Return to Operations Hub
    </a>
  </div>
);

const App: React.FC = () => {
  return (
    <QueryClientProvider client={queryClient}>
      <AuthProvider>
        <BrowserRouter>
          <Routes>
            {/* Public Enterprise Authentication */}
            <Route path="/login" element={<AuthPage />} />
            <Route path="/403" element={<Forbidden />} />

            {/* Secure Authenticated Workspaces */}
            <Route element={<RequireAuth />}>
              <Route element={<AppLayout />}>
                <Route path="/" element={<Navigate to="/logistics/dashboard" replace />} />
                
                {/* Logistics Freight Portal */}
                <Route path="/logistics">
                  <Route path="dashboard" element={<LogisticsDashboard />} />
                </Route>

                {/* Government & Customs Operations */}
                <Route path="/government">
                  <Route path="dashboard" element={<CustomsDashboard />} />
                </Route>

                {/* Dryport & Demurrage Container Yard */}
                <Route path="/dryport">
                  <Route path="dashboard" element={<DryportDashboard />} />
                </Route>

                {/* Multimodal Control Tower - Permission Protected */}
                <Route element={<RequirePermission permission="control-tower.read" />}>
                  <Route path="/control-tower" element={<ControlTowerDashboard />} />
                </Route>
              </Route>
            </Route>
          </Routes>
        </BrowserRouter>
      </AuthProvider>
    </QueryClientProvider>
  );
};

export default App;
