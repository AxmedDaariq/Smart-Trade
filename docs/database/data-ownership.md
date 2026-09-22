# Data Ownership

## Principles
- **Every business service owns its own data.**
- NEVER create a shared business database schema between services.
- Data sharing MUST be done via REST APIs, API Gateway, or Integration Events (RabbitMQ/Event Bus).

## Anti-Patterns Forbidden
- Customs service accessing Fleet database directly.
- Payment service accessing Customs database directly.
- Direct operational database access from Analytics.
