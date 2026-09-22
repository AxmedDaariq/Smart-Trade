import axios from 'axios';

// The BFF / API Gateway handles routing this to the correct backend microservice
export const api = axios.create({
  baseURL: '/api/v1',
  headers: {
    'Content-Type': 'application/json',
  },
  withCredentials: true, // Crucial for HttpOnly cookies provided by BFF Identity
});

api.interceptors.response.use(
  (response) => response,
  (error) => {
    if (error.response?.status === 401) {
      // Dispatch global event or call auth store to redirect to login
      window.location.href = '/login';
    }
    return Promise.reject(error);
  }
);
