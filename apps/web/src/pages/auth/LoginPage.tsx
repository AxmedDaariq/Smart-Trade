import React, { useState } from 'react';
import { useAuth } from '../../contexts/AuthContext';

export const LoginPage: React.FC = () => {
  const { login } = useAuth();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [tenantCode, setTenantCode] = useState('');
  const [error, setError] = useState('');

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    try {
      // Simulate API call to POST /api/v1/identity/auth/login
      console.log('Logging in with', { email, tenantCode });
      
      // Simulate successful response
      // The backend now sets a Secure, HttpOnly cookie.
      // The frontend no longer receives the token in the response body.
      login(tenantCode, ['users.read', 'customs.read']);

      
    } catch (err) {
      setError('Invalid credentials or tenant code');
    }
  };

  return (
    <div style={{ maxWidth: '400px', margin: '100px auto', fontFamily: 'sans-serif' }}>
      <h2>Sign in to Smart Trade Platform</h2>
      {error && <p style={{ color: 'red' }}>{error}</p>}
      <form onSubmit={handleLogin} style={{ display: 'flex', flexDirection: 'column', gap: '15px' }}>
        <div>
          <label>Tenant Code</label>
          <input 
            type="text" 
            value={tenantCode} 
            onChange={(e) => setTenantCode(e.target.value)} 
            required 
            style={{ width: '100%', padding: '8px' }}
          />
        </div>
        <div>
          <label>Email</label>
          <input 
            type="email" 
            value={email} 
            onChange={(e) => setEmail(e.target.value)} 
            required 
            style={{ width: '100%', padding: '8px' }}
          />
        </div>
        <div>
          <label>Password</label>
          <input 
            type="password" 
            value={password} 
            onChange={(e) => setPassword(e.target.value)} 
            required 
            style={{ width: '100%', padding: '8px' }}
          />
        </div>
        <button type="submit" style={{ padding: '10px', background: '#0056b3', color: 'white', border: 'none' }}>
          Sign In
        </button>
      </form>
    </div>
  );
};
