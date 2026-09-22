# GPS PostgreSQL Documentation

This directory tracks the migrations and EF Core PostgreSQL mappings for the `smart_gps` database.

## Architecture
- **PostGIS Extensions**: Enabled via `modelBuilder.HasPostgresExtension("postgis");`.
- **GiST Indexing**: Natively implemented on spatial properties (e.g. `Position` and `Geometry`).
- **Isolation**: Fleet entities are tracked purely via `VehicleReference` values. GPS is independent.

## Migrations
Due to the `.NET 10 SDK` constraint on the host, run `dotnet ef migrations add InitialGpsSchema` manually in an environment that has the SDK installed. The mappings defined in `GpsDbContext.cs` natively bind `NetTopologySuite` objects (`Point`, `Geometry`) to their PostgreSQL representations.
