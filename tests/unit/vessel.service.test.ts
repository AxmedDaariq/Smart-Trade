/**
 * Unit Tests: AIS Vessel Tracking & Geofencing Logic
 */

export interface Coordinates {
  latitude: number;
  longitude: number;
}

export interface VesselPosition {
  mmsi: string;
  name: string;
  speedKnots: number;
  coords: Coordinates;
  timestamp: Date;
}

export class VesselService {
  /**
   * Calculate distance between two coordinates in nautical miles using Haversine formula
   */
  public calculateDistanceNauticalMiles(c1: Coordinates, c2: Coordinates): number {
    const R = 3440.065; // Earth radius in nautical miles
    const dLat = this.toRadians(c2.latitude - c1.latitude);
    const dLon = this.toRadians(c2.longitude - c1.longitude);

    const a =
      Math.sin(dLat / 2) * Math.sin(dLat / 2) +
      Math.cos(this.toRadians(c1.latitude)) *
        Math.cos(this.toRadians(c2.latitude)) *
        Math.sin(dLon / 2) *
        Math.sin(dLon / 2);

    const c = 2 * Math.atan2(Math.sqrt(a), Math.sqrt(1 - a));
    return Math.round(R * c * 100) / 100;
  }

  private toRadians(degrees: number): number {
    return (degrees * Math.PI) / 180;
  }

  /**
   * Estimate Time of Arrival (ETA) in hours based on remaining distance and current speed
   */
  public estimateEtaHours(distanceNauticalMiles: number, speedKnots: number): number | null {
    if (speedKnots <= 0.5) return null; // Vessel stationary
    return Math.round((distanceNauticalMiles / speedKnots) * 10) / 10;
  }

  /**
   * Check if vessel is inside port geofence radius (nautical miles)
   */
  public isInsidePortGeofence(vesselCoords: Coordinates, portCoords: Coordinates, radiusNm: number = 5.0): boolean {
    const dist = this.calculateDistanceNauticalMiles(vesselCoords, portCoords);
    return dist <= radiusNm;
  }

  /**
   * Detect speed anomalies (e.g. speeding in port zone)
   */
  public isSpeedingInPortZone(speedKnots: number, inPortZone: boolean): boolean {
    const PORT_SPEED_LIMIT_KNOTS = 6.0;
    return inPortZone && speedKnots > PORT_SPEED_LIMIT_KNOTS;
  }
}

describe('VesselService Unit Tests', () => {
  let vesselService: VesselService;

  beforeEach(() => {
    vesselService = new VesselService();
  });

  const mogadishuPort: Coordinates = { latitude: 2.0333, longitude: 45.3333 };

  describe('Haversine Distance Calculations', () => {
    test('should calculate distance 0 for identical coordinates', () => {
      const dist = vesselService.calculateDistanceNauticalMiles(mogadishuPort, mogadishuPort);
      expect(dist).toBe(0);
    });

    test('should calculate correct distance between Mogadishu and Berbera ports', () => {
      const berberaPort: Coordinates = { latitude: 10.4333, longitude: 45.0167 };
      const dist = vesselService.calculateDistanceNauticalMiles(mogadishuPort, berberaPort);
      expect(dist).toBeGreaterThan(500);
      expect(dist).toBeLessThan(520);
    });
  });

  describe('ETA Estimation', () => {
    test('should estimate correct ETA hours for moving vessel', () => {
      // 100 nautical miles at 10 knots = 10 hours
      const eta = vesselService.estimateEtaHours(100, 10);
      expect(eta).toBe(10);
    });

    test('should return null for stationary or anchored vessel', () => {
      const eta = vesselService.estimateEtaHours(100, 0.2);
      expect(eta).toBeNull();
    });
  });

  describe('Port Geofence & Speed Anomalies', () => {
    test('should detect vessel inside port geofence (3 nautical miles away)', () => {
      const nearbyVessel: Coordinates = { latitude: 2.05, longitude: 45.34 };
      const isInside = vesselService.isInsidePortGeofence(nearbyVessel, mogadishuPort, 5.0);
      expect(isInside).toBe(true);
    });

    test('should detect vessel outside port geofence (20 nautical miles away)', () => {
      const distantVessel: Coordinates = { latitude: 2.30, longitude: 45.50 };
      const isInside = vesselService.isInsidePortGeofence(distantVessel, mogadishuPort, 5.0);
      expect(isInside).toBe(false);
    });

    test('should flag vessel speeding over 6 knots inside port zone', () => {
      expect(vesselService.isSpeedingInPortZone(12.5, true)).toBe(true);
      expect(vesselService.isSpeedingInPortZone(4.5, true)).toBe(false);
      expect(vesselService.isSpeedingInPortZone(15.0, false)).toBe(false); // Open sea
    });
  });
});
