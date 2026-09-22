# Indexing Standards

## Rules
- Every index must have a justified reason based on query patterns.
- Common index columns: `tenant_id`, `created_at_utc`, `updated_at_utc`, `status`, `external_reference`.

## PostGIS
- Use `GiST` indexes for appropriate geometry/geography columns.

## Caution
Avoid excessive indexes to minimize write costs, storage, and maintenance overhead.
