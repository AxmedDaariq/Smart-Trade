CREATE SCHEMA IF NOT EXISTS smart_analytics;

-- ==========================================
-- DIMENSIONS
-- ==========================================

CREATE TABLE smart_analytics.dim_tenant (
    tenant_key UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,
    tenant_reference VARCHAR(255),
    tenant_type VARCHAR(50),
    organization_reference VARCHAR(255),
    country_reference VARCHAR(100),
    effective_from_utc TIMESTAMP WITH TIME ZONE NOT NULL,
    effective_to_utc TIMESTAMP WITH TIME ZONE,
    is_current BOOLEAN NOT NULL DEFAULT true
);
CREATE INDEX idx_dim_tenant_id ON smart_analytics.dim_tenant(tenant_id, is_current);

CREATE TABLE smart_analytics.dim_date (
    date_key INT PRIMARY KEY, -- e.g., 20260921
    full_date DATE NOT NULL,
    day_of_month INT NOT NULL,
    day_of_week INT NOT NULL,
    week_of_year INT NOT NULL,
    month_of_year INT NOT NULL,
    month_name VARCHAR(20) NOT NULL,
    quarter INT NOT NULL,
    year INT NOT NULL,
    is_weekend BOOLEAN NOT NULL,
    fiscal_period VARCHAR(50)
);
CREATE INDEX idx_dim_date_full_date ON smart_analytics.dim_date(full_date);

CREATE TABLE smart_analytics.dim_time (
    time_key INT PRIMARY KEY, -- e.g., 143025 (14:30:25)
    hour_of_day INT NOT NULL,
    minute_of_hour INT NOT NULL,
    second_of_minute INT NOT NULL,
    time_bucket VARCHAR(50)
);

CREATE TABLE smart_analytics.dim_vehicle (
    vehicle_key UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    vehicle_id UUID NOT NULL,
    tenant_id UUID,
    vehicle_type VARCHAR(100),
    vehicle_category VARCHAR(100),
    effective_from_utc TIMESTAMP WITH TIME ZONE NOT NULL,
    effective_to_utc TIMESTAMP WITH TIME ZONE,
    is_current BOOLEAN NOT NULL DEFAULT true
);
CREATE INDEX idx_dim_vehicle_id ON smart_analytics.dim_vehicle(vehicle_id, is_current);

CREATE TABLE smart_analytics.dim_driver (
    driver_key UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    driver_id UUID NOT NULL,
    tenant_id UUID,
    effective_from_utc TIMESTAMP WITH TIME ZONE NOT NULL,
    effective_to_utc TIMESTAMP WITH TIME ZONE,
    is_current BOOLEAN NOT NULL DEFAULT true
);

CREATE TABLE smart_analytics.dim_route (
    route_key UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    route_id UUID NOT NULL,
    tenant_id UUID,
    origin_key UUID,
    destination_key UUID,
    effective_from_utc TIMESTAMP WITH TIME ZONE NOT NULL,
    effective_to_utc TIMESTAMP WITH TIME ZONE,
    is_current BOOLEAN NOT NULL DEFAULT true
);

CREATE TABLE smart_analytics.dim_corridor (
    corridor_key UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    corridor_id UUID NOT NULL,
    tenant_id UUID,
    effective_from_utc TIMESTAMP WITH TIME ZONE NOT NULL,
    effective_to_utc TIMESTAMP WITH TIME ZONE,
    is_current BOOLEAN NOT NULL DEFAULT true
);

-- ==========================================
-- FACTS
-- ==========================================

CREATE TABLE smart_analytics.fact_trip (
    trip_fact_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    trip_id UUID NOT NULL,
    trip_reference VARCHAR(255),
    date_key INT NOT NULL REFERENCES smart_analytics.dim_date(date_key),
    tenant_key UUID REFERENCES smart_analytics.dim_tenant(tenant_key),
    vehicle_key UUID REFERENCES smart_analytics.dim_vehicle(vehicle_key),
    driver_key UUID REFERENCES smart_analytics.dim_driver(driver_key),
    route_key UUID REFERENCES smart_analytics.dim_route(route_key),
    corridor_key UUID REFERENCES smart_analytics.dim_corridor(corridor_key),
    origin_key UUID,
    destination_key UUID,
    trip_count INT NOT NULL DEFAULT 0,
    completed_trip_count INT NOT NULL DEFAULT 0,
    cancelled_trip_count INT NOT NULL DEFAULT 0,
    duration_seconds BIGINT,
    distance_km NUMERIC(10, 2),
    delay_minutes BIGINT,
    estimated_duration BIGINT,
    actual_duration BIGINT,
    checkpoint_count INT,
    incident_count INT,
    deviation_count INT,
    border_wait_minutes BIGINT,
    dry_port_wait_minutes BIGINT
);
CREATE INDEX idx_fact_trip_date_tenant ON smart_analytics.fact_trip(date_key, tenant_key);

CREATE TABLE smart_analytics.fact_fleet_snapshot (
    snapshot_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    date_key INT NOT NULL REFERENCES smart_analytics.dim_date(date_key),
    tenant_key UUID REFERENCES smart_analytics.dim_tenant(tenant_key),
    vehicle_count INT NOT NULL,
    active_vehicle_count INT NOT NULL,
    inactive_vehicle_count INT NOT NULL,
    available_vehicle_count INT NOT NULL,
    in_transit_vehicle_count INT NOT NULL,
    maintenance_vehicle_count INT NOT NULL,
    compliance_due_count INT NOT NULL
);

CREATE TABLE smart_analytics.fact_gps_daily (
    gps_fact_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    date_key INT NOT NULL REFERENCES smart_analytics.dim_date(date_key),
    tenant_key UUID REFERENCES smart_analytics.dim_tenant(tenant_key),
    vehicle_key UUID REFERENCES smart_analytics.dim_vehicle(vehicle_key),
    position_count INT,
    distance_km NUMERIC(10, 2),
    moving_minutes BIGINT,
    idle_minutes BIGINT,
    stopped_minutes BIGINT,
    average_speed NUMERIC(10, 2),
    maximum_speed NUMERIC(10, 2),
    deviation_minutes BIGINT,
    offline_minutes BIGINT,
    tracking_coverage_percentage NUMERIC(5, 2)
);

CREATE TABLE smart_analytics.fact_corridor_performance (
    performance_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    date_key INT NOT NULL REFERENCES smart_analytics.dim_date(date_key),
    tenant_key UUID REFERENCES smart_analytics.dim_tenant(tenant_key),
    corridor_key UUID REFERENCES smart_analytics.dim_corridor(corridor_key),
    route_key UUID REFERENCES smart_analytics.dim_route(route_key),
    checkpoint_key UUID,
    trip_count INT,
    completed_trips INT,
    average_transit_time BIGINT,
    average_delay BIGINT,
    average_eta_error BIGINT,
    deviation_count INT,
    incident_count INT,
    checkpoint_wait_time BIGINT,
    border_wait_time BIGINT,
    delivery_success_rate NUMERIC(5, 2)
);

CREATE TABLE smart_analytics.fact_border_crossing (
    crossing_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    date_key INT NOT NULL REFERENCES smart_analytics.dim_date(date_key),
    tenant_key UUID REFERENCES smart_analytics.dim_tenant(tenant_key),
    vehicle_key UUID REFERENCES smart_analytics.dim_vehicle(vehicle_key),
    trip_id UUID,
    border_checkpoint_id UUID,
    gate_id UUID,
    crossing_count INT,
    average_processing_minutes BIGINT,
    average_waiting_minutes BIGINT,
    authorization_failure_count INT,
    manual_verification_count INT,
    incident_count INT
);

CREATE TABLE smart_analytics.fact_anpr_operational (
    anpr_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    date_key INT NOT NULL REFERENCES smart_analytics.dim_date(date_key),
    tenant_key UUID REFERENCES smart_analytics.dim_tenant(tenant_key),
    recognition_count INT,
    matched_count INT,
    unmatched_count INT,
    manual_verification_count INT,
    gate_passage_count INT,
    authorization_denied_count INT,
    recognition_latency_ms BIGINT,
    camera_failure_count INT
);

CREATE TABLE smart_analytics.fact_customs_operational (
    customs_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    date_key INT NOT NULL REFERENCES smart_analytics.dim_date(date_key),
    tenant_key UUID REFERENCES smart_analytics.dim_tenant(tenant_key),
    declaration_count INT,
    assessment_count INT,
    inspection_count INT,
    hold_count INT,
    clearance_count INT,
    release_count INT,
    processing_duration BIGINT,
    inspection_duration BIGINT,
    clearance_duration BIGINT,
    assessed_duty_amount NUMERIC(19, 4),
    collected_duty_amount NUMERIC(19, 4)
);

CREATE TABLE smart_analytics.fact_payment_operational (
    payment_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    date_key INT NOT NULL REFERENCES smart_analytics.dim_date(date_key),
    tenant_key UUID REFERENCES smart_analytics.dim_tenant(tenant_key),
    payment_count INT,
    successful_payment_count INT,
    failed_payment_count INT,
    refund_count INT,
    reconciliation_count INT,
    processing_duration BIGINT,
    payment_amount NUMERIC(19, 4)
);

CREATE TABLE smart_analytics.fact_ectn_operational (
    ectn_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    date_key INT NOT NULL REFERENCES smart_analytics.dim_date(date_key),
    tenant_key UUID REFERENCES smart_analytics.dim_tenant(tenant_key),
    application_count INT,
    issued_count INT,
    rejected_count INT,
    amended_count INT,
    processing_duration BIGINT,
    approval_duration BIGINT
);

CREATE TABLE smart_analytics.fact_vessel_port (
    port_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    date_key INT NOT NULL REFERENCES smart_analytics.dim_date(date_key),
    tenant_key UUID REFERENCES smart_analytics.dim_tenant(tenant_key),
    vessel_arrival_count INT,
    vessel_departure_count INT,
    port_call_count INT,
    berthing_duration BIGINT,
    waiting_duration BIGINT,
    cargo_movement_reference_count INT
);

CREATE TABLE smart_analytics.fact_notification (
    notification_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    date_key INT NOT NULL REFERENCES smart_analytics.dim_date(date_key),
    tenant_key UUID REFERENCES smart_analytics.dim_tenant(tenant_key),
    created_count INT,
    delivered_count INT,
    failed_count INT,
    read_count INT,
    retry_count INT,
    average_delivery_latency BIGINT
);

CREATE TABLE smart_analytics.fact_integration (
    integration_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    date_key INT NOT NULL REFERENCES smart_analytics.dim_date(date_key),
    tenant_key UUID REFERENCES smart_analytics.dim_tenant(tenant_key),
    provider_key UUID,
    integration_type VARCHAR(100),
    source_service VARCHAR(100),
    target_service VARCHAR(100),
    request_count INT,
    success_count INT,
    failure_count INT,
    retry_count INT,
    timeout_count INT,
    average_latency BIGINT,
    p95_latency BIGINT,
    p99_latency BIGINT,
    dead_letter_count INT
);

CREATE TABLE smart_analytics.fact_security_audit (
    audit_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    date_key INT NOT NULL REFERENCES smart_analytics.dim_date(date_key),
    tenant_key UUID REFERENCES smart_analytics.dim_tenant(tenant_key),
    login_failure_count INT,
    permission_denied_count INT,
    cross_tenant_attempt_count INT,
    bola_attempt_count INT,
    security_alert_count INT,
    export_count INT,
    suspicious_activity_count INT,
    integrity_failure_count INT
);

CREATE TABLE smart_analytics.fact_operational_incident (
    incident_id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    date_key INT NOT NULL REFERENCES smart_analytics.dim_date(date_key),
    tenant_key UUID REFERENCES smart_analytics.dim_tenant(tenant_key),
    service_key UUID,
    incident_type_key UUID,
    severity_key UUID,
    corridor_key UUID REFERENCES smart_analytics.dim_corridor(corridor_key),
    route_key UUID REFERENCES smart_analytics.dim_route(route_key),
    vehicle_key UUID REFERENCES smart_analytics.dim_vehicle(vehicle_key),
    incident_count INT,
    open_count INT,
    resolved_count INT,
    resolution_minutes BIGINT,
    severity_score NUMERIC(10, 2)
);

-- ==========================================
-- KPI & M&E
-- ==========================================

CREATE TABLE smart_analytics.kpi_definition (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID, -- nullable
    kpi_code VARCHAR(100) NOT NULL,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    category VARCHAR(100),
    unit VARCHAR(50),
    calculation_method VARCHAR(255),
    target_value NUMERIC(19, 4),
    warning_threshold NUMERIC(19, 4),
    critical_threshold NUMERIC(19, 4),
    frequency VARCHAR(50),
    owner VARCHAR(255),
    status VARCHAR(50),
    version INT NOT NULL DEFAULT 1,
    effective_from_utc TIMESTAMP WITH TIME ZONE NOT NULL,
    effective_to_utc TIMESTAMP WITH TIME ZONE,
    created_at_utc TIMESTAMP WITH TIME ZONE NOT NULL,
    updated_at_utc TIMESTAMP WITH TIME ZONE NOT NULL
);

CREATE TABLE smart_analytics.kpi_result (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,
    kpi_definition_id UUID NOT NULL REFERENCES smart_analytics.kpi_definition(id),
    period_start_utc TIMESTAMP WITH TIME ZONE NOT NULL,
    period_end_utc TIMESTAMP WITH TIME ZONE NOT NULL,
    value_numeric NUMERIC(19, 4),
    target_numeric NUMERIC(19, 4),
    variance_numeric NUMERIC(19, 4),
    status VARCHAR(50),
    calculated_at_utc TIMESTAMP WITH TIME ZONE NOT NULL,
    data_freshness_utc TIMESTAMP WITH TIME ZONE NOT NULL,
    data_quality_status VARCHAR(100),
    calculation_version INT NOT NULL,
    created_at_utc TIMESTAMP WITH TIME ZONE NOT NULL
);
CREATE INDEX idx_kpi_result_tenant_period ON smart_analytics.kpi_result(tenant_id, period_start_utc);

CREATE TABLE smart_analytics.me_indicator_definition (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID, -- nullable
    indicator_code VARCHAR(100) NOT NULL,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    unit VARCHAR(50),
    baseline_value NUMERIC(19, 4),
    target_value NUMERIC(19, 4),
    calculation_method VARCHAR(255),
    frequency VARCHAR(50),
    data_source VARCHAR(255),
    version INT NOT NULL DEFAULT 1,
    effective_from_utc TIMESTAMP WITH TIME ZONE NOT NULL,
    effective_to_utc TIMESTAMP WITH TIME ZONE,
    status VARCHAR(50)
);

CREATE TABLE smart_analytics.me_indicator_result (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,
    indicator_definition_id UUID NOT NULL REFERENCES smart_analytics.me_indicator_definition(id),
    period_start_utc TIMESTAMP WITH TIME ZONE NOT NULL,
    period_end_utc TIMESTAMP WITH TIME ZONE NOT NULL,
    value_numeric NUMERIC(19, 4),
    baseline_numeric NUMERIC(19, 4),
    target_numeric NUMERIC(19, 4),
    variance_numeric NUMERIC(19, 4),
    status VARCHAR(50),
    calculated_at_utc TIMESTAMP WITH TIME ZONE NOT NULL,
    data_quality_status VARCHAR(100),
    created_at_utc TIMESTAMP WITH TIME ZONE NOT NULL
);

-- ==========================================
-- DATA QUALITY, LINEAGE & SYSTEM
-- ==========================================

CREATE TABLE smart_analytics.analytics_data_quality (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    source_service VARCHAR(100) NOT NULL,
    dataset VARCHAR(100) NOT NULL,
    tenant_id UUID,
    expected_record_count INT,
    received_record_count INT,
    rejected_record_count INT,
    duplicate_record_count INT,
    late_record_count INT,
    missing_field_count INT,
    invalid_record_count INT,
    freshness_seconds BIGINT,
    quality_score NUMERIC(5, 2),
    calculated_at_utc TIMESTAMP WITH TIME ZONE NOT NULL
);

CREATE TABLE smart_analytics.analytics_processed_event (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    message_id UUID NOT NULL,
    source_service VARCHAR(100) NOT NULL,
    event_type VARCHAR(100) NOT NULL,
    tenant_id UUID,
    payload_hash VARCHAR(255),
    processed_at_utc TIMESTAMP WITH TIME ZONE NOT NULL,
    status VARCHAR(50) NOT NULL,
    UNIQUE(message_id, source_service)
);

CREATE TABLE smart_analytics.analytics_ingestion_batch (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    source_service VARCHAR(100) NOT NULL,
    event_type VARCHAR(100) NOT NULL,
    schema_version VARCHAR(50),
    batch_reference VARCHAR(255),
    record_count INT,
    accepted_count INT,
    rejected_count INT,
    duplicate_count INT,
    started_at_utc TIMESTAMP WITH TIME ZONE NOT NULL,
    completed_at_utc TIMESTAMP WITH TIME ZONE,
    status VARCHAR(50) NOT NULL,
    error_summary TEXT
);

CREATE TABLE smart_analytics.report_definition (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID,
    report_code VARCHAR(100) NOT NULL,
    name VARCHAR(255) NOT NULL,
    description TEXT,
    report_type VARCHAR(100),
    query_reference VARCHAR(255),
    parameters_schema TEXT,
    format VARCHAR(50),
    frequency VARCHAR(50),
    access_policy TEXT,
    version INT NOT NULL DEFAULT 1,
    status VARCHAR(50),
    created_at_utc TIMESTAMP WITH TIME ZONE NOT NULL,
    updated_at_utc TIMESTAMP WITH TIME ZONE NOT NULL
);

CREATE TABLE smart_analytics.report_execution (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    tenant_id UUID NOT NULL,
    report_definition_id UUID NOT NULL REFERENCES smart_analytics.report_definition(id),
    requested_by UUID NOT NULL,
    parameters_json TEXT,
    status VARCHAR(50) NOT NULL,
    started_at_utc TIMESTAMP WITH TIME ZONE NOT NULL,
    completed_at_utc TIMESTAMP WITH TIME ZONE,
    record_count INT,
    storage_reference VARCHAR(500),
    error_message TEXT,
    expires_at_utc TIMESTAMP WITH TIME ZONE
);

CREATE TABLE smart_analytics.analytics_data_lineage (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    dataset VARCHAR(100) NOT NULL,
    source_service VARCHAR(100) NOT NULL,
    source_event_type VARCHAR(100),
    source_field VARCHAR(100),
    target_dataset VARCHAR(100),
    target_field VARCHAR(100),
    transformation_reference VARCHAR(255),
    version INT NOT NULL DEFAULT 1,
    created_at_utc TIMESTAMP WITH TIME ZONE NOT NULL
);
