# Analytics Architecture

The Enterprise Analytics Data Store operates independently from the operational microservices. It aggregates business intelligence asynchronously.

## Core Rules
1. **No Operational Querying**: Operational microservices (Identity, Payment, Fleet, Customs, etc.) never query `smart_analytics`.
2. **No Analytics Callbacks**: The Analytics service does not reach into operational databases to scrape data.
3. **Event Driven Ingestion**: Data arrives purely via RabbitMQ Domain Events (`TripCompletedEvent`, `PaymentSettledEvent`, etc.)

## Ingestion Pipeline
We utilize an Idempotent Consumer pattern via MassTransit.
When a domain event arrives, the consumer checks the Analytics database (e.g. `FactTrips`) by the operational Correlation ID (`TripId`). If it exists, it ignores it. If not, it transforms the data into Star Schema Dimensions/Facts and inserts it.

## Dimensions & Facts (Star Schema)
Data is structured utilizing SCD Type 2 tracking for dimensional changes over time (e.g., `DimTenant`, `DimVehicle`).
Facts (`FactTrip`, `FactPaymentOperational`) track numerical analysis tied by `DateKey` for rapid querying and dashboard rendering.
