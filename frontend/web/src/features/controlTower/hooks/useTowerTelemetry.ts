import { useQuery } from '@tanstack/react-query';
import { api } from '../../../api/client';

export interface TowerKpi {
  code: string;
  name: string;
  value: string | number;
  trend: string;
  status: 'optimal' | 'warning' | 'critical';
}

export const useTowerTelemetry = () => {
  // Fetching general KPIs for the tower
  const kpisQuery = useQuery({
    queryKey: ['tower', 'kpis'],
    queryFn: async (): Promise<TowerKpi[]> => {
      const { data } = await api.get<TowerKpi[]>('/analytics/Kpis');
      return data;
    },
  });

  // Fetching transit time specifically
  const transitTimeQuery = useQuery({
    queryKey: ['tower', 'transit-time'],
    queryFn: async () => {
      const { data } = await api.get('/analytics/kpis/transit-time');
      return data;
    },
  });

  return {
    kpis: kpisQuery.data ?? [],
    transitTime: transitTimeQuery.data,
    isLoading: kpisQuery.isLoading || transitTimeQuery.isLoading,
    isError: kpisQuery.isError || transitTimeQuery.isError,
  };
};
