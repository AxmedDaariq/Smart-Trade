# Lock Policy

Locks must use `IDistributedLock` abstraction to safely coordinate access.

- Unique tokens and owner verification must be used.
- Atomic `NX` setting must enforce lock exclusivity.
- Do NOT issue bare `DEL` commands without confirming ownership token matches.
- All locks require aggressive TTLs. A crashed application must never permanently dead-lock a resource.
