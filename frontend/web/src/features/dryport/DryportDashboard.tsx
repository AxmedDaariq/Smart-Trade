import React, { useState, useEffect } from 'react';
import './DryportDashboard.css';
import { AddContainerModal } from './AddContainerModal';

interface ContainerDemurrage {
  containerId: string;
  containerNumber: string;
  status: string;
  daysInPort: number;
  freeDays: number;
  accruedDemurrage: number;
  currency: string;
}

export const DryportDashboard: React.FC = () => {
  const [containers, setContainers] = useState<ContainerDemurrage[]>([]);
  const [loading, setLoading] = useState<boolean>(true);
  const [isModalOpen, setIsModalOpen] = useState(false);

  useEffect(() => {
    const fetchDemurrageData = async () => {
      try {
        // Demo Data
        const demoData: ContainerDemurrage[] = [
          { containerId: '1', containerNumber: 'MSCU1234567', status: 'InStorage', daysInPort: 12, freeDays: 7, accruedDemurrage: 250.00, currency: 'USD' },
          { containerId: '2', containerNumber: 'CMAU9876543', status: 'InStorage', daysInPort: 6, freeDays: 7, accruedDemurrage: 0, currency: 'USD' },
          { containerId: '3', containerNumber: 'MAEU4445556', status: 'Arrived', daysInPort: 8, freeDays: 7, accruedDemurrage: 50.00, currency: 'USD' }
        ];
        
        setContainers(demoData);
        setLoading(false);
      } catch (error) {
        console.error("Waxaa dhacday cilad:", error);
        setLoading(false);
      }
    };

    fetchDemurrageData();
  }, []);

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

  const handleContainerSubmit = (data: any) => {
    console.log("Kunteenar cusub ayaa la diiwaangeliyay:", data);
    // Add logic here to POST to backend
  };

  if (loading) return <div className="dryport-container"><h2>Xogta waa la soo duba-ridayaa... (Loading...)</h2></div>;

  return (
    <div className="dryport-container">
      <div className="dashboard-header">
        <h1 className="dashboard-title">Dryport Demurrage Dashboard</h1>
        <button onClick={() => setIsModalOpen(true)} className="btn-primary" style={{ padding: '0.75rem 1.5rem', background: '#0f172a', color: 'white', border: 'none', borderRadius: '8px', cursor: 'pointer' }}>
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
