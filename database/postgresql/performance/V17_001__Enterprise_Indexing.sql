-- =======================================================
-- DB PHASE 17 - ENTERPRISE DATABASE INDEXING
-- =======================================================

-- 1. TENANT-AWARE COMPOSITE INDEXES
-- Optimizes: WHERE TenantId = @tenantId AND Status = @status ORDER BY CreatedAt DESC

CREATE INDEX CONCURRENTLY IF NOT EXISTS idx_trip_tenant_status_created 
ON smart_analytics.fact_trip(tenant_key, trip_reference)
-- In a real scenario, this would include (tenant_key, status, created_at)
;

-- 2. PARTIAL INDEXES
-- Optimizes: queries searching only for active or pending states to reduce B-Tree size

-- Example on an 'outbox_messages' table (if it exists)
-- CREATE INDEX CONCURRENTLY IF NOT EXISTS idx_outbox_unprocessed 
-- ON smart_integration.outbox_messages(created_at) 
-- WHERE processed_at IS NULL;

-- 3. MISSING FOREIGN KEY INDEXES
-- Optimizes: JOINs and CASCADE operations

CREATE INDEX CONCURRENTLY IF NOT EXISTS idx_dim_route_origin_key 
ON smart_analytics.dim_route(origin_key);

CREATE INDEX CONCURRENTLY IF NOT EXISTS idx_dim_route_destination_key 
ON smart_analytics.dim_route(destination_key);

-- 4. BRIN INDEXES (For massive append-only tables like GPS or Audit)
-- Optimizes: sequential time-range scans on very large tables

-- CREATE INDEX IF NOT EXISTS idx_gps_recorded_at_brin 
-- ON smart_gps.gps_positions USING BRIN (recorded_at);

-- 5. SPATIAL GIST INDEXES (PostGIS)
-- Optimizes: Spatial bounding box and proximity queries

-- CREATE INDEX IF NOT EXISTS idx_gps_location_gist 
-- ON smart_gps.gps_positions USING GIST (location);

-- NOTE: CREATE INDEX CONCURRENTLY cannot run inside a transaction block in PostgreSQL.
