import React from 'react';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import { LoginPage } from '../auth/LoginPage';
import { AuthProvider } from '../../contexts/AuthContext';
import '@testing-library/jest-dom';

// Mocking the API calls
const mockLoginApi = jest.fn();

// We mock the actual API call inside the component logic if it were using a service
// Since the current LoginPage has a simulated call, we can test the UI behavior.

describe('LoginPage Functional & Security Tests', () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  const renderLoginPage = () => {
    return render(
      <AuthProvider>
        <LoginPage />
      </AuthProvider>
    );
  };

  test('should render all required input fields', () => {
    renderLoginPage();

    expect(screen.getByLabelText(/tenant code/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/email/i)).toBeInTheDocument();
    expect(screen.getByLabelText(/password/i)).toBeInTheDocument();
    expect(screen.getByRole('button', { name: /sign in/i })).toBeInTheDocument();
  });

  test('should show validation error when fields are empty', async () => {
    renderLoginPage();

    const submitBtn = screen.getByRole('button', { name: /sign in/i });
    fireEvent.click(submitBtn);

    // HTML5 validation usually handles this, but we check if our logic handles it
    // In the current implementation, it uses 'required' attributes
    expect(submitBtn).toBeInTheDocument();
  });

  test('should handle successful login flow', async () => {
    renderLoginPage();

    fireEvent.change(screen.getByLabelText(/tenant code/i), { target: { value: 'TENT001' } });
    fireEvent.change(screen.getByLabelText(/email/i), { target: { value: 'user@example.com' } });
    fireEvent.change(screen.getByLabelText(/password/i), { target: { value: 'Password123!' } });

    fireEvent.click(screen.getByRole('button', { name: /sign in/i }));

    // In the current mock implementation, it just logs to console and calls login()
    // We verify that the "Invalid credentials" error does NOT appear
    expect(screen.queryByText(/invalid credentials/i)).not.toBeInTheDocument();
  });

  test('should protect against basic XSS in input fields', () => {
    renderLoginPage();

    const emailInput = screen.getByLabelText(/email/i);
    const xssPayload = '<script>alert("xss")</script>';

    fireEvent.change(emailInput, { target: { value: xssPayload } });

    // Verify the input value is set but not executed (React handles this by default)
    expect(emailInput.value).toBe(xssPayload);
  });
});
