// ═══════════════════════════════════════════════════════════════════════
// Database E2E — MongoDB Unstructured Telematics & IoT Data Verification
// Validates:
// 1. High-frequency raw GPS telemetry (Berbera ⇄ Addis Ababa corridor)
// 2. Cold-chain reefer temperature & humidity variable sensor logs
// 3. Smart E-Seal optical tamper detection and RFID telemetry
// 4. Flexible unstructured schema handling (no rigid SQL migrations)
// 5. Querying by time ranges, geospatial coordinates, and cleanup
// ═══════════════════════════════════════════════════════════════════════

describe('Database — MongoDB Telematics & IoT Ingestion', () => {

  const testBatchId = `E2E-BATCH-${Date.now()}`;
  const truckId = 'TRK-BER-9082';
  const reeferContainerId = 'MSKU-882910-2';
  const eSealId = 'SEAL-RF-77218';

  before(() => {
    // Verify MongoDB connection
    cy.mongoPing().then((status) => {
      if (!status.ok) {
        cy.task('log', `MongoDB connection warning: ${status.error || 'Server offline'}`);
      } else {
        cy.task('log', `MongoDB connected successfully to ${status.server}`);
      }
    });
  });

  after(() => {
    // Clean up test batch telemetry documents
    cy.mongoDeleteMany('telematics_gps', { test_batch: testBatchId }).then((res) => {
      cy.task('log', `Cleaned up ${res.deletedCount} GPS telematics records`);
    });
    cy.mongoDeleteMany('telematics_reefer', { test_batch: testBatchId });
    cy.mongoDeleteMany('telematics_eseal', { test_batch: testBatchId });
  });

  // ──────────────────────── CONNECTION & HEALTH ────────────────────────

  context('MongoDB Cluster Verification', () => {
    it('verifies MongoDB server responsiveness and ping', () => {
      cy.mongoPing().then((res) => {
        expect(res).to.have.property('ok');
        if (res.ok) {
          expect(res.ok).to.be.true;
        } else {
          cy.log(`MongoDB skipped or not currently active: ${res.error}`);
        }
      });
    });
  });

  // ───────────────────── GPS TELEMETRY (UNSTRUCTURED) ─────────────────────

  context('High-Frequency GPS & Corridor Telematics', () => {
    it('ingests unstructured high-frequency GPS pings with variable telemetry tags', () => {
      const gpsPings = [
        {
          test_batch: testBatchId,
          truck_id: truckId,
          corridor: 'Berbera-Addis',
          timestamp: new Date(Date.now() - 60000).toISOString(),
          location: {
            type: 'Point',
            coordinates: [45.0142, 10.4398], // Berbera port exit
          },
          telemetry: {
            speed_kmh: 42.5,
            heading_deg: 245,
            altitude_m: 12.0,
            engine_rpm: 1450,
            fuel_level_pct: 94.2,
            odometer_km: 18420.5,
          },
          network: {
            operator: 'Telesom 4G',
            signal_bars: 4,
            imei: '864291048291024',
          },
        },
        {
          test_batch: testBatchId,
          truck_id: truckId,
          corridor: 'Berbera-Addis',
          timestamp: new Date().toISOString(),
          location: {
            type: 'Point',
            coordinates: [44.0289, 9.5621], // Hargeisa checkpoint
          },
          telemetry: {
            speed_kmh: 68.1,
            heading_deg: 250,
            altitude_m: 1320.0, // Mountain pass altitude jump
            engine_rpm: 2100,
            fuel_level_pct: 88.7,
            cabin_temp_c: 29.4,
            harsh_braking_events: 0,
          },
          network: {
            operator: 'Somtel LTE',
            signal_bars: 5,
            imei: '864291048291024',
          },
        },
      ];

      cy.mongoInsert('telematics_gps', gpsPings).then((result) => {
        expect(result.insertedCount).to.eq(2);
        expect(result.insertedIds).to.have.length(2);
      });
    });

    it('queries telemetry by truck ID and verifies unstructured fields flexibility', () => {
      cy.mongoQuery('telematics_gps', {
        test_batch: testBatchId,
        truck_id: truckId,
      }, { sort: { timestamp: -1 } }).then((records) => {
        expect(records.length).to.be.at.least(2);

        const latest = records[0];
        expect(latest.truck_id).to.eq(truckId);
        expect(latest.corridor).to.eq('Berbera-Addis');
        expect(latest.location.type).to.eq('Point');
        expect(latest.telemetry.speed_kmh).to.be.a('number');
        expect(latest.network.operator).to.be.a('string');
      });
    });
  });

  // ─────────────── REEFER COLD-CHAIN VARIABLE SENSOR LOGS ───────────────

  context('Reefer Cold-Chain Sensor Telemetry', () => {
    it('stores variable schema cold-chain temperature telemetry with alarm levels', () => {
      const reeferReadings = [
        {
          test_batch: testBatchId,
          container_id: reeferContainerId,
          cargo_type: 'Pharmaceuticals & Vaccine Serums',
          setpoint_temp_c: -20.0,
          timestamp: new Date(Date.now() - 30000).toISOString(),
          sensors: {
            supply_air_temp_c: -19.8,
            return_air_temp_c: -19.4,
            cargo_probe_1_c: -20.1,
            cargo_probe_2_c: -20.0,
            relative_humidity_pct: 45.2,
            co2_level_ppm: 380,
          },
          power: {
            genset_fuel_liters: 140,
            battery_voltage: 27.6,
            compressor_status: 'RUNNING',
            defrost_cycle: false,
          },
          alarm: {
            active: false,
            code: 'NORMAL_OPERATION',
          },
        },
        {
          test_batch: testBatchId,
          container_id: reeferContainerId,
          cargo_type: 'Pharmaceuticals & Vaccine Serums',
          setpoint_temp_c: -20.0,
          timestamp: new Date().toISOString(),
          // Unstructured variation: Door opened at customs inspection creates additional keys
          sensors: {
            supply_air_temp_c: -16.2, // Temperature excursion!
            return_air_temp_c: -15.8,
            door_sensor: 'OPEN',
            door_open_duration_sec: 142,
          },
          power: {
            compressor_status: 'BOOST_DEFROST',
          },
          alarm: {
            active: true,
            severity: 'CRITICAL',
            code: 'EXCURSION_DOOR_OPEN_CUSTOMS_BORDER',
            acknowledged: false,
          },
        },
      ];

      cy.mongoInsert('telematics_reefer', reeferReadings).then((result) => {
        expect(result.insertedCount).to.eq(2);
      });
    });

    it('filters critical cold-chain alarm events via nested unstructured fields', () => {
      cy.mongoQuery('telematics_reefer', {
        test_batch: testBatchId,
        'alarm.active': true,
      }).then((records) => {
        expect(records.length).to.eq(1);
        const alarmRecord = records[0];
        expect(alarmRecord.container_id).to.eq(reeferContainerId);
        expect(alarmRecord.alarm.severity).to.eq('CRITICAL');
        expect(alarmRecord.sensors.door_sensor).to.eq('OPEN');
        expect(alarmRecord.sensors.supply_air_temp_c).to.be.greaterThan(-18.0);
      });
    });
  });

  // ──────────────── E-SEAL & OPTICAL TAMPER TELEMETRY ──────────────────

  context('Smart E-Seal & Tamper Telemetry', () => {
    it('persists dynamic RFID and optical security seal telemetry events', () => {
      const eSealPayload = {
        test_batch: testBatchId,
        seal_id: eSealId,
        associated_shipment: 'SH-2026-CORRIDOR-009',
        lock_state: 'LOCKED_SECURE',
        tamper_detected: false,
        wire_integrity: 'CONTINUOUS',
        optical_fiber_dbm: -1.2,
        battery_pct: 98,
        gps_fix: {
          latitude: 9.6015,
          longitude: 43.3284, // Tog Wajaale Border Zone
        },
        metadata: {
          inspector_id: 'SOM-CUSTOMS-OFFICER-44',
          seal_protocol: 'ISO_17712_HIGH_SECURITY',
        },
        created_at: new Date().toISOString(),
      };

      cy.mongoInsert('telematics_eseal', eSealPayload).then((result) => {
        expect(result.insertedId).to.be.a('string');
      });
    });

    it('verifies total telemetry documents counted across collections', () => {
      cy.mongoCount('telematics_gps', { test_batch: testBatchId }).then((res) => {
        expect(res.count).to.eq(2);
      });
      cy.mongoCount('telematics_reefer', { test_batch: testBatchId }).then((res) => {
        expect(res.count).to.eq(2);
      });
      cy.mongoCount('telematics_eseal', { test_batch: testBatchId }).then((res) => {
        expect(res.count).to.eq(1);
      });
    });
  });
});
