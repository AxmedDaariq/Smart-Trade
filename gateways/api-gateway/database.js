const { DatabaseSync } = require('node:sqlite');
const path = require('node:path');
const fs = require('node:fs');

const dbDir = path.resolve(__dirname, '../../database');
if (!fs.existsSync(dbDir)) {
  fs.mkdirSync(dbDir, { recursive: true });
}

const dbPath = path.join(dbDir, 'smart_trade.db');
const db = new DatabaseSync(dbPath);

// Initialize Tables
function initDatabase() {
  db.exec(`
    CREATE TABLE IF NOT EXISTS tenants (
      id TEXT PRIMARY KEY,
      tenant_code TEXT UNIQUE,
      name TEXT,
      tenant_type TEXT,
      status TEXT,
      created_at TEXT
    );

    CREATE TABLE IF NOT EXISTS users (
      id TEXT PRIMARY KEY,
      tenant_id TEXT,
      email TEXT UNIQUE,
      password_hash TEXT,
      full_name TEXT,
      roles TEXT,
      permissions TEXT,
      created_at TEXT
    );

    CREATE TABLE IF NOT EXISTS shipments (
      id TEXT PRIMARY KEY,
      tracking_number TEXT UNIQUE,
      tenant_id TEXT,
      consignee TEXT,
      origin TEXT,
      destination TEXT,
      container_count INTEGER,
      container_codes TEXT,
      cargo_type TEXT,
      gross_weight TEXT,
      status TEXT,
      eta TEXT,
      risk_level TEXT,
      e_seal_number TEXT,
      declaration_ref TEXT,
      demurrage_countdown TEXT,
      created_at TEXT,
      updated_at TEXT
    );

    CREATE TABLE IF NOT EXISTS tracking_events (
      id TEXT PRIMARY KEY,
      shipment_id TEXT,
      title TEXT,
      location TEXT,
      status TEXT,
      timestamp TEXT,
      note TEXT
    );

    CREATE TABLE IF NOT EXISTS customs_cases (
      id TEXT PRIMARY KEY,
      case_number TEXT UNIQUE,
      tenant_id TEXT,
      declaration_ref TEXT,
      declarant_name TEXT,
      status TEXT,
      channel TEXT,
      risk_score INTEGER,
      port_code TEXT,
      inspected_at TEXT,
      created_at TEXT
    );

    CREATE TABLE IF NOT EXISTS dryport_containers (
      id TEXT PRIMARY KEY,
      container_number TEXT UNIQUE,
      size_type TEXT,
      carrier TEXT,
      consignee TEXT,
      yard_location TEXT,
      status TEXT,
      arrived_at TEXT,
      free_days_limit INTEGER,
      daily_rate REAL,
      accrued_demurrage REAL
    );

    CREATE TABLE IF NOT EXISTS corridor_alerts (
      id TEXT PRIMARY KEY,
      title TEXT,
      location TEXT,
      timestamp TEXT,
      description TEXT,
      severity TEXT,
      icon_code TEXT,
      created_at TEXT
    );

    CREATE TABLE IF NOT EXISTS driver_telemetry (
      id TEXT PRIMARY KEY,
      driver_id TEXT,
      truck_plate TEXT,
      speed INTEGER,
      reefer_temp REAL,
      target_temp REAL,
      e_seal_secured INTEGER,
      gps_status TEXT,
      corridor TEXT,
      distance_remaining INTEGER,
      next_waypoint TEXT,
      eta_next_waypoint TEXT,
      recorded_at TEXT
    );
  `);

  // Seed default enterprise records if empty
  const tenantCount = db.prepare('SELECT count(*) as count FROM tenants').get().count;
  if (tenantCount === 0) {
    seedInitialData();
  }
}

function seedInitialData() {
  const now = new Date().toISOString();

  // 1. Tenants & Users
  db.prepare(`
    INSERT INTO tenants (id, tenant_code, name, tenant_type, status, created_at)
    VALUES (?, ?, ?, ?, ?, ?)
  `).run('t-01', 'TENANT-GLB', 'Smart Trade Global Logistics', 'LogisticsOperator', 'Active', now);

  db.prepare(`
    INSERT INTO tenants (id, tenant_code, name, tenant_type, status, created_at)
    VALUES (?, ?, ?, ?, ?, ?)
  `).run('t-02', 'TENANT-ST-EAST', 'Smart Trade East Africa Freight Consortium', 'Carrier', 'Active', now);

  db.prepare(`
    INSERT INTO users (id, tenant_id, email, password_hash, full_name, roles, permissions, created_at)
    VALUES (?, ?, ?, ?, ?, ?, ?, ?)
  `).run(
    'u-01',
    't-01',
    'dispatcher@smarttrade.com',
    'P@ssword123!',
    'Captain Daariq Axmed',
    JSON.stringify(['EnterpriseAdmin', 'LogisticsManager']),
    JSON.stringify(['control-tower.read', 'customs.inspect', 'fleet.manage', 'shipping.read', 'shipping.create']),
    now
  );

  // 2. Shipments
  const shipmentsData = [
    {
      id: 'SHP-9021',
      tracking_number: 'STL-BER-2026-081',
      tenant_id: 't-01',
      consignee: 'Ethio-Telecom Imports Ltd',
      origin: 'Berbera Container Terminal (Quay 2)',
      destination: 'Addis Ababa (Mojo Dryport Hub)',
      container_count: 14,
      container_codes: 'MSCU-992144, MSCU-992145 + 12 more',
      cargo_type: 'Fiber-Optic Routers & 5G Towers',
      gross_weight: '48,200 kg',
      status: 'In-Transit',
      eta: 'Today, 18:30 EAT',
      risk_level: 'Low',
      e_seal_number: 'SEAL-IOT-99214',
      declaration_ref: 'ASYCUDA-SOM-2026-9011',
      demurrage_countdown: '4 Days Remaining'
    },
    {
      id: 'SHP-9022',
      tracking_number: 'STL-BER-2026-082',
      tenant_id: 't-01',
      consignee: 'Midroc Agro Industries',
      origin: 'Berbera Port Quay 2',
      destination: 'Dire Dawa Terminal',
      container_count: 22,
      container_codes: 'TGHU-882190, TGHU-882191 + 20 more',
      cargo_type: 'Controlled Temp Fertilizer & Seeds',
      gross_weight: '62,500 kg',
      status: 'Customs Cleared',
      eta: 'Tomorrow, 09:00 EAT',
      risk_level: 'Low',
      e_seal_number: 'SEAL-IOT-77142',
      declaration_ref: 'ASYCUDA-SOM-2026-9045',
      demurrage_countdown: '6 Days Remaining'
    },
    {
      id: 'SHP-9023',
      tracking_number: 'STL-DJI-2026-104',
      tenant_id: 't-01',
      consignee: 'Hargeisa Central Health Supply',
      origin: 'Djibouti Doraleh Terminal',
      destination: 'Hargeisa Central Depot',
      container_count: 6,
      container_codes: 'HLXU-441201, HLXU-441202',
      cargo_type: 'Reefer Pharmaceuticals (-20°C Cold Chain)',
      gross_weight: '18,400 kg',
      status: 'In-Transit',
      eta: '14 Oct, 12:00 EAT',
      risk_level: 'Medium',
      e_seal_number: 'SEAL-IOT-55198',
      declaration_ref: 'ECTN-DJI-2026-0044',
      demurrage_countdown: '5 Days Remaining'
    },
    {
      id: 'SHP-9024',
      tracking_number: 'STL-BER-2026-089',
      tenant_id: 't-01',
      consignee: 'Awash Steel Infrastructure',
      origin: 'Berbera Container Terminal',
      destination: 'Wajaale Cross-Border Post',
      container_count: 30,
      container_codes: 'MSCU-119280, MSCU-119281 + 28 more',
      cargo_type: 'Structural Steel Beams (Overweight Permit)',
      gross_weight: '98,000 kg',
      status: 'Border Gate',
      eta: 'Under Border Audit',
      risk_level: 'Critical',
      e_seal_number: 'SEAL-IOT-10928',
      declaration_ref: 'ASYCUDA-SOM-2026-9118',
      demurrage_countdown: '1 Day Remaining'
    }
  ];

  const insertShipment = db.prepare(`
    INSERT INTO shipments (
      id, tracking_number, tenant_id, consignee, origin, destination,
      container_count, container_codes, cargo_type, gross_weight, status,
      eta, risk_level, e_seal_number, declaration_ref, demurrage_countdown,
      created_at, updated_at
    ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
  `);

  for (const s of shipmentsData) {
    insertShipment.run(
      s.id, s.tracking_number, s.tenant_id, s.consignee, s.origin, s.destination,
      s.container_count, s.container_codes, s.cargo_type, s.gross_weight, s.status,
      s.eta, s.risk_level, s.e_seal_number, s.declaration_ref, s.demurrage_countdown,
      now, now
    );
  }

  // 3. Tracking Events
  const insertTracking = db.prepare(`
    INSERT INTO tracking_events (id, shipment_id, title, location, status, timestamp, note)
    VALUES (?, ?, ?, ?, ?, ?, ?)
  `);

  insertTracking.run('evt-1', 'SHP-9021', 'Port Gate Out Inspection', 'Berbera Port Gate 4', 'completed', '06:15 EAT', 'ANPR verified, E-seal lock activated.');
  insertTracking.run('evt-2', 'SHP-9021', 'Hargeisa Highway Bypass', 'Hargeisa Outer Ring Road', 'completed', '09:40 EAT', 'Weighbridge axle balance within tolerance (44.2T).');
  insertTracking.run('evt-3', 'SHP-9021', 'Cross-Border Customs Inspection', 'Tog-Wajaale Border Gate Post B', 'active', 'Expected 14:15 EAT', 'Single Window green-channel clearance queue.');
  insertTracking.run('evt-4', 'SHP-9021', 'Dryport Inbound Discharge', 'Mojo Dryport Container Yard', 'pending', 'Expected 18:30 EAT', 'Final consignee e-POD digital signature handover.');

  // 4. Customs Cases
  const insertCustoms = db.prepare(`
    INSERT INTO customs_cases (
      id, case_number, tenant_id, declaration_ref, declarant_name,
      status, channel, risk_score, port_code, inspected_at, created_at
    ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
  `);

  insertCustoms.run('cs-1', 'CUS-2026-4401', 't-01', 'ASYCUDA-SOM-2026-9011', 'Ethio-Telecom Logistics', 'Cleared', 'Green', 12, 'SO-BBO', now, now);
  insertCustoms.run('cs-2', 'CUS-2026-4402', 't-01', 'ASYCUDA-SOM-2026-9045', 'Midroc Agro Clearing', 'Cleared', 'Green', 8, 'SO-BBO', now, now);
  insertCustoms.run('cs-3', 'CUS-2026-4403', 't-01', 'ASYCUDA-SOM-2026-9118', 'Awash Cross-Border Forwarding', 'Under Review', 'Red', 88, 'SO-TWJ', now, now);

  // 5. Dryport Containers
  const insertDryport = db.prepare(`
    INSERT INTO dryport_containers (
      id, container_number, size_type, carrier, consignee, yard_location,
      status, arrived_at, free_days_limit, daily_rate, accrued_demurrage
    ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
  `);

  insertDryport.run('dp-1', 'MSCU-992144', '40HC', 'MSC Mediterranean', 'Ethio-Telecom Imports', 'Bay A-12', 'InStorage', '2026-10-10', 7, 45.0, 0.0);
  insertDryport.run('dp-2', 'TGHU-882190', '20GP', 'Maersk Line', 'Midroc Agro', 'Bay B-04', 'InStorage', '2026-10-08', 7, 30.0, 60.0);
  insertDryport.run('dp-3', 'HLXU-441201', '40RF', 'Hapag-Lloyd', 'Hargeisa Health Central', 'Reefer Bay R-02', 'InStorage', '2026-10-11', 5, 80.0, 0.0);

  // 6. Corridor Alerts
  const insertAlert = db.prepare(`
    INSERT INTO corridor_alerts (id, title, location, timestamp, description, severity, icon_code, created_at)
    VALUES (?, ?, ?, ?, ?, ?, ?, ?)
  `);

  insertAlert.run('alt-1', 'Reefer Temp Threshold Warning', 'Container MSCU-9921 @ Wajaale', '6 mins ago', 'Pharmaceutical shipment temperature increased to -16.5°C (Target: -20.0°C). Compressor cycle accelerated.', 'warning', 'thermostat', now);
  insertAlert.run('alt-2', 'Border Post Queue Congestion', 'Tog-Wajaale Crossing Gate B', '24 mins ago', 'Wait times increased to 35 mins due to optical scanner routine calibration. Green-channel e-pass expedited.', 'warning', 'traffic', now);
  insertAlert.run('alt-3', 'Vessel Berthing & Discharge Live', 'Berbera Port Quay 1 (DP World Terminal)', '42 mins ago', 'MV Maersk Seletar (2,800 TEU) berthed safely. 140 dry containers discharged to yard staging.', 'info', 'anchor', now);

  // 7. Driver Telemetry
  db.prepare(`
    INSERT INTO driver_telemetry (
      id, driver_id, truck_plate, speed, reefer_temp, target_temp,
      e_seal_secured, gps_status, corridor, distance_remaining,
      next_waypoint, eta_next_waypoint, recorded_at
    ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
  `).run(
    'tel-01', 'STL-DRV-4092', 'SL-6421-B', 64, -18.2, -20.0,
    1, '5G High-Precision Galileo (3.8m)', 'Berbera ⇄ Dire Dawa ⇄ Mojo Hub',
    218, 'Tog-Wajaale Border Gate Post B', '1h 24m', now
  );
}

module.exports = {
  db,
  initDatabase,
};
