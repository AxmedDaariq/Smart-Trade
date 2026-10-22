// ═══════════════════════════════════════════════════════════════════════
// Smart Trade Logistics — Cypress Custom Commands TypeScript Definitions
// ═══════════════════════════════════════════════════════════════════════

declare global {
  namespace Cypress {
    interface Chainable {
      /** Log in via the UI form */
      login(email?: string, password?: string): Chainable<void>;
      /** Programmatic fast login via API */
      loginViaApi(email?: string, password?: string): Chainable<void>;
      /** Logout from the application */
      logout(): Chainable<void>;
      /** Make authenticated backend API request */
      apiRequest(method: string, path: string, body?: any): Chainable<Response<any>>;
      /** Verify backend health */
      apiHealthCheck(): Chainable<void>;
      /** Run PostgreSQL query via task */
      dbQuery(query: string, params?: any[]): Chainable<any[]>;
      /** Seed a database record */
      dbSeed(table: string, data: Record<string, any>): Chainable<any>;
      /** Delete test records */
      dbCleanup(table: string, where: string): Chainable<number>;
      /** Verify a database record exists with expected fields */
      dbVerifyRecord(table: string, where: string, expectedFields: Record<string, any>): Chainable<void>;
      /** Check MongoDB connectivity */
      mongoPing(): Chainable<{ ok: boolean; server?: string; error?: string }>;
      /** Insert unstructured IoT or telematics documents into MongoDB */
      mongoInsert(collection: string, docOrDocs: any): Chainable<{ insertedId?: string; insertedCount?: number; insertedIds?: string[] }>;
      /** Query MongoDB documents with filter, limit, and sort */
      mongoQuery(collection: string, filter?: any, options?: { limit?: number; sort?: any }): Chainable<any[]>;
      /** Delete documents in MongoDB matching filter */
      mongoDeleteMany(collection: string, filter?: any): Chainable<{ deletedCount: number }>;
      /** Count documents in MongoDB matching filter */
      mongoCount(collection: string, filter?: any): Chainable<{ count: number }>;
      /** Check Redis connection ping */
      redisPing(): Chainable<{ ok: boolean; response?: string; error?: string }>;
      /** Set Redis cache key with optional TTL */
      redisSet(key: string, value: any, ttlSeconds?: number | null): Chainable<{ success: boolean; key: string }>;
      /** Get Redis cached value (auto-parsed) */
      redisGet(key: string): Chainable<any>;
      /** Delete Redis cache key */
      redisDel(key: string): Chainable<{ deletedCount: number }>;
      /** Check if Redis key exists */
      redisExists(key: string): Chainable<{ exists: boolean }>;
      /** Flush Redis keys matching pattern */
      redisFlushPattern(pattern: string): Chainable<{ deletedCount: number }>;
      /** Check RabbitMQ broker health & overview */
      rabbitCheckHealth(): Chainable<{ ok: boolean; version?: string; cluster_name?: string; queue_totals?: any; error?: string }>;
      /** Publish message to RabbitMQ exchange */
      rabbitPublish(exchange: string, routingKey: string, message: any): Chainable<{ routed: boolean; details?: any; error?: string }>;
      /** Inspect RabbitMQ queue stats */
      rabbitCheckQueue(queue: string): Chainable<{ exists: boolean; messages?: number; messages_ready?: number; consumers?: number; state?: string; error?: string }>;
      /** Purge messages from RabbitMQ queue */
      rabbitPurgeQueue(queue: string): Chainable<{ purged: boolean; details?: any; error?: string }>;
      /** Mobile perspective API request */
      mobileApiRequest(method: string, path: string, body?: any): Chainable<Response<any>>;
      /** Mobile driver programmatic login */
      mobileLogin(email?: string, password?: string): Chainable<void>;
      /** Select by data-cy */
      dataCy(value: string): Chainable<JQuery<HTMLElement>>;
      /** Assert toast message */
      assertToast(text: string): Chainable<void>;
      /** Wait for API call */
      waitForApi(method: string, pathPattern: string): Chainable<any>;
      /** Intercept API response with fixture */
      interceptApiResponse(method: string, pathPattern: string, statusCode: number, fixture: string): Chainable<void>;
      /** Set mobile device viewport */
      setMobileViewport(): Chainable<void>;
    }
  }
}

export {};
