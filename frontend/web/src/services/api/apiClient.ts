import axios from 'axios';

export const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_GATEWAY_URL || '/api/v1',
  timeout: 10000,
});

apiClient.interceptors.request.use((config) => {
  // Inject JWT from secure storage/cookie
  const token = localStorage.getItem('access_token');
  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }
  return config;
});

apiClient.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Trigger global logout / session refresh
      window.location.href = '/login';
    }
    // Never expose stack traces to user
    return Promise.reject(error);
  }
);
