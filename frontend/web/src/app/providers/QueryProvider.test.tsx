import { render, screen } from '@testing-library/react';
import { useQueryClient } from '@tanstack/react-query';
import { QueryProvider } from './QueryProvider';

const ClientProbe = () => {
  const client = useQueryClient();
  return <span data-testid="has-client">{String(Boolean(client))}</span>;
};

describe('QueryProvider', () => {
  it('renders its children', () => {
    render(
      <QueryProvider>
        <div>child content</div>
      </QueryProvider>,
    );

    expect(screen.getByText('child content')).toBeInTheDocument();
  });

  it('provides a QueryClient to its children', () => {
    render(
      <QueryProvider>
        <ClientProbe />
      </QueryProvider>,
    );

    expect(screen.getByTestId('has-client')).toHaveTextContent('true');
  });
});
