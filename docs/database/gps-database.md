# GPS & Telematics Database Architecture

## Overview
The `smart_gps` database utilizes **PostgreSQL** with the **PostGIS** extension to serve as the authoritative source for geospatial locations, geofencing, and vehicle tracking across the Smart Trade & Logistics Corridor. High-volume, flexible telemetry logs fall under the `smart_telemetry` **MongoDB** domain, while **Redis** powers real-time "latest location" queries for the Control Tower.

## PostGIS Architecture
- **SRID**: `4326` (WGS 84 coordinate system)
- **Data Types**: `geography(Point,4326)` for pure location points, `geometry(Geometry,4326)` for geofence boundaries to ensure accurate spherical math and containment validation.
- **Indexes**: `GiST` indexes are mapped across `VehicleLocation`, `GpsPosition`, `Geofence`, `GeofenceEvent`, and `TrackingAlert` allowing efficient `ST_Contains` and `ST_DWithin` spatial operations natively in the database.

## Boundary Enforcement
- **No Domain Leakage**: This database records locations for a `VehicleReference` string. It absolutely does **not** duplicate the Fleet master tables (Vehicle, Driver, Truck).
- **Corridor**: GPS publishes `RouteDeviationDetected` and `GeofenceEntered` events, but Corridor remains authoritative for the Trip Master. GPS tracks *what physically happened*.

## Commands & Telemetry
- Commands issued to IoT Gateways and Devices (`DeviceCommand`) are time-limited, JSONB payload structured, and emit a resulting `DeviceCommandResult`.
- Operational Telemetry (Odometer, Engine, Fuel) mappings exist for robust SQL query access when required.
