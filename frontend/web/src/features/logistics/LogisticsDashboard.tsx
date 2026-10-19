import React, { useState } from 'react';
import {
  Truck,
  Ship,
  Clock,
  ShieldCheck,
  Plus,
  Download,
  Navigation,
  ExternalLink,
  ChevronRight,
  CheckCircle2,
  AlertTriangle
} from 'lucide-react';
import './LogisticsDashboard.css';
import { useLogisticsShipments } from './hooks/useLogisticsShipments';

export const LogisticsDashboard: React.FC = () => {
  const {
    data: shipments = [],
    isLoading,
    isError,
    error
  } = useLogisticsShipments();

  const [filter, setFilter] = useState<string>('All');

  const filteredShipments = filter === 'All'
    ? shipments
    : shipments.filter(s => s.status.toLowerCase().includes(filter.toLowerCase()));

  const getStatusBadge = (status: string) => {
    switch (status) {
      case 'Customs Cleared':
        return <span className="badge badge-success" data-cy="shipment-status-badge"><CheckCircle2 size={12} /> Cleared</span>;
      case 'In-Transit':
        return <span className="badge badge-info" data-cy="shipment-status-badge"><Truck size={12} /> In Transit</span>;
      case 'Border Gate':
        return <span className="badge badge-warning" data-cy="shipment-status-badge"><AlertTriangle size={12} /> Border Inspection</span>;
      default:
        return <span className="badge badge-info" data-cy="shipment-status-badge"><Ship size={12} /> Port Yard</span>;
    }
  };

  if (isLoading) return <div className="dashboard-page"><h2 style={{ textAlign: 'center', padding: '2rem' }}>Loading Logistics Data...</h2></div>;
  if (isError) return <div className="dashboard-page"><h2 style={{ textAlign: 'center', padding: '2rem', color: '#ef4444' }}>Error: {error instanceof Error ? error.message : 'Unknown Error'}</h2></div>;

  return (
    <div className="dashboard-page animate-fade-in">
      {/* Top Hero Banner */}
      <div className="dashboard-hero-header">
        <div>
          <h1 className="dashboard-hero-title">
            <Truck size={32} color="#3b82f6" />
            Logistics & Freight Hub
          </h1>
          <p className="dashboard-hero-desc">
            Real-time multimodal corridor tracking, ECTN declaration integration, and fleet dispatches.
          </p>
        </div>

        <div className="hero-actions">
          <button type="button" className="btn-secondary" data-cy="btn-export-manifest">
            <Download size={16} /> Export Manifest
          </button>
          <button type="button" className="btn-primary" data-cy="btn-dispatch-consignment">
            <Plus size={16} /> Dispatch New Consignment
          </button>
        </div>
      </div>

      {/* KPI Stats Grid */}
      <div className="kpi-grid">
        <div className="kpi-card" data-cy="kpi-active-cargo">
          <div className="kpi-header">
            <span className="kpi-title">Active In-Transit Cargo</span>
            <div className="kpi-icon-wrap"><Ship size={20} /></div>
          </div>
          <div className="kpi-value">{shipments.length} <span style={{ fontSize: '1rem', color: '#94a3b8' }}>TEU</span></div>
          <div className="kpi-footer">
            <span className="kpi-trend-positive">↑ +14.2%</span> vs last calendar month
          </div>
        </div>

        <div className="kpi-card kpi-success" data-cy="kpi-on-time-delivery">
          <div className="kpi-header">
            <span className="kpi-title">On-Time Corridor Delivery</span>
            <div className="kpi-icon-wrap"><ShieldCheck size={20} color="#10b981" /></div>
          </div>
          <div className="kpi-value">98.4%</div>
          <div className="kpi-footer">
            <span className="kpi-trend-positive">Optimal</span> Average Dwell: 18.2 hrs
          </div>
        </div>

        <div className="kpi-card kpi-warning" data-cy="kpi-demurrage-risk">
          <div className="kpi-header">
            <span className="kpi-title">Demurrage Risk Flags</span>
            <div className="kpi-icon-wrap"><Clock size={20} color="#f59e0b" /></div>
          </div>
          <div className="kpi-value">3 <span style={{ fontSize: '1rem', color: '#f59e0b' }}>Alerts</span></div>
          <div className="kpi-footer">
            <span className="kpi-trend-negative">Requires Action</span> Free days &lt; 48 hrs
          </div>
        </div>

        <div className="kpi-card kpi-purple" data-cy="kpi-fleet-units">
          <div className="kpi-header">
            <span className="kpi-title">Active GPS Fleet Units</span>
            <div className="kpi-icon-wrap"><Navigation size={20} color="#a855f7" /></div>
          </div>
          <div className="kpi-value">46 <span style={{ fontSize: '1rem', color: '#94a3b8' }}>Trucks</span></div>
          <div className="kpi-footer">
            <span className="pulse-dot" style={{ backgroundColor: '#a855f7' }} />
            Connected via Satellite Telemetry
          </div>
        </div>
      </div>

      {/* Corridor Flow Milestone Track */}
      <div className="corridor-milestone-panel">
        <div className="corridor-header">
          <div>
            <h2 className="corridor-title">Strategic Trade Corridor: Berbera Port ⇄ Addis Ababa</h2>
            <div style={{ fontSize: '0.8125rem', color: '#94a3b8', marginTop: '0.2rem' }}>
              Distance: 920 km • Fast-track customs green channel enabled
            </div>
            </div>
          <span className="badge badge-success">Highway Cleared</span>
        </div>

        <div className="corridor-track">
          <div className="corridor-line" />
          <div className="corridor-line-progress" />

          <div className="corridor-stop passed" data-cy="corridor-node-berbera">
            <div className="corridor-node">✓</div>
            <div className="stop-name">Berbera Port</div>
            <div className="stop-meta">Terminal Quay</div>
          </div>

          <div className="corridor-stop passed" data-cy="corridor-node-hargeisa">
            <div className="corridor-node">✓</div>
            <div className="stop-name">Hargeisa Hub</div>
            <div className="stop-meta">Transit Hub</div>
          </div>

          <div className="corridor-stop current" data-cy="corridor-node-togwajaale">
            <div className="corridor-node">●</div>
            <div className="stop-name">Tog-Wajaale</div>
            <div className="stop-meta">Customs Border</div>
          </div>

          <div className="corridor-stop upcoming" data-cy="corridor-node-dire-dawa">
            <div className="corridor-node">○</div>
            <div className="stop-name">Dire Dawa</div>
            <div className="stop-meta">Dryport Rail</div>
          </div>

          <div className="corridor-stop upcoming" data-cy="corridor-node-addis">
            <div className="corridor-node">○</div>
            <div className="stop-name">Mojo / Addis</div>
            <div className="stop-meta">Destination ICD</div>
          </div>
        </div>
      </div>

      {/* Data Table: Active Shipments */}
      <div className="data-panel">
        <div className="panel-toolbar">
          <h2 className="panel-title">Active Consignments & Dispatches</h2>

          <div style={{ display: 'flex', gap: '0.5rem' }}>
            <button
              type="button"
              className={`btn-secondary ${filter === 'All' ? 'active' : ''}`}
              onClick={() => setFilter('All')}
              style={{ padding: '0.4rem 0.8rem', fontSize: '0.8125rem' }}
              data-cy="filter-all"
            >
              All ({shipments.length})
            </button>
            <button
              type="button"
              className="btn-secondary"
              onClick={() => setFilter('In-Transit')}
              style={{ padding: '0.4rem 0.8rem', fontSize: '0.8125rem' }}
              data-cy="filter-in-transit"
            >
              In Transit
            </button>
            <button
              type="button"
              className="btn-secondary"
              onClick={() => setFilter('Border')}
              style={{ padding: '0.4rem 0.8rem', fontSize: '0.8125rem' }}
              data-cy="filter-border"
            >
              Border Gate
            </button>
          </div>
        </div>

        <div className="table-wrapper">
          <table className="enterprise-table">
            <thead>
              <tr>
                <th>Waybill / B/L Code</th>
                <th>Consignee / Importer</th>
                <th>Corridor Route</th>
                <th>Volume</th>
                <th>Transit Status</th>
                <th>Corridor ETA</th>
                <th>Action</th>
              </tr>
            </thead>
            <tbody>
              {filteredShipments.map((shp) => (
                <tr key={shp.id} data-cy="shipment-table-row">
                  <td>
                    <span className="tracking-num">{shp.trackingNumber}</span>
                  </td>
                  <td style={{ fontWeight: 600, color: '#f1f5f9' }}>
                    {shp.consignee}
                  </td>
                  <td>
                    <div style={{ display: 'flex', alignItems: 'center', gap: '0.35rem', fontSize: '0.8125rem' }}>
                      <span>{shp.origin}</span>
                      <ChevronRight size={14} color="#64748b" />
                      <span style={{ color: '#93c5fd' }}>{shp.destination}</span>
                    </div>
                  </td>
                  <td>
                    <strong>{shp.containersCount}</strong> <span style={{ color: '#64748b' }}>Containers</span>
                  </td>
                  <td>
                    {getStatusBadge(shp.status)}
                  </td>
                  <td style={{ fontWeight: 500 }}>
                    {shp.eta}
                  </td>
                  <td>
                    <span className="action-link" data-cy="shipment-action-link">
                      Inspect Details <ExternalLink size={12} style={{ display: 'inline', marginLeft: '2px' }} />
                    </span>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
};
