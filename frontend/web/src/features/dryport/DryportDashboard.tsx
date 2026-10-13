import React, { useState } from 'react';
import './DryportDashboard.css';
import { AddContainerModal } from './AddContainerModal';
import { useDryportDemurrage } from './hooks/useDryportDemurrage';

export const DryportDashboard: React.FC = () => {
  const {
    containers,
    isLoading,
    isError,
    error,
    addContainer,
    isAdding
  } = useDryportDemurrage();

  const [isModalOpen, setIsModalOpen] = useState(false);

  const totalContainers = containers.length;
  const criticalContainers = containers.filter(c => c.accruedDemurrage > 0).length;
  const warningContainers = containers.filter(c => c.daysInPort === c.freeDays || c.daysInPort === c.freeDays - 1).length;
  const totalAccrued = containers.reduce((sum, current) => sum + current.accruedDemurrage, 0);

  const getBadgeClass = (daysInPort: number, freeDays: number) => {
    if (daysInPort > freeDays) return 'badge badge-critical';
    if (daysInPort >= freeDays - 1) return 'badge badge-warning';
    return 'badge badge-safe';
  };

  const getStatusText = (daysInPort: number, freeDays: number) => {
    if (daysInPort > freeDays) return 'Demurrage (Waaseegay)';
    if (daysInPort >= freeDays - 1) return 'Warning (Wuu dhow yahay)';
    return 'Safe (Waqti baa haray)';
  };

  const handleContainerSubmit = async (data: any) => {
    try {
      await addContainer(data);
      console.log("Kunteenar cusub ayaa la diiwaangeliyay");
    } catch (err) {
      console.error("Cilad ayaa dhacday marka la kaydinayay kunteenarka:", err);
      alert("Cilad ayaa dhacday: Kunteenarka lama kaydin");
    }
  };

  if (isLoading) return <div className="dryport-container"><h2 style={{ textAlign: 'center', padding: '2rem' }}>Xogta waa la soo duba-ridayaa... (Loading...)</h2></div>;

  if (isError) return <div className="dryport-container"><h2 style={{ textAlign: 'center', padding: '2rem', color: '#ef4444' }}>Cilad ayaa dhacday: {error instanceof Error ? error.message : 'Unknown Error'}</h2></div>;

  return (
    <div className="dryport-container">
      <div className="dashboard-header">
        <h1 className="dashboard-title">Dryport Demurrage & Yard Dashboard</h1>
        <button onClick={() => setIsModalOpen(true)} className="btn-primary">
          + Diiwaangeli Kunteenar
        </button>
      </div>

      <div className="stats-grid">
        <div className="stat-card">
          <div className="stat-title">Kunteenarada Dekedda Ku Jira</div>
          <div className="stat-value">{totalContainers}</div>
        </div>
        <div className="stat-card warning">
          <div className="stat-title">Kunteenarada Digniinta Ku Jira</div>
          <div className="stat-value">{warningContainers}</div>
        </div>
        <div className="stat-card critical">
          <div className="stat-title">Kuwa Galay Demurrage</div>
          <div className="stat-value">{criticalContainers}</div>
        </div>
        <div className="stat-card">
          <div className="stat-title">Lacagta Demurrage ee Uruurtay</div>
          <div className="stat-value" style={{ color: '#ef4444' }}>
            ${totalAccrued.toLocaleString()}
          </div>
        </div>
      </div>

      <div className="table-container">
        <table className="modern-table">
          <thead>
            <tr>
              <th>Numbarka Kunteenarka</th>
              <th>Xaaladda (Status)</th>
              <th>Maalmaha Dekedda</th>
              <th>Maalmaha Bilaashka</th>
              <th>Lacagta Demurrage</th>
              <th>Heerka Khatarta</th>
            </tr>
          </thead>
          <tbody>
            {containers.map((container) => (
              <tr key={container.containerId}>
                <td style={{ fontWeight: 600 }}>{container.containerNumber}</td>
                <td>{container.status}</td>
                <td>{container.daysInPort} Maalmood</td>
                <td>{container.freeDays} Maalmood</td>
                <td style={{ fontWeight: 600, color: container.accruedDemurrage > 0 ? '#ef4444' : '#334155' }}>
                  {container.currency} {container.accruedDemurrage.toLocaleString()}
                </td>
                <td>
                  <span className={getBadgeClass(container.daysInPort, container.freeDays)}>
                    {getStatusText(container.daysInPort, container.freeDays)}
                  </span>
                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>

      <AddContainerModal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        onSubmit={handleContainerSubmit}
      />
    </div>
  );
};
