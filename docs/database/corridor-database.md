# Corridor / Trip / Route Management Database Architecture

## Overview
The `smart_corridor` database utilizes **PostgreSQL** coupled with **PostGIS** to provide highly accurate spatial operations across operational trade routes, enforcing strict milestone and checkpoint adherence across logistics corridors without duplicating the responsibilities of Fleet, GPS, or Customs databases.

## Spatial (PostGIS) Integration
- **Routes/Segments**: Managed via `geometry(LineString, 4326)` representing exact road paths.
- **Checkpoints/Milestones**: Managed via `geography(Point, 4326)` and `geometry(Polygon, 4326)` to allow exact proximity evaluations (e.g. `ST_DWithin` checks for checkpoint arrivals).
- **Spatial Indexing**: GiST indexes systematically applied to all geospatial columns.

## Boundary Enforcement
- GPS is responsible for emitting real-time telemetry coordinates. Corridor does *not* query GPS continuously; it consumes events to calculate `RouteDeviation`.
- Fleet retains all authority over the Truck, Driver, and Vehicle parameters. Corridor only captures references ensuring no out-of-sync master data.

## Immutable Data Strategies
- **ETA**: `TripEtaRecord` is an append-only time-series history log enabling BI tracking of corridor friction.
- **Versioning**: `RouteVersion`, `CorridorVersion`, and `TripVersion` prevent active operations from suffering silent mutations if administrators update reference geometry mid-transit.

## Caching (Redis)
- Uses key-stores (e.g. `corridor:trip:{tripId}:state`) strictly for read-model scaling, ensuring `smart_corridor` operates primarily as the authoritative operational datastore, decoupled from frontend querying loads.
