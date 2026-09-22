# Fleet & Vehicle Management Database Architecture

## Overview
The `smart_fleet` database is the authoritative source for fleet companies, vehicles (trucks/trailers), drivers, vehicle compliance, and driver verifications. 

It forms the foundational operational context for physical movement in the Smart Trade & Logistics Corridor.

## ERD Core Groupings
- **Core Assets** 
  - **FleetCompany** -> **Vehicle** / **Truck** / **Trailer** / **Driver**
- **Compliance & Credentialing**
  - **VehicleRegistration**, **VehicleCompliance**, **DriverLicense**, **DriverQualification**, **DriverCompliance**, **DriverVerification**
- **Operations & Assignments**
  - **DispatchOrder** (With **DispatchRequirement**)
  - **VehicleAssignment**, **DriverAssignment**, **TrailerAssignment**
- **History & Security**
  - **FleetHold** (Blocking states)
  - **FleetAuditEntry** (Traceability)
  - **StatusHistories**

## Multi-Tenancy & Isolation
All entities require a `TenantId`. Fleet operations are strictly siloed. A `FleetCompany` maps to a globally authenticated `OrganizationId` originating from the Identity database. Tenants cannot read or dispatch vehicles/drivers outside of their isolation boundary.

## Integration Boundaries (Strictly Enforced)
The Fleet Service must **NOT** replicate data belonging to:
- **Customs**: Tariffs, taxes, clearance rules, assessments.
- **ECTN**: Certificates.
- **GPS / Telematics**: GPS tables, live locations, telemetry data.
- **Payment**: Invoices, transactions.

Fleet stores only *external references* (e.g., `CustomsReleaseReference`, `DeclarationReference`) within the `DispatchOrder` or via `FleetExternalReference` to trigger readiness evaluations based on webhooks.
