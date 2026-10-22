import { renderHook, waitFor } from '@testing-library/react';
import { QueryClient, QueryClientProvider } from '@tanstack/react-query';
import { ReactNode } from 'react';
import { api } from '../../../api/client';
import { useControlTowerOverview } from './useControlTowerOverview';

// Mock the shared API client so no real HTTP call is made.
jest.mock('../../../api/client', () => ({ api: { get: jest.fn() } }));

const mockGet = api.get as jest.Mock;

const createWrapper = () => {
  const queryClient = new QueryClient({
    defaultOptions: { queries: { retry: false } },
  });
  const Wrapper = ({ children }: { children: ReactNode }) => (
    <QueryClientProvider client={queryClient}>{children}</QueryClientProvider>
  );
  return Wrapper;
};

describe('useControlTowerOverview', () => {
  afterEach(() => jest.clearAllMocks());

  it('fetches the control tower overview from the API', async () => {
    const overview = {
      activeTrips: 12,
      vehiclesLive: 8,
      vehiclesStale: 2,
      pendingCustoms: 3,
      openAlerts: 1,
    };
    mockGet.mockResolvedValueOnce({ data: overview });

    const { result } = renderHook(() => useControlTowerOverview(), {
      wrapper: createWrapper(),
    });

    await waitFor(() => expect(result.current.isSuccess).toBe(true));

    expect(mockGet).toHaveBeenCalledWith('/control-tower/overview');
    expect(result.current.data).toEqual(overview);
  });

  it('surfaces an error state when the API call fails', async () => {
    mockGet.mockRejectedValueOnce(new Error('boom'));

    const { result } = renderHook(() => useControlTowerOverview(), {
      wrapper: createWrapper(),
    });

    await waitFor(() => expect(result.current.isError).toBe(true));
  });
});
