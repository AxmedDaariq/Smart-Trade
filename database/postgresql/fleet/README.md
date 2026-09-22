# Fleet Database Schema & Configurations

This directory manages the PostgreSQL schema definitions and migrations for the `smart_fleet` database.

## Principles
- **Authoritative Assets Only**: Fleet creates, owns, and manages Vehicles, Trailers, and Drivers. It explicitly does **not** manage external domains (GPS pings, Customs tariffs, Payments). 
- **Assignment Concurrency**: `VehicleAssignment`, `DriverAssignment`, and `TrailerAssignment` are configured with optimistic concurrency and indexing (`StartAtUtc`, `EndAtUtc`, `Status`) to prevent double-booking physical assets.
- **Dispatch Readiness**: Dispatches are blocked by `FleetHold` active records. A Dispatch transitions to `Ready` only after all its `DispatchRequirement` entities (including external Customs Release flags sent via event) are satisfied.

## Migration Structure
Entity Framework Migrations belong to `/src/services/Fleet/Fleet.Infrastructure`. 
- No raw SQL logic is permitted to bypass EF tracking unless specifically annotated for performance index tuning.

## Backups
Standard backup rotation must apply here. Operations heavily depend on the continuity of driver credentials, active compliance metrics, and dispatch orders.
