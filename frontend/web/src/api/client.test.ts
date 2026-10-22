import { api } from './client';
import { apiClient } from '../services/api/apiClient';

jest.mock('../services/api/apiClient', () => ({ apiClient: { get: jest.fn() } }));

describe('api client re-export', () => {
  it('exposes the shared apiClient instance', () => {
    expect(api).toBe(apiClient);
  });
});
