/**
 * Integration Test: Redis Container
 */

import Redis from 'ioredis';
import { getRedisClient } from '../helpers/dbClient';

describe('Redis Integration Tests', () => {
  let client: Redis;

  beforeAll(async () => {
    client = getRedisClient();
    await client.connect();
  });

  afterAll(async () => {
    if (client) {
      await client.quit();
    }
  });

  test('should ping Redis and receive PONG', async () => {
    const response = await client.ping();
    expect(response).toBe('PONG');
  });

  test('should set and get a string key with TTL', async () => {
    const key = 'test:key';
    await client.set(key, 'value', 'EX', 2); // expires in 2 seconds
    const val = await client.get(key);
    expect(val).toBe('value');
    // wait 3 seconds to ensure expiration
    await new Promise((r) => setTimeout(r, 3000));
    const expired = await client.get(key);
    expect(expired).toBeNull();
  });

  test('should work with hash structures for session data', async () => {
    const hashKey = 'session:123';
    await client.hset(hashKey, { userId: 'u-42', role: 'Importer', ttl: '3600' });
    const hash = await client.hgetall(hashKey);
    expect(hash).toMatchObject({ userId: 'u-42', role: 'Importer', ttl: '3600' });
    await client.del(hashKey);
  });

  test('should publish and receive a message via Pub/Sub', async () => {
    const channel = 'test:channel';
    const message = 'hello-world';
    const subscriber = new Redis();
    await subscriber.subscribe(channel);

    const promise = new Promise<string>((resolve) => {
      subscriber.on('message', (chan, msg) => {
        if (chan === channel) resolve(msg);
      });
    });

    await client.publish(channel, message);
    const received = await promise;
    expect(received).toBe(message);

    await subscriber.unsubscribe(channel);
    await subscriber.quit();
  });
});
