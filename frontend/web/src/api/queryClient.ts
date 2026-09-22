import { QueryClient } from '@tanstack/react-query';

export const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 1000 * 60 * 5, // 5 minutes by default
      retry: 2,
      refetchOnWindowFocus: false, // Enterprise apps usually prefer manual refresh for heavy grids
    },
  },
});
