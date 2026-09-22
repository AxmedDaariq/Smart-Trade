# ANPR Redis Documentation

Redis in the ANPR Service provides ephemeral caching to accelerate operational throughput without compromising the authoritative integrity of PostgreSQL.

## Caching Strategies

### Camera Health & Config
- `anpr:camera:{cameraId}:health`
- `anpr:camera:{cameraId}:config`
**Purpose**: Avoid slamming the DB with heartbeat/config queries when evaluating thousands of plate recognition events per minute.

### Gate State
- `anpr:gate:{gateId}:state`
- `anpr:lane:{laneId}:state`
**Purpose**: Rapid validation of physical infrastructure status before producing authorization requests.

### Operations
- `anpr:authorization:{requestId}:state`
- `anpr:plate:{normalizedPlate}:recent`
- `anpr:border:{borderId}:state`
**Purpose**: `anpr:plate:{normalizedPlate}:recent` is critical for real-time duplicate rejection (debouncing duplicate recognitions of a stalled truck). 

## Failure Modes
If Redis falls offline, ANPR shifts query load to PostgreSQL.
**CRITICAL**: We never `FailOpen` gates just because Redis cache misses occur. Default `OperatingMode` is `FailClosed` or `ManualOnly`.
