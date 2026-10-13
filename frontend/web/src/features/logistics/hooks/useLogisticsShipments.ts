import { useQuery } from '@tanstack/react-query';
import { api } from '../../../api/client';

export interface Shipment {
  id: string;
  trackingNumber: string;
  consignee: string;
  origin: string;
  destination: string;
  containersCount: number;
  status: string;
  eta: string;
  riskLevel: string;
}

interface BackendShipment {
  id: string;
  shipmentNumber: string;
  shipperReference: string;
  consigneeReference: string;
  origin: string;
  destination: string;
  status: string;
  transportMode: string;
  estimatedArrivalUtc: string;
  createdAtUtc: string;
}

interface PaginatedResponse<T> {
  total: number;
  page: number;
  pageSize: number;
  items: T[];
}

export const useLogisticsShipments = () => {
  return useQuery({
    queryKey: ['logistics', 'shipments'],
    queryFn: async (): Promise<Shipment[]> => {
      const { data } = await api.get<PaginatedResponse<BackendShipment>>('/shipments');

      // Map backend items to frontend Shipment interface
      return data.items.map(item => ({
        id: item.id,
        trackingNumber: item.shipmentNumber,
        consignee: item.consigneeReference || 'Unknown Consignee',
        origin: item.origin,
        destination: item.destination,
        containersCount: 0, // Backend list endpoint doesn't provide count, would need detail call
        status: item.status,
        eta: new Date(item.estimatedArrivalUtc).toLocaleString(),
        riskLevel: 'Low' // Defaulting since it's not in the list response
      }));
    },
  });
};
