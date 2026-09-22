# AIS MongoDB Documentation

This directory tracks the retention and schemas for the `smart_ais_raw` database.

## Architecture
- **Purpose**: A high-volume, flexible datastore capable of capturing massive provider payloads (e.g. NMEA strings, JSON feeds, XML bursts).
- **Indexing**: Designed for time-series extraction and incident reprocessing.
- **Payload Security**: No raw payloads are executed.

## Security
- Store MongoDB connection strings strictly in Vault (`secret/platform/mongodb/ais`).
- Never expose telemetry databases to the public web directly.
