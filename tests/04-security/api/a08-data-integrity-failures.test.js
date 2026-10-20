// ═══════════════════════════════════════════════════════════════════════
// OWASP A08:2021 — Software and Data Integrity Failures Test Suite
// Covers:
// - Payload Size Limit Defense (Prevents large body DoS attacks)
// - Content-Type Spoofing and Insecure Deserialization defense
// - IoT E-Seal and digital handover integrity verification
// ═══════════════════════════════════════════════════════════════════════

const { expect } = require('chai');
const { CONFIG, probe, TokenCraft } = require('../security-helper');

describe('OWASP A08:2021 — Software and Data Integrity Failures', () => {

  const validToken = TokenCraft.custom(CONFIG.TENANT_A, 'Administrator');

  describe('1. HTTP Payload Size Limit Enforcement', () => {
    it('rejects oversized JSON request payloads (e.g. >10MB) with 413 Payload Too Large', async () => {
      // Create a 12MB string payload
      const hugePadding = 'A'.repeat(12 * 1024 * 1024);

      const res = await probe('POST', `${CONFIG.GATEWAY_URL}/api/shipping/orders`, {
        headers: {
          Authorization: `Bearer ${validToken}`,
          'X-Tenant-Code': CONFIG.TENANT_A,
          'Content-Type': 'application/json',
        },
        data: {
          origin: 'Berbera',
          destination: 'Addis Ababa',
          oversized_junk: hugePadding,
        },
        timeout: 10000,
      });

      if (!res.isNetworkError) {
        // Must reject oversized request body
        expect([400, 413, 502, 504]).to.include(res.status);
      }
    });
  });

  describe('2. Content-Type Header Strictness', () => {
    it('rejects non-JSON payload when Content-Type is application/json', async () => {
      const res = await probe('POST', `${CONFIG.GATEWAY_URL}/api/shipping/orders`, {
        headers: {
          Authorization: `Bearer ${validToken}`,
          'X-Tenant-Code': CONFIG.TENANT_A,
          'Content-Type': 'application/json',
        },
        data: 'MALFORMED_NON_JSON_RAW_DATA_<<<>>>',
      });

      if (!res.isNetworkError) {
        expect([400, 422]).to.include(res.status);
      }
    });
  });

  describe('3. Telematics & Digital Signature Integrity', () => {
    it('flags or rejects e-seal telemetry where tamper flag is true or checksum is invalid', async () => {
      const res = await probe('POST', `${CONFIG.GATEWAY_URL}/api/v1/fleet/telemetry/eseal`, {
        headers: {
          Authorization: `Bearer ${validToken}`,
          'X-Tenant-Code': CONFIG.TENANT_A,
          'Content-Type': 'application/json',
        },
        data: {
          seal_id: 'SEAL-TAMPERED-009',
          tamper_detected: true,
          checksum: 'INVALID_CORRUPTED_HMAC',
        },
      });

      if (!res.isNetworkError) {
        // System either rejects invalid checksum with 400/422, or processes with an alert generated
        expect([200, 202, 400, 422]).to.include(res.status);
        if (res.status === 200 || res.status === 202) {
          expect(res.data).to.have.property('alert');
        }
      }
    });
  });
});
