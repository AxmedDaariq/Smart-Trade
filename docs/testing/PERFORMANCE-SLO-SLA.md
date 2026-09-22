# Platform SLO / SLA Framework (TARGETS)

## 1. Interactive APIs (User-Facing)
| Metric | Target | Actual | Status |
| :--- | :--- | :--- | :--- |
| Gateway p95 Latency | < 200ms | N/A | **NOT TESTED** |
| Identity Token p99 Latency | < 150ms | N/A | **NOT TESTED** |
| Control Tower Dashboard Load | < 2000ms | N/A | **NOT TESTED** |
| Fleet Lookup p95 Latency | < 300ms | N/A | **NOT TESTED** |

## 2. Ingestion Pipelines (Machine-Facing)
| Metric | Target | Actual | Status |
| :--- | :--- | :--- | :--- |
| GPS Ingestion p95 Latency | < 50ms | N/A | **NOT TESTED** |
| ANPR Auth Lookup p95 Latency| < 100ms | N/A | **NOT TESTED** |
| Payment Webhook Idempotency | < 50ms | N/A | **NOT TESTED** |

## 3. Real-Time & Event Propagation
| Metric | Target | Actual | Status |
| :--- | :--- | :--- | :--- |
| GPS to Control Tower (End-to-End) | < 500ms | N/A | **NOT TESTED** |
| MassTransit Consumer Lag (Max) | < 1000ms | N/A | **NOT TESTED** |
| RabbitMQ Queue Depth (Sustained) | < 500 msgs| N/A | **NOT TESTED** |

## 4. Availability & Error Rates
| Metric | Target | Actual | Status |
| :--- | :--- | :--- | :--- |
| API Gateway Error Rate (5xx) | < 0.1% | N/A | **NOT TESTED** |
| SignalR Reconnect Rate | < 2.0% | N/A | **NOT TESTED** |
