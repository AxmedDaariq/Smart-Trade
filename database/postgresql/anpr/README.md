# ANPR / Gate / Border Operations PostgreSQL Documentation

This directory encapsulates schema documentation, ERDs, and migration standards for the `smart_anpr` database.

## Critical Security Principle
The primary architectural design rule of this database is:
**ANPR RECOGNITION IS EVIDENCE. ANPR RECOGNITION IS NOT AUTHORIZATION.**

Gates never open directly due to an `AnprRecognitionEvent`. They open exclusively via a `GateAuthorizationDecision`.

## PostGIS Initialization
The database requires `postgis` to be active. `AnprDbContext` initializes this dependency programmatically (`modelBuilder.HasPostgresExtension("postgis");`). 

## Spatial Configurations
- All coordinates must use standard SRID `4326` (WGS 84).
- Point locations utilize `geography` types mapping accurately over Earth's curvature.

## Execution
Run `dotnet ef migrations add InitialAnprSchema` manually from a host containing the .NET 10 SDK, as the target execution environment may not possess full SDK binaries.
