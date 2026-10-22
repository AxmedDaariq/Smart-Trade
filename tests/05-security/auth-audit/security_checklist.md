# Security Audit Checklist - Smart Trade Logistics

## 1. Authentication & Authorization
- [ ] **JWT Validation**: Ensure tokens are signed with a strong secret and have a short expiration (TTL).
- [ ] **RBAC**: Verify that a 'Driver' cannot access 'Customs' admin APIs.
- [ ] **Password Hashing**: Ensure passwords are hashed with Argon2 or BCrypt (not plaintext or MD5).
- [ ] **MFA**: Verify if Multi-Factor Authentication is required for Admin accounts.

## 2. Input Validation (API Layer)
- [ ] **SQL Injection**: All inputs passed through parameterized queries or ORM (EF Core).
- [ ] **XSS**: All frontend outputs are sanitized to prevent script injection.
- [ ] **Rate Limiting**: APIs must have rate limits to prevent Brute Force attacks on `/login`.
- [ ] **Payload Size**: Enforce maximum request size to prevent DoS via huge JSON payloads.

## 3. Database Security
- [ ] **Least Privilege**: The application user should not have `sysadmin` or `db_owner` roles.
- [ ] **Encryption at Rest**: Sensitive data (Taxes, Passports) should be encrypted in the DB.
- [ ] **Audit Logs**: Every change to a Customs Declaration must be logged with a timestamp and UserID.

## 4. Network & Infrastructure
- [ ] **HTTPS/TLS**: All traffic must be encrypted using TLS 1.2+.
- [ ] **CORS**: Only allow requests from authorized frontend domains.
- [ ] **Secrets Management**: No API keys or DB passwords in `appsettings.json` (use Azure KeyVault or AWS Secrets Manager).
