import { Pool } from 'pg';
import { MongoClient } from 'mongodb';
import Redis from 'ioredis';

export const postgresConfig = {
  host: process.env.POSTGRES_HOST || 'localhost',
  port: parseInt(process.env.POSTGRES_PORT || '5432', 10),
  user: process.env.POSTGRES_USER || 'postgres',
  password: process.env.POSTGRES_PASSWORD || 'Password123!',
  database: process.env.POSTGRES_DB || 'postgres',
};

export const mongoUrl = process.env.MONGO_URL || 'mongodb://localhost:27017';
export const mongoDbName = process.env.MONGO_DB_NAME || 'smart_trade_test';

export const redisConfig = {
  host: process.env.REDIS_HOST || 'localhost',
  port: parseInt(process.env.REDIS_PORT || '6379', 10),
};

export function getPostgresPool(): Pool {
  return new Pool(postgresConfig);
}

export function getMongoClient(): MongoClient {
  return new MongoClient(mongoUrl);
}

export function getRedisClient(): Redis {
  return new Redis(redisConfig);
}
