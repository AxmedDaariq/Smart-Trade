# Platform Threat Model

## 1. Threat Actors
- **External Attacker**: Attempts API enumeration, credential stuffing, and DDoS against the API Gateway.
- **Malicious Tenant (Logistics/Trader)**: Attempts IDOR/BOLA to access competing tenant's cargo data, customs holds, or GPS telemetry.
- **Compromised GPS/ANPR Device**: Attempts to spoof location data or fabricate gate authorizations to bypass customs checks.
- **Insider Threat**: Unauthorized attempts by internal users (e.g., Driver) to elevate privileges to Logistics Admin or Customs Officer.

## 2. Threat Categories (STRIDE)
- **Spoofing**: Faking ANPR events to clear a border. *Mitigation*: mTLS and cryptographic device credentials at the ingestion API.
- **Tampering**: Altering Customs Assessment rates (e.g., from 15% to 10%) via intercepted React payload. *Mitigation*: Backend calculates authoritative values from read-only Government Configuration; ignores client-supplied duty rates.
- **Repudiation**: Driver denying receipt of goods. *Mitigation*: Immutable Audit trails and EF Core Outbox pattern ensuring DeliveryConfirmed events are durable.
- **Information Disclosure**: Tenant A reading Tenant B's SignalR feed. *Mitigation*: Strict SignalR group isolation in `ControlTowerHub.cs`.
- **Denial of Service**: High-frequency GPS flooding. *Mitigation*: YARP Rate Limiting and Payload Size constraints.
- **Elevation of Privilege**: Driver attempting to hit `/api/v1/government/tariff`. *Mitigation*: Strict JWT Role and Permission claims validated at the Domain Controller level.
