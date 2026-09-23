/**
 * Integration Test: PostgreSQL / PostGIS Database Container
 */

import { Pool } from 'pg';
import { getPostgresPool } from '../helpers/dbClient';

describe('PostgreSQL / PostGIS Database Integration Tests', () => {
  let pool: Pool;

  beforeAll(async () => {
    pool = getPostgresPool();
  });

  afterAll(async () => {
    if (pool) {
      await pool.end();
    }
  });

  test('should successfully connect to PostgreSQL container and run SELECT 1', async () => {
    const client = await pool.connect();
    try {
      const res = await client.query('SELECT 1 AS connected, NOW() AS server_time');
      expect(res.rows).toHaveLength(1);
      expect(res.rows[0].connected).toBe(1);
      expect(res.rows[0].server_time).toBeInstanceOf(Date);
    } finally {
      client.release();
    }
  });

  test('should verify PostGIS spatial extension is available', async () => {
    const client = await pool.connect();
    try {
      await client.query('CREATE EXTENSION IF NOT EXISTS postgis;');
      const res = await client.query('SELECT PostGIS_Full_Version()');
      expect(res.rows).toHaveLength(1);
      const versionStr = res.rows[0].postgis_full_version;
      expect(versionStr.toLowerCase()).toContain('postgis');
    } finally {
      client.release();
    }
  });

  test('should perform table DDL, insert spatial coordinates, and query distance using PostGIS', async () => {
    const client = await pool.connect();
    try {
      // 1. Create temporary test table
      await client.query(`
        CREATE TABLE IF NOT EXISTS test_ports (
          id SERIAL PRIMARY KEY,
          name VARCHAR(100) NOT NULL,
          location GEOMETRY(Point, 4326)
        );
      `);

      // Clear previous test records
      await client.query('TRUNCATE TABLE test_ports');

      // 2. Insert test ports using PostGIS ST_MakePoint (Longitude, Latitude)
      // Mogadishu Port: 45.3333 E, 2.0333 N
      // Berbera Port: 45.0167 E, 10.4333 N
      await client.query(`
        INSERT INTO test_ports (name, location) VALUES
        ('Mogadishu Port', ST_SetSRID(ST_MakePoint(45.3333, 2.0333), 4326)),
        ('Berbera Port', ST_SetSRID(ST_MakePoint(45.0167, 10.4333), 4326));
      `);

      // 3. Query ports count
      const countRes = await client.query('SELECT COUNT(*) FROM test_ports');
      expect(parseInt(countRes.rows[0].count, 10)).toBe(2);

      // 4. Calculate spatial distance using PostGIS ST_DistanceSphere (in meters)
      const distRes = await client.query(`
        SELECT 
          p1.name AS from_port,
          p2.name AS to_port,
          ST_DistanceSphere(p1.location, p2.location) AS distance_meters
        FROM test_ports p1, test_ports p2
        WHERE p1.name = 'Mogadishu Port' AND p2.name = 'Berbera Port';
      `);

      expect(distRes.rows).toHaveLength(1);
      const distKm = distRes.rows[0].distance_meters / 1000;
      expect(distKm).toBeGreaterThan(900);
      expect(distKm).toBeLessThan(1000);

      // 5. Cleanup test table
      await client.query('DROP TABLE IF EXISTS test_ports');
    } finally {
      client.release();
    }
  });

  test('should handle database transactions correctly (BEGIN, ROLLBACK)', async () => {
    const client = await pool.connect();
    try {
      await client.query('CREATE TABLE IF NOT EXISTS test_tx (id INT, val TEXT)');
      await client.query('BEGIN');
      await client.query("INSERT INTO test_tx VALUES (1, 'rollback_me')");
      await client.query('ROLLBACK');

      const res = await client.query('SELECT * FROM test_tx WHERE id = 1');
      expect(res.rows).toHaveLength(0);

      await client.query('DROP TABLE IF EXISTS test_tx');
    } finally {
      client.release();
    }
  });
});
