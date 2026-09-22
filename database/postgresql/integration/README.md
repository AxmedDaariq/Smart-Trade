# Integration & Interoperability PostgreSQL Documentation

This directory encapsulates schema documentation, ERDs, and migration standards for the `smart_integration` database.

## Critical Ownership Principle
The primary architectural design rule of this database is:
**INTEGRATION SERVICE STORES METADATA, ROUTING, AND STATUS. IT DOES NOT OWN THE DOMAIN.**

No `CustomsAssessment`, `Vehicle`, `PaymentLedger`, or `AnprRecognition` tables exist here. The database stores strictly `IntegrationExternalReference`, `IntegrationMessage`, and `WebhookSubscription` artifacts mapping these platforms together.

## Vault Integration
`IntegrationAuthentication` strictly stores `VaultSecretPath`. Never insert tokens directly into this database.

## Execution
Run `dotnet ef migrations add InitialIntegrationSchema` manually from a host containing the .NET 10 SDK, as the target execution environment may not possess full SDK binaries.
