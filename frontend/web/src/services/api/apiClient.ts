import axios from 'axios';

let accessToken: string | null = null;

export const setAccessToken = (token: string | null) => {
  accessToken = token;
};

export const apiClient = axios.create({
  baseURL: import.meta.env.VITE_API_GATEWAY_URL || '/api/v1',
  timeout: 10000,
  withCredentials: true,
});

apiClient.interceptors.request.use((config) => {
  if (accessToken) {
    config.headers.Authorization = `Bearer ${accessToken}`;
  }
  return config;
});

type RetryableRequest = NonNullable<import('axios').AxiosError['config']> & { _authRetry?: boolean };
let refreshRequest: Promise<string> | null = null;

apiClient.interceptors.response.use((response) => response, async (error: import('axios').AxiosError) => {
  const request = error.config as RetryableRequest | undefined;
  const isAuthEndpoint = request?.url?.includes('/identity/auth/');
  if (error.response?.status !== 401 || !request || request._authRetry || isAuthEndpoint || !accessToken) {
    return Promise.reject(error);
  }

  request._authRetry = true;
  try {
    refreshRequest ??= apiClient.post('/identity/auth/refresh')
      .then(({ data }) => {
        if (typeof data.accessToken !== 'string') throw new Error('Invalid refresh response');
        setAccessToken(data.accessToken);
        return data.accessToken as string;
      })
      .finally(() => { refreshRequest = null; });
    const newToken = await refreshRequest;
    request.headers.set('Authorization', `Bearer ${newToken}`);
    return apiClient(request);
  } catch (refreshError) {
    setAccessToken(null);
    window.location.href = '/login';
    return Promise.reject(refreshError);
  }
});
