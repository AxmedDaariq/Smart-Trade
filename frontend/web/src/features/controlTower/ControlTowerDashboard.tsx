import React from 'react';
import {
  Radio,
  AlertOctagon,
  Anchor,
  Compass,
  Layers,
  ArrowUpRight,
  Activity
} from 'lucide-react';
import './ControlTowerDashboard.css';
import { useTowerTelemetry } from './hooks/useTowerTelemetry';

export const ControlTowerDashboard: React.FC = () => {
  const {
    kpis,
    transitTime,
    isLoading,
    isError,
    error
  } = useTowerTelemetry();

  const alerts: CorridorAlert[] = [
    {
      id: 'ALT-1',
      type: 'critical',
      title: 'Reefer Temp Threshold Alert',
      location: 'Container MSCU-9921 @ Wajaale',
      timestamp: '6 mins ago',
      description: 'Pharmaceutical shipment temperature increased to +8.2°C (limit: +4.0°C). Driver notified.'
    },
    {
      id: 'ALT-2',
      type: 'warning',
      title: 'Border Post Queue Congestion',
      location: 'Tog-Wajaale Crossing Gate B',
      timestamp: '24 mins ago',
      description: 'Wait times increased from 25 mins to 55 mins due to telecommunications line maintenance.'
    },
    {
      id: 'ALT-3',
      type: 'info',
      title: 'Vessel Berthing Confirmed',
      location: 'Berbera Container Terminal Quay 1',
      timestamp: '42 mins ago',
      description: 'MV Maersk Seletar (2,800 TEU) safely berthed. Discharge operations initiated.'
    }
  ];

  if (isLoading) return <div className="control-tower-page"><h2 style={{ textAlign: 'center', padding: '2rem' }}>Loading Telemetry Data...</h2></div>;
  if (isError) return <div className="control-tower-page"><h2 style={{ textAlign: 'center', padding: '2rem', color: '#ef4444' }}>Error: {error instanceof Error ? error.message : 'Unknown Error'}</h2></div>;

  return (
    <div className="control-tower-page animate-fade-in">
      {/* Header */}
      <div className="tower-hero-header">
        <div>
          <h1 className="tower-hero-title">
            <Radio size={32} color="#f87171" />
            Corridor Command & Control Tower
          </h1>
          <p style={{ fontSize: '0.875rem', color: '#94a3b8', marginTop: '0.35rem' }}>
            Multi-tier real-time IoT telemetry, vessel AIS tracking, and bottleneck prediction engine.
          </p>
        </div>

        <div style={{ display: 'flex', alignItems: 'center', gap: '0.75rem' }}>
          <div className="badge badge-success" style={{ padding: '0.5rem 1rem' }}>
            <span className="pulse-dot" style={{ backgroundColor: '#10b981' }} />
            Satellite Feed: Connected (4.2ms)
          </div>
        </div>
      </div>

      {/* Main Dual Grid */}
      <div className="tower-grid">
        {/* Left: Telemetry Matrix & Corridor Waypoints */}
        <div className="telemetry-map-panel">
          <div className="map-radar-ring" />

          <div className="map-telemetry-header">
            <div>
              <div style={{ fontSize: '1.125rem', fontWeight: 700, color: '#ffffff', display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
                <Activity size={18} color="#38bdf8" />
                Live Corridor Telemetry Grid
              </div>
              <div style={{ fontSize: '0.8125rem', color: '#94a3b8' }}>
                Berbera Port ⇄ Jigjiga ⇄ Dire Dawa ⇄ Mojo Hub
              </div>
            </div>

            <span className="badge badge-info">{kpis.length} Active Convoys</span>
          </div>

          {/* Central Map Mock Display */}
          <div style={{ margin: '3rem 0', textAlign: 'center', position: 'relative', zIndex: 2 }}>
            <div style={{ fontSize: '2.5rem', fontWeight: 800, color: '#f8fafc', letterSpacing: '-0.02em' }}>
              Horn of Africa Corridor Flow
            </div>
            <div style={{ color: '#38bdf8', fontSize: '1rem', fontWeight: 600, marginTop: '0.5rem' }}>
              Average Corridor Throughput: {transitTime ? (transitTime as any).value : '...'} TEU / Hour
            </div>
          </div>

          {/* Waypoints Telemetry Footer */}
          <div className="map-waypoints-list">
            <div className="waypoint-card">
              <div className="waypoint-title">
                <span>Berbera Port Quay</span>
                <Anchor size={14} color="#60a5fa" />
              </div>
              <div className="waypoint-stat">3 Vessels</div>
              <div style={{ fontSize: '0.71875rem', color: '#94a3b8', marginTop: '0.2rem' }}>Dwell time: 1.1 days</div>
            </div>

            <div className="waypoint-card">
              <div className="waypoint-title">
                <span>Wajaale Border</span>
                <Compass size={14} color="#34d399" />
              </div>
              <div className="waypoint-stat">98.2% Fast-Pass</div>
              <div style={{ fontSize: '0.71875rem', color: '#94a3b8', marginTop: '0.2rem' }}>Avg clearance: 18 min</div>
            </div>

            <div className="waypoint-card">
              <div className="waypoint-title">
                <span>Mojo Dryport</span>
                <Layers size={14} color="#a855f7" />
              </div>
              <div className="waypoint-stat">82% Capacity</div>
              <div style={{ fontSize: '0.71875rem', color: '#94a3b8' }}>1,210 Slots Available</div>
            </div>
          </div>
        </div>

        {/* Right: Real-Time Alerts Stream */}
        <div className="alerts-stream-panel">
          <div className="alerts-header">
            <h2 style={{ fontSize: '1.125rem', fontWeight: 700, color: '#ffffff', display: 'flex', alignItems: 'center', gap: '0.5rem' }}>
              <AlertOctagon size={18} color="#f87171" />
              Incident Stream
            </h2>
            <span className="badge badge-danger">3 Unresolved</span>
          </div>

          <div className="alerts-list">
            {alerts.map((alt) => (
              <div key={alt.id} className={`alert-item ${alt.type}`}>
                <div className="alert-title-row">
                  <span className="alert-title">{alt.title}</span>
                  <span className="alert-time">{alt.timestamp}</span>
                </div>
                <div style={{ fontSize: '0.71875rem', color: '#38bdf8', fontWeight: 600 }}>
                  {alt.location}
                </div>
                <p className="alert-desc">{alt.description}</p>
              </div>
            ))}
          </div>

          <div style={{ marginTop: '1.5rem', paddingTop: '1rem', borderTop: '1px solid rgba(255, 255, 255, 0.08)' }}>
            <button type="button" className="btn-secondary" style={{ width: '100%' }}>
              Configure IoT Sensor Thresholds <ArrowUpRight size={14} />
            </button>
          </div>
        </div>
      </div>
    </div>
  );
};

interface CorridorAlert {
  id: string;
  type: 'critical' | 'warning' | 'info';
  title: string;
  location: string;
  timestamp: string;
  description: string;
}
