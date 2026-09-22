# Corridor PostgreSQL Documentation

This directory encapsulates schema documentation, ERDs, and migration standards for the `smart_corridor` database.

## PostGIS Initialization
The database requires `postgis` to be active. `CorridorDbContext` initializes this dependency programmatically (`modelBuilder.HasPostgresExtension("postgis");`). 
During CI/CD schema deployments, ensure the base Postgres image supports PostGIS 3.0+.

## Entity Framework Bindings
- All coordinates must use standard SRID `4326` (WGS 84).
- Point locations utilize `geography` types mapping accurately over Earth's curvature.
- Polygons and LineStrings utilize `geometry` types configured to 4326.

## Execution
Run `dotnet ef migrations add InitialCorridorSchema` manually from a host containing the .NET 10 SDK, as the target execution environment may not possess full SDK binaries.
