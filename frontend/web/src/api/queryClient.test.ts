import { queryClient } from './queryClient';

describe('queryClient', () => {
  it('applies enterprise default query options', () => {
    const defaults = queryClient.getDefaultOptions().queries;

    expect(defaults?.staleTime).toBe(1000 * 60 * 5); // 5 minutes
    expect(defaults?.retry).toBe(2);
    expect(defaults?.refetchOnWindowFocus).toBe(false);
  });
});
