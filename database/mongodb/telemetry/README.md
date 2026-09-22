# Telemetry MongoDB Documentation

This directory tracks the retention and schemas for the `smart_telemetry` database.

## Architecture
- **Purpose**: A high-volume, flexible datastore capable of capturing diverse IoT sensors (Odometer, Fuel, Engine, Temperature).
- **Indexing**: Designed for time-series extraction. Ensure indexes are mapped by `tenantId`, `vehicleReference`, and `timestampUtc`.
- **Schema Mapping**: While EF Core structurally maps the strict SQL queryable properties, the firehose of unstructured GPS tracking payloads and IoT metrics land directly here.

## Security
- Store MongoDB connection strings strictly in Vault (`secret/platform/mongodb/telemetry`).
- Never expose telemetry databases to the public web directly.
