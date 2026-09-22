# MongoDB Standards

## Use Cases
Data that benefits from flexible schemas, high-volume ingestion, semi-structured provider payloads, raw telemetry, AIS messages.

## Avoid For
- Customs financial assessment source of truth
- Payment ledger
- Core identity

## Document Standards
- `_id`: String (UUID)
- `tenantId`
- `schemaVersion`
- `eventType`
- `source`
- `correlationId`
- `causationId`
- `occurredAtUtc`
- `receivedAtUtc`

## Indexing
- Based on query patterns.
- High-volume collections must consider: `tenantId`, `occurredAtUtc`, `receivedAtUtc`, `eventType`, `source`, `deviceId`.
- Use TTL index strategy where retention is appropriate, but never for legally retained records.
