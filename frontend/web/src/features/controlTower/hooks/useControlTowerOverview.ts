import { useQuery } from '@tanstack/react-query';
import { api } from '../api/client';

interface ControlTowerOverview {
  activeTrips: number;
  vehiclesLive: number;
  vehiclesStale: number;
  pendingCustoms: number;
  openAlerts: number;
}

export const useControlTowerOverview = () => {
  return useQuery<ControlTowerOverview>({
    queryKey: ['control-tower', 'overview'],
    queryFn: async () => {
      const response = await api.get('/control-tower/overview');
      return response.data;
    },
    // The query is refreshed when a specific SignalR event arrives (in a separate hook), 
    // but we can also set a slow polling interval as a fallback
    refetchInterval: 30000, 
  });
};
