// =============================================================================
// MONGODB ENTERPRISE INDEXING STRATEGY
// Goal: High-speed reads for unstructured logs, audit trails, and caching
// =============================================================================

db = db.getSiblingDB('smarttrade_logs');

// 1. Audit Logs: Fast search by User or Timestamp
db.audit_logs.createIndex({ "userId": 1 }, { name: "idx_audit_user" });
db.audit_logs.createIndex({ "timestamp": -1 }, { name: "idx_audit_time" });

// 2. Notification Cache: Fast lookup for unread messages
db.notifications.createIndex({ "userId": 1, "isRead": 1 }, { name: "idx_user_unread" });

// 3. Search Index: Text search for shipment descriptions
db.shipment_details.createIndex({ "description": "text" }, { name: "idx_shipment_text_search" });

// 4. TTL Index: Automatic deletion of session data after 24 hours
db.sessions.createIndex({ "createdAt": 1 }, { expireAfterSeconds: 86400 });

console.log("MongoDB Enterprise Indexes created successfully.");
