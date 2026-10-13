import React from 'react';
import { NavLink, Outlet, useNavigate } from 'react-router-dom';
import { useAuth } from '../../app/auth/AuthContext';
import {
  Globe2,
  LayoutDashboard,
  ShieldAlert,
  Radio,
  Layers,
  LogOut,
  Bell,
  Search,
  CheckCircle,
  Truck
} from 'lucide-react';
import './AppLayout.css';

export const AppLayout: React.FC = () => {
  const { session, logout } = useAuth();
  const navigate = useNavigate();

  const handleSignOut = async () => {
    await logout();
    navigate('/login', { replace: true });
  };

  const getInitials = () => {
    if (!session?.tenantCode) return 'ST';
    return session.tenantCode.substring(0, 2).toUpperCase();
  };

  return (
    <div className="app-layout">
      {/* Sidebar */}
      <aside className="app-sidebar" aria-label="Main Navigation">
        <div className="sidebar-header">
          <div className="sidebar-logo">
            <Globe2 size={22} />
          </div>
          <div>
            <div className="sidebar-brand-name">Smart Trade</div>
            <div className="sidebar-brand-sub">Control Center</div>
          </div>
        </div>

        {/* Current Tenant Badge */}
        <div className="sidebar-tenant-badge">
          <div className="tenant-meta">
            <span className="tenant-label">Current Organization</span>
            <span className="tenant-code">{session?.tenantCode || 'TENANT-GLB'}</span>
          </div>
          <span className="badge badge-success" style={{ fontSize: '0.65rem' }}>Active</span>
        </div>

        {/* Navigation Sections */}
        <nav className="sidebar-nav">
          <div className="nav-section-title">Portals & Workspaces</div>

          <NavLink 
            to="/logistics/dashboard" 
            className={({ isActive }) => `nav-item ${isActive ? 'active' : ''}`}
          >
            <LayoutDashboard size={18} />
            <span>Logistics Portal</span>
            <span className="nav-badge">Live</span>
          </NavLink>

          <NavLink 
            to="/government/dashboard" 
            className={({ isActive }) => `nav-item ${isActive ? 'active' : ''}`}
          >
            <ShieldAlert size={18} />
            <span>Customs & Port Auth</span>
            <span className="nav-badge" style={{ backgroundColor: 'rgba(16, 185, 129, 0.15)', color: '#34d399' }}>WCO</span>
          </NavLink>

          <NavLink 
            to="/control-tower" 
            className={({ isActive }) => `nav-item ${isActive ? 'active' : ''}`}
          >
            <Radio size={18} />
            <span>Corridor Control Tower</span>
            <span className="nav-badge" style={{ backgroundColor: 'rgba(239, 68, 68, 0.15)', color: '#f87171' }}>IoT</span>
          </NavLink>

          <NavLink 
            to="/dryport/dashboard" 
            className={({ isActive }) => `nav-item ${isActive ? 'active' : ''}`}
          >
            <Layers size={18} />
            <span>Dryport Demurrage</span>
          </NavLink>

          <div className="nav-section-title" style={{ marginTop: '1rem' }}>Fleet & Transit</div>
          <div className="nav-item" style={{ cursor: 'pointer', opacity: 0.85 }}>
            <Truck size={18} />
            <span>GPS Fleet Telemetry</span>
            <span className="nav-badge" style={{ backgroundColor: 'rgba(168, 85, 247, 0.2)', color: '#c084fc' }}>18 Active</span>
          </div>
        </nav>

        {/* User Profile & Logout */}
        <div className="sidebar-footer">
          <div className="user-profile">
            <div className="user-avatar">{getInitials()}</div>
            <div className="user-info">
              <span className="user-name">{session?.tenantCode ? `User (${session.tenantCode})` : 'Enterprise User'}</span>
              <span className="user-role">{session?.roles[0] || 'Logistics Admin'}</span>
            </div>
          </div>
          <button 
            type="button" 
            className="logout-btn" 
            title="Sign out" 
            onClick={handleSignOut}
            aria-label="Sign out"
          >
            <LogOut size={18} />
          </button>
        </div>
      </aside>

      {/* Main App Canvas */}
      <div className="app-main-wrapper">
        {/* Top Header */}
        <header className="app-header">
          <div className="header-search">
            <Search size={16} color="#64748b" />
            <input 
              type="text" 
              placeholder="Search B/L, Container ID, Customs Case, Truck..." 
              aria-label="Global Search"
            />
            <span className="search-shortcut">⌘K</span>
          </div>

          <div className="header-actions">
            <div className="corridor-selector">
              <CheckCircle size={14} />
              <span>Berbera — Addis Ababa Gateway: OPTIMAL</span>
            </div>

            <button type="button" className="icon-btn" aria-label="Notifications">
              <Bell size={18} />
              <span className="icon-badge-dot" />
            </button>
          </div>
        </header>

        {/* Page Content */}
        <main className="page-container animate-fade-in">
          <Outlet />
        </main>
      </div>
    </div>
  );
};
