import React, { useState } from 'react';
import {
  ShieldAlert,
  FileCheck,
  CheckCircle2,
  AlertTriangle,
  BadgePercent,
  Search,
  SlidersHorizontal,
  Eye,
  FileSpreadsheet
} from 'lucide-react';
import './CustomsDashboard.css';
import { useCustomsCases } from './hooks/useCustomsCases';

export const CustomsDashboard: React.FC = () => {
  const {
    data: cases = [],
    isLoading,
    isError,
    error
  } = useCustomsCases();

  const [searchTerm, setSearchTerm] = useState('');

  const handleQuickRelease = (caseNumber: string) => {
    alert(`Quick release triggered for ${caseNumber}. Implementation requires a specific API endpoint.`);
  };

  const filteredCases = cases.filter(c =>
    c.caseNumber.toLowerCase().includes(searchTerm.toLowerCase()) ||
    c.declarationNumber.toLowerCase().includes(searchTerm.toLowerCase()) ||
    c.declarant.toLowerCase().includes(searchTerm.toLowerCase())
  );

  if (isLoading) return <div className="customs-page"><h2 style={{ textAlign: 'center', padding: '2rem' }}>Loading Customs Data...</h2></div>;
  if (isError) return <div className="customs-page"><h2 style={{ textAlign: 'center', padding: '2rem', color: '#ef4444' }}>Error: {error instanceof Error ? error.message : 'Unknown Error'}</h2></div>;

  return (
    <div className="customs-page animate-fade-in">
      {/* Header */}
      <div className="customs-hero-header">
        <div>
          <h1 className="customs-hero-title">
            <ShieldAlert size={32} color="#10b981" />
            Customs Authority & Border Clearance
          </h1>
          <p className="customs-hero-desc">
            WCO SAFE-compliant automated risk assessment, tariff schedule verification, and ECTN clearance.
          </p>
        </div>

        <div style={{ display: 'flex', gap: '0.75rem' }}>
          <button type="button" className="btn-secondary">
            <FileSpreadsheet size={16} /> Tariff Schedule
          </button>
          <button type="button" className="btn-primary" style={{ background: 'linear-gradient(135deg, #059669 0%, #047857 100%)' }}>
            <FileCheck size={16} /> File Inward Declaration
          </button>
        </div>
      </div>

      {/* High-Level Revenue & Compliance KPIs */}
      <div className="kpi-grid">
        <div className="kpi-card kpi-success">
          <div className="kpi-header">
            <span className="kpi-title">Assessed Duties (Today)</span>
            <div className="kpi-icon-wrap" style={{ color: '#34d399' }}><BadgePercent size={20} /></div>
          </div>
          <div className="kpi-value">$284,500 <span style={{ fontSize: '0.875rem', color: '#94a3b8' }}>USD</span></div>
          <div className="kpi-footer">
            <span className="kpi-trend-positive">↑ +8.5%</span> Automated electronic collection
          </div>
        </div>

        <div className="kpi-card">
          <div className="kpi-header">
            <span className="kpi-title">Avg Clearance Turnaround</span>
            <div className="kpi-icon-wrap"><CheckCircle2 size={20} /></div>
          </div>
          <div className="kpi-value">1.4 <span style={{ fontSize: '1rem', color: '#94a3b8' }}>Hours</span></div>
          <div className="kpi-footer">
            <span className="kpi-trend-positive">Optimal</span> Down from 48h traditional paper
          </div>
        </div>

        <div className="kpi-card kpi-warning">
          <div className="kpi-header">
            <span className="kpi-title">Physical Audits Required</span>
            <div className="kpi-icon-wrap"><AlertTriangle size={20} color="#f59e0b" /></div>
          </div>
          <div className="kpi-value">7 <span style={{ fontSize: '1rem', color: '#f59e0b' }}>Cases</span></div>
          <div className="kpi-footer">
            <span className="kpi-trend-negative">Red Channel</span> Non-intrusive container scan scheduled
          </div>
        </div>

        <div className="kpi-card kpi-purple">
          <div className="kpi-header">
            <span className="kpi-title">Audit Compliance Index</span>
            <div className="kpi-icon-wrap"><FileCheck size={20} color="#a855f7" /></div>
          </div>
          <div className="kpi-value">99.2%</div>
          <div className="kpi-footer">
            Zero discrepancy rate on electronic seals
          </div>
        </div>
      </div>

      {/* WCO Risk Channels Matrix */}
      <div className="risk-channels-panel">
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <div>
            <h2 style={{ fontSize: '1.125rem', fontWeight: 700, color: '#ffffff' }}>
              WCO Risk Classification Pipeline
            </h2>
            <p style={{ fontSize: '0.8125rem', color: '#94a3b8', marginTop: '0.2rem' }}>
              Real-time automated routing of inward manifests based on compliance profile.
            </p>
          </div>
          <span className="badge badge-info">AI Risk Engine Active</span>
        </div>

        <div className="channels-grid">
          <div className="channel-card channel-green">
            <div>
              <div className="channel-name" style={{ color: '#34d399' }}>Green Lane (Fast Release)</div>
              <div className="channel-desc">Instant clearance with electronic duty payment</div>
            </div>
            <div className="channel-count">78%</div>
          </div>

          <div className="channel-card channel-yellow">
            <div>
              <div className="channel-name" style={{ color: '#fbbf24' }}>Yellow Lane (Documentary)</div>
              <div className="channel-desc">Invoice, B/L and Certificate of Origin check</div>
            </div>
            <div className="channel-count">16%</div>
          </div>

          <div className="channel-card channel-red">
            <div>
              <div className="channel-name" style={{ color: '#f87171' }}>Red Lane (Physical / X-Ray)</div>
              <div className="channel-desc">Gantry scanner inspection & physical seal audit</div>
            </div>
            <div className="channel-count">6%</div>
          </div>
        </div>
      </div>

      {/* Customs Declaration Cases Queue */}
      <div className="data-panel">
        <div className="panel-toolbar">
          <h2 className="panel-title">Active Customs Case Queue</h2>

          <div style={{ display: 'flex', gap: '0.75rem', alignItems: 'center' }}>
            <div style={{ position: 'relative' }}>
              <input
                type="text"
                placeholder="Search case, declaration, broker..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                style={{ padding: '0.4rem 0.8rem 0.4rem 2rem', fontSize: '0.8125rem', width: '240px' }}
              />
              <Search size={14} style={{ position: 'absolute', left: '0.65rem', top: '50%', transform: 'translateY(-50%)', color: '#64748b' }} />
            </div>

            <button type="button" className="btn-secondary" style={{ padding: '0.4rem 0.8rem', fontSize: '0.8125rem' }}>
              <SlidersHorizontal size={14} /> Filter
            </button>
          </div>
        </div>

        <div className="table-wrapper">
          <table className="enterprise-table">
            <thead>
              <tr>
                <th>Case Tracking #</th>
                <th>Declaration ID</th>
                <th>Declarant / Broker</th>
                <th>Procedure</th>
                <th>CIF Value (USD)</th>
                <th>Duty Assessed</th>
                <th>Risk Channel</th>
                <th>Status</th>
                <th>Action</th>
              </tr>
            </thead>
            <tbody>
              {filteredCases.map((c) => (
                <tr key={c.caseNumber}>
                  <td>
                    <span className="tracking-num" style={{ color: '#34d399' }}>{c.caseNumber}</span>
                  </td>
                  <td style={{ fontWeight: 600 }}>{c.declarationNumber}</td>
                  <td style={{ fontWeight: 600 }}>{c.declarant}</td>
                  <td>
                    <span style={{ fontSize: '0.8125rem', color: '#94a3b8' }}>{c.procedureCode}</span>
                  </td>
                  <td style={{ fontWeight: 600 }}>${c.cifValue.toLocaleString()}</td>
                  <td style={{ fontWeight: 600, color: c.assessedDuty > 0 ? '#38bdf8' : '#94a3b8' }}>
                    ${c.assessedDuty.toLocaleString()}
                  </td>
                  <td>
                    <span className={`lane-badge lane-badge-${c.channel.toLowerCase()}`}>
                      {c.channel} Lane
                    </span>
                  </td>
                  <td>
                    {c.status === 'Cleared' ? (
                      <span className="badge badge-success">Cleared</span>
                    ) : c.status === 'Under Inspection' ? (
                      <span className="badge badge-danger">Inspection</span>
                    ) : (
                      <span className="badge badge-warning">Valuation</span>
                    )}
                  </td>
                  <td>
                    <div style={{ display: 'flex', gap: '0.5rem', alignItems: 'center' }}>
                      <button
                        type="button"
                        className="btn-secondary"
                        style={{ padding: '0.25rem 0.5rem', fontSize: '0.75rem' }}
                        title="View Dossier"
                      >
                        <Eye size={12} />
                      </button>
                      {c.status !== 'Cleared' && (
                        <button
                          type="button"
                          className="btn-primary"
                          style={{ padding: '0.25rem 0.6rem', fontSize: '0.75rem', background: '#059669' }}
                          onClick={() => handleQuickRelease(c.caseNumber)}
                        >
                          Release
                        </button>
                      )}
                    </div>
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
