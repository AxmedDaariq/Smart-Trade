/**
 * Integration Test: MongoDB Database Container
 */

import { MongoClient, Db } from 'mongodb';
import { getMongoClient, mongoDbName } from '../helpers/dbClient';

describe('MongoDB Integration Tests', () => {
  let client: MongoClient;
  let db: Db;

  beforeAll(async () => {
    client = getMongoClient();
    await client.connect();
    db = client.db(mongoDbName);
  });

  afterAll(async () => {
    if (client) {
      await client.close();
    }
  });

  test('should successfully connect to MongoDB container and perform ping command', async () => {
    const pingResult = await db.command({ ping: 1 });
    expect(pingResult.ok).toBe(1);
  });

  test('should perform CRUD operations on logistics telemetry collection', async () => {
    const collection = db.collection('audit_logs');
    await collection.deleteMany({}); // Reset

    // 1. Create / Insert document
    const testLog = {
      action: 'DECLARATION_SUBMITTED',
      declarationId: 'DEC-88912',
      operatorId: 'op-404',
      timestamp: new Date(),
      metadata: {
        ipAddress: '192.168.1.50',
        userAgent: 'JestTestRunner/1.0',
      },
    };

    const insertRes = await collection.insertOne(testLog);
    expect(insertRes.acknowledged).toBe(true);
    expect(insertRes.insertedId).toBeDefined();

    // 2. Read / Find document
    const found = await collection.findOne({ declarationId: 'DEC-88912' });
    expect(found).not.toBeNull();
    expect(found?.action).toBe('DECLARATION_SUBMITTED');
    expect(found?.metadata.ipAddress).toBe('192.168.1.50');

    // 3. Update document
    const updateRes = await collection.updateOne(
      { declarationId: 'DEC-88912' },
      { $set: { action: 'DECLARATION_VERIFIED', 'metadata.status': 'APPROVED' } }
    );
    expect(updateRes.modifiedCount).toBe(1);

    const updated = await collection.findOne({ declarationId: 'DEC-88912' });
    expect(updated?.action).toBe('DECLARATION_VERIFIED');
    expect(updated?.metadata.status).toBe('APPROVED');

    // 4. Delete document
    const deleteRes = await collection.deleteOne({ declarationId: 'DEC-88912' });
    expect(deleteRes.deletedCount).toBe(1);
  });

  test('should support creating compound indexes on document fields', async () => {
    const collection = db.collection('vessel_telemetry');
    
    // Create index
    const indexName = await collection.createIndex({ mmsi: 1, timestamp: -1 });
    expect(typeof indexName).toBe('string');

    // Verify index exists
    const indexes = await collection.indexes();
    const hasMmsiIndex = indexes.some((idx) => idx.name === indexName);
    expect(hasMmsiIndex).toBe(true);

    await collection.drop();
  });
});
