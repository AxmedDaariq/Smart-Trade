# Notification PostgreSQL Documentation

This directory encapsulates schema documentation, ERDs, and migration standards for the `smart_notification` database.

## Critical Ownership Principle
The Notification Service is responsible for notification templates, rules, and delivery states across SMS, Email, Push, and In-App channels. **IT DOES NOT OWN THE BUSINESS DOMAIN.**

No `User`, `Driver`, `Vehicle`, `CustomsDeclaration`, or `Payment` tables exist here. The database stores strictly reference IDs (e.g., `UserReference`, `SourceEventId`) linking these domain boundaries together.

## Vault Integration
`NotificationProviderReference` strictly interfaces with Vault secrets (e.g., Twilio API Keys, SendGrid SMTP passwords) at runtime. Credentials must NEVER be stored in plaintext within this database.

## Execution
Run `dotnet ef migrations add InitialNotificationSchema` manually from a host containing the .NET 10 SDK.
