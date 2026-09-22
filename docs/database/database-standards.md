# Database Standards

## Naming Conventions
- **C#**: `PascalCase`
- **PostgreSQL**: `snake_case`

## Primary Keys
- Use `uuid` (`Guid` in C#) for service-owned entity identifiers.
- Column name: `id`

## Foreign Keys
- Suffix with `_id` (e.g., `organization_id`)

## Money and Decimals
- Use PostgreSQL `NUMERIC(19,4)` for financial precision. NEVER use `float` or `double`.

## Timestamps
- `created_at_utc`
- `updated_at_utc`
- All persisted timestamps MUST be UTC.

## PostGIS
- Standard spatial reference: SRID 4326.
