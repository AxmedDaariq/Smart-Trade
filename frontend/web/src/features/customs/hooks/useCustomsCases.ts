import { useQuery } from '@tanstack/react-query';
import { api } from '../../../api/client';

export interface CustomsCase {
  caseNumber: string;
  declarationNumber: string;
  declarant: string;
  procedureCode: string;
  cifValue: number;
  assessedDuty: number;
  channel: 'Green' | 'Yellow' | 'Red';
  status: string;
}

export const useCustomsCases = () => {
  return useQuery({
    queryKey: ['customs', 'cases'],
    queryFn: async (): Promise<CustomsCase[]> => {
      // Note: Based on the backend controller, we might need to map a specific endpoint
      // as CustomsCasesController seems to focus on case creation and risk.
      // For a dashboard list, we'll target the declarations or cases endpoint.
      const { data } = await api.get<CustomsCase[]>('/CustomsCases');
      return data;
    },
  });
};
