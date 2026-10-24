const { defineConfig } = require('cypress');
const { Client } = require('pg');

module.exports = defineConfig({
  // ─────────────────────────────── Global ───────────────────────────────
  projectId: 'smart-trade-e2e',
  viewportWidth: 1440,
  viewportHeight: 900,
  video: false,                     // disabled in CI (enabled via --config override locally)
  screenshotOnRunFailure: true,
  watchForFileChanges: false,       // must be false for headless / CI mode
  defaultCommandTimeout: 12000,
  requestTimeout: 15000,
  responseTimeout: 30000,
  pageLoadTimeout: 30000,
  numTestsKeptInMemory: 10,         // prevents memory exhaustion in long CI runs
  experimentalMemoryManagement: true,
  retries: {
    runMode: 2,    // CI retries
    openMode: 0,   // Dev retries
  },

  // Reporter – Mochawesome for HTML reports
  reporter: 'cypress-mochawesome-reporter',
  reporterOptions: {
    reportDir: 'cypress/reports',
    charts: true,
    reportPageTitle: 'Smart Trade E2E Report',
    embeddedScreenshots: true,
    inlineAssets: true,
    saveAllAttempts: false,
    overwrite: false,
    html: true,
    json: true,
  },

  // ─────────────────────────────── E2E ──────────────────────────────────
  e2e: {
    baseUrl: process.env.CYPRESS_BASE_URL || 'http://localhost:5173',
    specPattern: 'cypress/e2e/**/*.cy.{js,ts}',
    supportFile: 'cypress/support/e2e.js',
    fixturesFolder: 'cypress/fixtures',

    // ── Ensure reports dir exists before run ──
    setupNodeEvents(on, config) {
      const fs = require('fs');
      const path = require('path');

      // Create reports directory if missing
      const reportsDir = path.join(__dirname, 'cypress', 'reports');
      if (!fs.existsSync(reportsDir)) fs.mkdirSync(reportsDir, { recursive: true });

      // ─── Mochawesome reporter ───
      try {
        require('cypress-mochawesome-reporter/plugin')(on);
      } catch (_) {
        // Reporter not installed – skip silently
      }

      // ─── Environment variables (CI / local) ───
      config.env.API_URL       = process.env.API_URL       ?? 'http://localhost:5000';
      config.env.IDENTITY_API  = process.env.IDENTITY_API  ?? config.env.API_URL;
      config.env.GATEWAY_URL   = process.env.GATEWAY_URL   ?? 'http://localhost:8080';
      config.env.API_GATEWAY   = process.env.API_GATEWAY   ?? config.env.GATEWAY_URL;
      config.env.MOBILE_API    = process.env.MOBILE_API    ?? 'http://localhost:5000';
      config.env.TENANT_CODE   = process.env.TENANT_CODE   ?? 'ACME';
      config.env.ADMIN_EMAIL   = process.env.ADMIN_EMAIL   ?? 'admin@acme.com';
      config.env.ADMIN_PASS    = process.env.ADMIN_PASS    ?? 'Admin@123456!';
      config.env.DRIVER_EMAIL  = process.env.DRIVER_EMAIL  ?? 'driver_test@smarttrade.com';
      config.env.DRIVER_PASS   = process.env.DRIVER_PASS   ?? 'DriverPass123!';

      // ─── Database connection (PostgreSQL) ───
      config.env.DB_HOST       = process.env.DB_HOST       ?? 'localhost';
      config.env.DB_PORT       = process.env.DB_PORT       ?? '5432';
      config.env.DB_USER       = process.env.DB_USER       ?? 'postgres';
      config.env.DB_PASS       = process.env.DB_PASS       ?? 'postgres';
      config.env.DB_NAME       = process.env.DB_NAME       ?? 'smart_trade';

      // ─── MongoDB connection ───
      config.env.MONGO_URL     = process.env.MONGO_URL     ?? 'mongodb://localhost:27017/smart_trade';

      // ─── Redis connection ───
      config.env.REDIS_URL     = process.env.REDIS_URL     ?? 'redis://localhost:6379';

      // ─── RabbitMQ connection ───
      config.env.RABBITMQ_URL  = process.env.RABBITMQ_URL  ?? 'amqp://guest:guest@localhost:5672';
      config.env.RABBITMQ_API  = process.env.RABBITMQ_API  ?? 'http://localhost:15672/api';
      config.env.RABBITMQ_USER = process.env.RABBITMQ_USER ?? 'guest';
      config.env.RABBITMQ_PASS = process.env.RABBITMQ_PASS ?? 'guest';

      // ───────────────── Node Tasks ─────────────────
      on('task', {

        // ═════════════════════ POSTGRESQL ═════════════════════

        // ── PostgreSQL: run any query and return rows ──
        async queryDatabase({ query, params = [] }) {
          const client = new Client({
            host:     config.env.DB_HOST,
            port:     parseInt(config.env.DB_PORT),
            user:     config.env.DB_USER,
            password: config.env.DB_PASS,
            database: config.env.DB_NAME,
            connectionTimeoutMillis: 5000,
            ssl:      process.env.DB_SSL === 'true' ? { rejectUnauthorized: false } : false,
          });
          await client.connect();
          try {
            const result = await client.query(query, params);
            return result.rows;
          } finally {
            await client.end();
          }
        },

        // ── Seed: insert a test record (returns inserted id) ──
        async seedRecord({ table, data }) {
          const client = new Client({
            host:     config.env.DB_HOST,
            port:     parseInt(config.env.DB_PORT),
            user:     config.env.DB_USER,
            password: config.env.DB_PASS,
            database: config.env.DB_NAME,
            connectionTimeoutMillis: 5000,
            ssl:      process.env.DB_SSL === 'true' ? { rejectUnauthorized: false } : false,
          });
          await client.connect();
          try {
            const keys = Object.keys(data);
            const vals = Object.values(data);
            const placeholders = keys.map((_, i) => `$${i + 1}`).join(', ');
            const sql = `INSERT INTO ${table} (${keys.join(', ')}) VALUES (${placeholders}) RETURNING *`;
            const result = await client.query(sql, vals);
            return result.rows[0];
          } finally {
            await client.end();
          }
        },

        // ── Cleanup: delete test data by a WHERE clause ──
        async cleanupDatabase({ table, where }) {
          const client = new Client({
            host:     config.env.DB_HOST,
            port:     parseInt(config.env.DB_PORT),
            user:     config.env.DB_USER,
            password: config.env.DB_PASS,
            database: config.env.DB_NAME,
            connectionTimeoutMillis: 5000,
            ssl:      process.env.DB_SSL === 'true' ? { rejectUnauthorized: false } : false,
          });
          await client.connect();
          try {
            const sql = `DELETE FROM ${table} WHERE ${where}`;
            const result = await client.query(sql);
            return result.rowCount;
          } finally {
            await client.end();
          }
        },

        // ═════════════════════ MONGODB (TELEMATICS & IOT) ═════════════════════

        // ── MongoDB: Ping / Health check ──
        async mongoPing() {
          const { MongoClient } = require('mongodb');
          const client = new MongoClient(config.env.MONGO_URL, { serverSelectionTimeoutMS: 4000 });
          try {
            await client.connect();
            const pingRes = await client.db().admin().ping();
            return { ok: pingRes.ok === 1, server: config.env.MONGO_URL };
          } catch (err) {
            return { ok: false, error: err.message };
          } finally {
            await client.close().catch(() => {});
          }
        },

        // ── MongoDB: Insert unstructured telematics document(s) ──
        async mongoInsert({ collection, doc, docs }) {
          const { MongoClient } = require('mongodb');
          const client = new MongoClient(config.env.MONGO_URL, { serverSelectionTimeoutMS: 4000 });
          try {
            await client.connect();
            const col = client.db().collection(collection);
            if (docs && Array.isArray(docs)) {
              const res = await col.insertMany(docs);
              return { insertedCount: res.insertedCount, insertedIds: Object.values(res.insertedIds).map(id => id.toString()) };
            } else {
              const res = await col.insertOne(doc);
              return { insertedId: res.insertedId.toString() };
            }
          } finally {
            await client.close().catch(() => {});
          }
        },

        // ── MongoDB: Query telematics collection ──
        async mongoQuery({ collection, filter = {}, limit = 50, sort = {} }) {
          const { MongoClient } = require('mongodb');
          const client = new MongoClient(config.env.MONGO_URL, { serverSelectionTimeoutMS: 4000 });
          try {
            await client.connect();
            const col = client.db().collection(collection);
            const results = await col.find(filter).sort(sort).limit(limit).toArray();
            return results.map(doc => ({ ...doc, _id: doc._id.toString() }));
          } finally {
            await client.close().catch(() => {});
          }
        },

        // ── MongoDB: Cleanup documents by filter ──
        async mongoDeleteMany({ collection, filter = {} }) {
          const { MongoClient } = require('mongodb');
          const client = new MongoClient(config.env.MONGO_URL, { serverSelectionTimeoutMS: 4000 });
          try {
            await client.connect();
            const col = client.db().collection(collection);
            const res = await col.deleteMany(filter);
            return { deletedCount: res.deletedCount };
          } finally {
            await client.close().catch(() => {});
          }
        },

        // ── MongoDB: Count documents ──
        async mongoCount({ collection, filter = {} }) {
          const { MongoClient } = require('mongodb');
          const client = new MongoClient(config.env.MONGO_URL, { serverSelectionTimeoutMS: 4000 });
          try {
            await client.connect();
            const col = client.db().collection(collection);
            const count = await col.countDocuments(filter);
            return { count };
          } finally {
            await client.close().catch(() => {});
          }
        },

        // ── MongoDB: Create Index (e.g. 2dsphere for GPS coordinates) ──
        async mongoCreateIndex({ collection, spec, options = {} }) {
          const { MongoClient } = require('mongodb');
          const client = new MongoClient(config.env.MONGO_URL, { serverSelectionTimeoutMS: 4000 });
          try {
            await client.connect();
            const col = client.db().collection(collection);
            const indexName = await col.createIndex(spec, options);
            return { indexName };
          } finally {
            await client.close().catch(() => {});
          }
        },

        // ═════════════════════ REDIS (CACHE & LIVE STATE) ═════════════════════

        // ── Redis: Ping / Connection test ──
        async redisPing() {
          try {
            const Redis = require('ioredis');
            const redis = new Redis(config.env.REDIS_URL, {
              maxRetriesPerRequest: 1,
              connectTimeout: 3000,
              lazyConnect: true,
              enableAutoPipelining: false,
            });
            await redis.connect();
            const pong = await redis.ping();
            await redis.disconnect();
            return { ok: pong === 'PONG', response: pong };
          } catch (err) {
            return { ok: false, error: err.message };
          }
        },

        // ── Redis: Set key value with optional TTL ──
        async redisSet({ key, value, ttlSeconds }) {
          const Redis = require('ioredis');
          const redis = new Redis(config.env.REDIS_URL, {
            maxRetriesPerRequest: 1,
            connectTimeout: 3000,
            lazyConnect: true,
          });
          try {
            await redis.connect();
            const strVal = typeof value === 'object' ? JSON.stringify(value) : String(value);
            if (ttlSeconds && Number.isInteger(ttlSeconds)) {
              await redis.set(key, strVal, 'EX', ttlSeconds);
            } else {
              await redis.set(key, strVal);
            }
            return { success: true, key };
          } finally {
            redis.disconnect();
          }
        },

        // ── Redis: Get key value (auto-parses JSON) ──
        async redisGet({ key }) {
          const Redis = require('ioredis');
          const redis = new Redis(config.env.REDIS_URL, {
            maxRetriesPerRequest: 1,
            connectTimeout: 3000,
            lazyConnect: true,
          });
          try {
            await redis.connect();
            const raw = await redis.get(key);
            if (raw === null) return null;
            try { return JSON.parse(raw); } catch (_) { return raw; }
          } finally {
            redis.disconnect();
          }
        },

        // ── Redis: Delete key ──
        async redisDel({ key }) {
          const Redis = require('ioredis');
          const redis = new Redis(config.env.REDIS_URL, {
            maxRetriesPerRequest: 1,
            connectTimeout: 3000,
            lazyConnect: true,
          });
          try {
            await redis.connect();
            const deleted = await redis.del(key);
            return { deletedCount: deleted };
          } finally {
            redis.disconnect();
          }
        },

        // ── Redis: Check key existence ──
        async redisExists({ key }) {
          const Redis = require('ioredis');
          const redis = new Redis(config.env.REDIS_URL, {
            maxRetriesPerRequest: 1,
            connectTimeout: 3000,
            lazyConnect: true,
          });
          try {
            await redis.connect();
            const exists = await redis.exists(key);
            return { exists: exists === 1 };
          } finally {
            redis.disconnect();
          }
        },

        // ── Redis: Flush keys matching pattern ──
        async redisFlushPattern({ pattern }) {
          const Redis = require('ioredis');
          const redis = new Redis(config.env.REDIS_URL, {
            maxRetriesPerRequest: 1,
            connectTimeout: 3000,
            lazyConnect: true,
          });
          try {
            await redis.connect();
            const keys = await redis.keys(pattern);
            let count = 0;
            if (keys.length > 0) { count = await redis.del(...keys); }
            return { deletedCount: count };
          } finally {
            redis.disconnect();
          }
        },

        // ═════════════════════ RABBITMQ (EVENT STREAMING) ═════════════════════

        // ── RabbitMQ: Health / Overview check ──
        async rabbitCheckHealth() {
          const axios = require('axios');
          try {
            const res = await axios.get(`${config.env.RABBITMQ_API}/overview`, {
              auth: { username: config.env.RABBITMQ_USER, password: config.env.RABBITMQ_PASS },
              timeout: 4000,
            });
            return {
              ok: true,
              version: res.data.rabbitmq_version,
              cluster_name: res.data.cluster_name,
              queue_totals: res.data.queue_totals || {},
            };
          } catch (err) {
            return { ok: false, error: err.message };
          }
        },

        // ── RabbitMQ: Publish message to exchange ──
        async rabbitPublish({ exchange = '', routingKey = '', message = {} }) {
          const axios = require('axios');
          const payload = {
            properties: { content_type: 'application/json', delivery_mode: 2 },
            routing_key: routingKey,
            payload: typeof message === 'string' ? message : JSON.stringify(message),
            payload_encoding: 'string',
          };
          const targetExchange = exchange ? encodeURIComponent(exchange) : 'amq.default';
          try {
            const res = await axios.post(
              `${config.env.RABBITMQ_API}/exchanges/%2f/${targetExchange}/publish`,
              payload,
              { auth: { username: config.env.RABBITMQ_USER, password: config.env.RABBITMQ_PASS }, timeout: 4000 }
            );
            return { routed: res.data.routed, details: res.data };
          } catch (err) {
            return { routed: false, error: err.message };
          }
        },

        // ── RabbitMQ: Check queue stats & existence ──
        async rabbitCheckQueue({ queue }) {
          const axios = require('axios');
          try {
            const res = await axios.get(
              `${config.env.RABBITMQ_API}/queues/%2f/${encodeURIComponent(queue)}`,
              { auth: { username: config.env.RABBITMQ_USER, password: config.env.RABBITMQ_PASS }, timeout: 4000 }
            );
            return {
              exists: true,
              messages: res.data.messages,
              messages_ready: res.data.messages_ready,
              consumers: res.data.consumers,
              state: res.data.state,
            };
          } catch (err) {
            if (err.response && err.response.status === 404) {
              return { exists: false, error: 'Queue not found' };
            }
            return { exists: false, error: err.message };
          }
        },

        // ── RabbitMQ: Purge queue messages ──
        async rabbitPurgeQueue({ queue }) {
          const axios = require('axios');
          try {
            const res = await axios.delete(
              `${config.env.RABBITMQ_API}/queues/%2f/${encodeURIComponent(queue)}/contents`,
              { auth: { username: config.env.RABBITMQ_USER, password: config.env.RABBITMQ_PASS }, timeout: 4000 }
            );
            return { purged: true, details: res.data };
          } catch (err) {
            return { purged: false, error: err.message };
          }
        },

        // ── Log helper (outputs to terminal) ──
        log(message) {
          console.log(`  [E2E] ${message}`);
          return null;
        },
      });

      return config;
    },
  },
});