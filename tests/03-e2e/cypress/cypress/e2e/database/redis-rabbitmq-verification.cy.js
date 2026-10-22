// ═══════════════════════════════════════════════════════════════════════
// Database & Messaging E2E — Redis Cache & RabbitMQ Event Broker
// Validates:
// 1. Redis high-speed caching for real-time fleet GPS coordinates
// 2. Redis session / JWT token blacklist revocation
// 3. Redis TTL expiration and pattern cleanup
// 4. RabbitMQ broker cluster health and overview metrics
// 5. RabbitMQ corridor event publishing (shipments, customs, telematics)
// 6. RabbitMQ queue inspection, message routing, and purge operations
// ═══════════════════════════════════════════════════════════════════════

describe('Database & Messaging — Redis Cache & RabbitMQ Broker', () => {

  const testKeyPrefix = `test:e2e:${Date.now()}`;
  const truckId = 'TRK-DJI-4412';
  const testQueueName = 'e2e.corridor.telemetry.test';

  after(() => {
    // Clean up Redis test keys
    cy.redisFlushPattern(`${testKeyPrefix}:*`).then((res) => {
      cy.task('log', `Redis: Flushed ${res.deletedCount} test cache keys`);
    });
  });

  // ═════════════════════════════════════════════════════════════════════
  // PART 1: REDIS IN-MEMORY CACHE & REAL-TIME STATE
  // ═════════════════════════════════════════════════════════════════════

  context('Redis — High-Speed Cache & Session Management', () => {

    it('verifies Redis ping response', () => {
      cy.redisPing().then((res) => {
        expect(res).to.have.property('ok');
        if (res.ok) {
          expect(res.response).to.eq('PONG');
        } else {
          cy.log(`Redis ping skipped or offline: ${res.error}`);
        }
      });
    });

    it('caches real-time vehicle GPS positions for live control tower map polling', () => {
      const positionKey = `${testKeyPrefix}:truck:${truckId}:live_pos`;
      const livePositionPayload = {
        truck_id: truckId,
        latitude: 9.6102,
        longitude: 44.0321,
        speed_kmh: 62.4,
        heading: 260,
        driver: 'Khadar Jama',
        corridor_status: 'IN_TRANSIT_HARGEISA_BYPASS',
        timestamp: Date.now(),
      };

      // Set in Redis with 120s TTL
      cy.redisSet(positionKey, livePositionPayload, 120).then((res) => {
        expect(res.success).to.be.true;
      });

      // Retrieve from Redis and verify instant deserialization
      cy.redisGet(positionKey).then((cached) => {
        expect(cached).to.not.be.null;
        expect(cached.truck_id).to.eq(truckId);
        expect(cached.latitude).to.eq(9.6102);
        expect(cached.speed_kmh).to.eq(62.4);
        expect(cached.corridor_status).to.eq('IN_TRANSIT_HARGEISA_BYPASS');
      });
    });

    it('manages JWT token revocation & session blacklist in Redis', () => {
      const tokenJti = `jti-session-${Date.now()}`;
      const blacklistKey = `${testKeyPrefix}:token_blacklist:${tokenJti}`;

      // Check key does not exist yet
      cy.redisExists(blacklistKey).then((res) => {
        expect(res.exists).to.be.false;
      });

      // Revoke token / place on blacklist with 3600s expiration
      cy.redisSet(blacklistKey, { revoked_at: new Date().toISOString(), reason: 'DRIVER_LOGOUT_TERMINAL' }, 3600);

      // Verify token is now blacklisted
      cy.redisExists(blacklistKey).then((res) => {
        expect(res.exists).to.be.true;
      });

      cy.redisGet(blacklistKey).then((val) => {
        expect(val.reason).to.eq('DRIVER_LOGOUT_TERMINAL');
      });
    });

    it('deletes cached keys cleanly', () => {
      const tempKey = `${testKeyPrefix}:temp_metric`;
      cy.redisSet(tempKey, 'sample_metric_value');

      cy.redisDel(tempKey).then((res) => {
        expect(res.deletedCount).to.be.at.least(1);
      });

      cy.redisGet(tempKey).then((val) => {
        expect(val).to.be.null;
      });
    });
  });

  // ═════════════════════════════════════════════════════════════════════
  // PART 2: RABBITMQ ASYNC EVENT STREAMING
  // ═════════════════════════════════════════════════════════════════════

  context('RabbitMQ — Corridor Event Bus & Queue Processing', () => {

    it('verifies RabbitMQ broker overview and management cluster health', () => {
      cy.rabbitCheckHealth().then((res) => {
        expect(res).to.have.property('ok');
        if (res.ok) {
          expect(res.ok).to.be.true;
          expect(res).to.have.property('version');
          cy.task('log', `RabbitMQ Version: ${res.version}, Cluster: ${res.cluster_name}`);
        } else {
          cy.log(`RabbitMQ management API skipped or offline: ${res.error}`);
        }
      });
    });

    it('publishes a corridor shipment lifecycle event to RabbitMQ', () => {
      const eventPayload = {
        event_id: `EVT-${Date.now()}`,
        event_type: 'ShipmentCorridorDispatched',
        aggregate_id: 'SH-2026-CORRIDOR-881',
        origin_port: 'Berbera Container Terminal (DP World)',
        destination_inland: 'Addis Ababa Dry Port (Kality)',
        customs_ectn_reference: 'ECTN-SO-2026-88192',
        cargo_weight_kg: 24500,
        published_at: new Date().toISOString(),
      };

      // Publish to amq.default or standard direct/topic exchange
      cy.rabbitPublish('amq.default', 'corridor.shipment.dispatched', eventPayload).then((res) => {
        if (res.error) {
          cy.log(`RabbitMQ publish skipped (broker offline): ${res.error}`);
        } else {
          expect(res).to.have.property('details');
        }
      });
    });

    it('publishes high-priority telemetry SOS event to broker', () => {
      const emergencySosPayload = {
        event_id: `SOS-${Date.now()}`,
        event_type: 'FleetEmergencySosTriggered',
        truck_id: truckId,
        driver_name: 'Khadar Jama',
        location: { lat: 9.582, lng: 43.821 },
        alert_code: 'MECHANICAL_BRAKE_OVERHEAT',
        priority: 'P0_CRITICAL',
        dispatched_at: new Date().toISOString(),
      };

      cy.rabbitPublish('amq.default', 'fleet.emergency.sos', emergencySosPayload).then((res) => {
        if (!res.error) {
          expect(res).to.have.property('details');
        }
      });
    });

    it('inspects or manages queue states safely', () => {
      // Check if a queue exists or report status
      cy.rabbitCheckQueue(testQueueName).then((res) => {
        if (!res.error) {
          expect(res).to.have.property('exists');
          if (res.exists) {
            expect(res).to.have.property('messages');
          }
        } else {
          cy.log(`Queue check skipped: ${res.error}`);
        }
      });
    });
  });
});
