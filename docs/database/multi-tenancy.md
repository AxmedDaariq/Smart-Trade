# Multi-Tenancy

## Standard
The entire platform is multi-tenant. Tenant isolation MUST be enforced at the application/service level.

## Key Columns
- `tenant_id`
- `organization_id`

## Validation
DO NOT trust `TenantId` supplied directly by the client. It must originate from authenticated identity and authorized server-side context.

## Tenant Types
- Government
- LogisticsCompany
- Trader
- PortAuthority
- DevelopmentPartner
- Platform
