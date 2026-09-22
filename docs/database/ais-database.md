# AIS / Vessel & Port Operations Database Architecture

## Overview
The `smart_ais` database utilizes **PostgreSQL** with the **PostGIS** extension to serve as the authoritative source for vessel lifecycle, port geometries, port-call assignments, and real-time operational boundaries across the Smart Trade & Logistics Corridor.

Simultaneously, `smart_ais_raw` operates in **MongoDB** to handle the unstructured, high-volume firehose of raw AIS provider messages. Finally, **Redis** caches latest known operational state for zero-latency queries.

## Boundary Enforcement
- **Fleet Boundary**: AIS tracks `Vessels`. It absolutely does not manage `Truck`, `Driver`, or `Vehicle` objects.
- **Corridor Boundary**: AIS manages the physical maritime tracking `Voyage`. The Corridor Trip master retains higher-order corridor coordination.
- **Customs & ECTN**: Identifiers link outward via `AisExternalReference`, but no customs tariffs or ECTN certificates exist inside the AIS boundary.

## PostGIS Integration
- Port definitions require PostGIS `geography(Point)` and `geometry(Geometry)`.
- Geofences evaluating vessel entry (`VesselPortEvent`) are natively backed by spatial evaluations natively tracked within Entity Framework via NetTopologySuite configurations.

## Multi-Tenancy & Security
- `TenantId` partitioning strictly prevents cross-tenant data leakage.
- Credentials for AIS data ingestion providers exist only in Vault (`secret/platform/ais/providers/*`), never in source or database strings.
