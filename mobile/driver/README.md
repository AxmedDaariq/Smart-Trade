# Smart Trade Logistics — Enterprise Mobile Driver & Field Client

A high-performance, dark-themed, enterprise-grade mobile application designed for truck captains, fleet dispatchers, dryport yard agents, and customs border operators across the **Horn of Africa / Red Sea Trade Corridor** (Berbera Port ⇄ Tog-Wajaale ⇄ Dire Dawa ⇄ Mojo Dryport Hub).

---

## 🎨 Enterprise Design Language & Aesthetics

Engineered to match the **Smart Trade Control Center Web Portal**:
- **Palette**: Deep Obsidian Slate (`#090D16`), Elevated Surfaces (`#0F172A`), Glassmorphism Cards (`#151F32`), Electric Cobalt (`#2563EB` / `#3B82F6`), Cyan (`#38BDF8`), Emerald (`#10B981`), Amber (`#F59E0B`), and Crimson (`#EF4444`).
- **Typography & Components**: High-density typography, live pulsing status pills, subtle borders (`rgba(255,255,255,0.08)`), interactive gesture cards, and smooth navigation.

---

## 📱 Core Operational Modules & Tabs

### 1. Active Mission & Cabin Cockpit (`CockpitTab`)
- **Real-time Mission HUD**: Origin, Next Corridor Waypoint, Final Destination, and Consignment Waybill metadata.
- **Live Cabin Telemetry Matrix**:
  - GPS Velocity with corridor speed limit adherence.
  - Reefer Core Temperature with live cold chain tolerance monitoring (-18.2°C).
  - Cryptographic IoT Electronic Seal (E-Seal) lock status.
  - Galileo 5G satellite fix with sub-4m precision.
- **Corridor Milestone Timeline**: Step-by-step progress from Berbera Port Gate Out to Mojo Discharge.
- **Safety Triggers**: "Report Transit Delay" broadcast and "Corridor Emergency SOS" dispatch.

### 2. Shipments & Container Manifest Directory (`ManifestTab`)
- **Fast Search & Filter**: Filter by Waybill number, Consignee name, destination terminal, or cargo type.
- **Dynamic Filter Chips**: `All`, `In-Transit`, `Customs Cleared`, `Border Gate`.
- **Interactive Waybill Inspection (`ShipmentDetailSheet`)**:
  - Full container manifest (e.g. `14x TEU ISO Containers`).
  - ASYCUDA / WCO customs green release reference.
  - Port demurrage countdown clock & dwell time liability.
  - Cryptographic e-Waybill PDF export simulator.

### 3. Corridor Command & Telemetry Tower (`ControlTowerTab`)
- **Corridor Flow Radar**: Live satellite feed status, 38 active convoys, and 124 TEU/hour corridor throughput.
- **Gateway Clearance Status**: Real-time monitoring of Berbera Container Terminal (DP World), Tog-Wajaale Border Gate B, and Mojo Multimodal Yard.
- **Incident Feed**: Real-time push stream for temperature threshold warnings, queue congestion, and escort clearance.

### 4. Electronic Proof of Delivery (`EpodTab`)
- **Digital Handover Checklist**: Mandatory checks for seal integrity, cargo shell damage, cold-chain temperature, and customs stamps.
- **Photo Evidence Capture**: Camera attachment simulator with GPS timestamp watermarking.
- **Interactive Signature Pad**: High-precision custom signature canvas for driver and receiving agent.
- **SHA-256 Token Certification**: Generates cryptographically verifiable e-POD receipt.

### 5. Driver Profile & Offline Sync Engine (`SyncProfileTab`)
- **Driver Identity**: Commercial Heavy Rigid Class 4 credential, vehicle assignment (Volvo FH16 Globetrotter), and multi-tenant organization context.
- **Hours of Service (ELD Compliance)**: Visual driving clock remaining gauge with shift limits.
- **Offline Synchronization Engine**: Event queue manager with offline/online cellular drop simulation and manual flush capability.

---

## 🏗️ Architecture & Directory Layout

```
mobile/driver/lib/
├── main.dart                          # App Entrypoint & Theme bootstrap
├── theme/
│   ├── app_colors.dart                # Obsidian slate & enterprise neon tokens
│   └── app_theme.dart                 # Material 3 dark theme specifications
├── models/
│   └── logistics_models.dart          # Shipments, telemetry, waypoints, driver models
├── services/
│   └── mock_data_service.dart         # Horn of Africa corridor mock dataset
├── widgets/
│   ├── status_badge.dart              # Status pill badge with live glow dot
│   ├── metric_card.dart               # Telemetry and KPI metric card
│   ├── glass_card.dart                # Frosted surface card container
│   ├── corridor_timeline.dart         # Milestone transit progression tracker
│   └── signature_pad.dart             # Interactive stylus/finger signature canvas
├── screens/
│   ├── home_dashboard_screen.dart     # Main scaffold with header & bottom navigation
│   ├── shipment_detail_sheet.dart     # Comprehensive Waybill inspection modal
│   └── tabs/
│       ├── cockpit_tab.dart           # Active Mission & Telemetry Cockpit
│       ├── manifest_tab.dart          # Shipments & Waybill directory
│       ├── control_tower_tab.dart     # Corridor Gateways & Live IoT Alerts
│       ├── epod_tab.dart              # Electronic Proof of Delivery & Handover
│       └── sync_profile_tab.dart      # Offline Sync Engine & Driver Profile
└── core/
    ├── networking/api_client.dart     # Dio client with JWT & Idempotency interceptor
    └── synchronization/offline_queue.dart # Offline SQLite/local event queue engine
```

---

## 🚀 Running the Mobile App

Ensure Flutter is installed on your path:
```bash
cd mobile/driver
flutter pub get
flutter run
```
