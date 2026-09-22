# Customs Management & Clearance Database Architecture

## Overview
The `smart_customs` database is the authoritative source for all things related to customs cases, assessments, tariff schedules, duty calculations, and cargo clearance. It intentionally avoids mirroring Declaration, ECTN, or Payment systems entirely to enforce a microservices boundary.

## ERD Core Groupings
- **CustomsCase** (Root Aggregate)
  - Connects to **CustomsReference** (Declarations, ECTN, etc.)
  - Connects to **CustomsAssessment** -> (**DutyAssessment**, **TaxAssessment**)
  - Connects to **CustomsValuation** -> (**CustomsValuationComponent**)
  - Connects to **InspectionOrder**, **CustomsHold**, **CustomsDecision**, **CustomsClearance**.
- **Configuration** (Version-Controlled Tariff/Rates Engine)
  - **TariffSchedule** -> **TariffScheduleVersion** -> **TariffItem**
  - **CustomsRate** -> **CustomsRateVersion**
  - **CustomsRule** -> **CustomsRuleVersion**
  - **ExemptionRule**

## Reproducibility & Immutability
All government rates (Tariff, Rules, Customs Rates) are heavily versioned (`VersionNumber`). When a historical `CustomsAssessment` is calculated, it directly snapshots the exact version IDs of the Tariff and Rules used. Subsequent publication of a new government rate (e.g. Duty changes from 10% to 15%) creates a new `TariffScheduleVersion` and does not silently alter historical assessments.

## Financial Precision
Because Customs deals heavily in international monetary conversions and weight, the following types were enforced mapped onto PostgreSQL `numeric(19,4)`:
- `CustomsValue`, `DutyAmount`, `CustomsChargesAmount`, `TotalAmount`
- `TaxableBase`, `Quantity`, `Rate`, `CalculatedAmount`
Exchange Rates use `numeric(19,6)`.

## Integration
Events like `CustomsCaseCreated` and `ClearanceApproved` use the `OutboxMessage` pattern to broadcast asynchronously to the gateway or other services.
