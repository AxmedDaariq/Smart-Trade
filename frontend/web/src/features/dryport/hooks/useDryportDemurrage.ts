import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { api } from '../../../api/client';

export interface ContainerDemurrage {
  containerId: string;
  containerNumber: string;
  status: string;
  daysInPort: number;
  freeDays: number;
  accruedDemurrage: number;
  currency: string;
}

export interface AddContainerRequest {
  containerNumber: string;
  size: string;
  type: string;
  demurrageRuleId: string;
}

export const useDryportDemurrage = () => {
  const queryClient = useQueryClient();

  // Fetch containers and their demurrage status
  const containersQuery = useQuery({
    queryKey: ['dryport', 'containers'],
    queryFn: async (): Promise<ContainerDemurrage[]> => {
      const { data } = await api.get<ContainerDemurrage[]>('/Dryport/containers/demurrage');
      return data;
    },
  });

  // Mutation to add a new container
  const addContainerMutation = useMutation({
    mutationFn: async (newContainer: AddContainerRequest) => {
      const { data } = await api.post('/Dryport/containers/receive', newContainer);
      return data;
    },
    onSuccess: () => {
      // Invalidate and refetch containers list
      queryClient.invalidateQueries({ queryKey: ['dryport', 'containers'] });
    },
  });

  return {
    containers: containersQuery.data ?? [],
    isLoading: containersQuery.isLoading,
    isError: containersQuery.isError,
    error: containersQuery.error,
    addContainer: addContainerMutation.mutateAsync,
    isAdding: addContainerMutation.isPending,
  };
};
