import { BrowserRouter, Routes, Route, Navigate, useNavigate } from 'react-router-dom';
import { FormEvent, useState } from 'react';
import { QueryClientProvider } from '@tanstack/react-query';
import { queryClient } from './api/queryClient';
import { RequireAuth, RequirePermission } from './app/RequireAuth';
import { AuthProvider } from './app/auth/AuthContext';
import { useAuth } from './app/auth/AuthContext';

const Login = () => {
  const { login } = useAuth();
  const navigate = useNavigate();
  const [error, setError] = useState('');
  const [pending, setPending] = useState(false);

  const submit = async (event: FormEvent<HTMLFormElement>) => {
    event.preventDefault();
    setPending(true);
    setError('');
    const form = new FormData(event.currentTarget);
    try {
      const response = await fetch('/api/v1/identity/auth/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ tenantCode: form.get('tenantCode'), email: form.get('email'), password: form.get('password') }),
      });
      if (!response.ok) throw new Error('Sign in failed');
      const result = await response.json();
      if (typeof result.accessToken !== 'string' || !Array.isArray(result.permissions)) throw new Error('Invalid sign in response');
      login({
        accessToken: result.accessToken,
        tenantCode: String(form.get('tenantCode')),
        permissions: result.permissions,
        roles: Array.isArray(result.roles) ? result.roles : [],
        tenantType: typeof result.tenantType === 'string' ? result.tenantType : '',
      });
      navigate('/', { replace: true });
    } catch {
      setError('Unable to sign in. Check your credentials and try again.');
    } finally {
      setPending(false);
    }
  };

  return <main>
    <h1>Sign in</h1>
    {error && <p role="alert">{error}</p>}
    <form onSubmit={submit}>
      <label>Tenant code<input name="tenantCode" autoComplete="organization" required /></label>
      <label>Email<input name="email" type="email" autoComplete="username" required /></label>
      <label>Password<input name="password" type="password" autoComplete="current-password" required /></label>
      <button type="submit" disabled={pending}>{pending ? 'Signing in…' : 'Sign in'}</button>
    </form>
  </main>;
};
const Forbidden = () => <div>403 Forbidden</div>;
const LogisticsDashboard = () => <div>Logistics Company Dashboard</div>;
const GovernmentDashboard = () => <div>Government Operations Dashboard</div>;
const ControlTower = () => <div>Enterprise Control Tower (Map + Alerts)</div>;

const App = () => {
  return (
    <QueryClientProvider client={queryClient}>
      <AuthProvider>
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
      </AuthProvider>
    </QueryClientProvider>
  );
};

export default App;
