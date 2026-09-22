# AIS PostgreSQL Documentation

This directory tracks the migrations and EF Core PostgreSQL mappings for the `smart_ais` database.

## Architecture
- **PostGIS Extensions**: Enabled via `modelBuilder.HasPostgresExtension("postgis");`.
- **GiST Indexing**: Natively implemented on spatial properties across Ports, Berths, Anchorage Areas, and Vessel Positions.
- **Optimistic Concurrency**: Enforced on all configuration elements utilizing `byte[] Version` mapped to RowVersion limits.

## Migrations
Run `dotnet ef migrations add InitialAisSchema` manually in an environment that has the .NET SDK installed. The mappings defined in `AisDbContext.cs` natively bind `NetTopologySuite` objects to their PostgreSQL representations.
