class WaypointItem {
  final String title;
  final String location;
  final String status; // 'completed', 'active', 'pending'
  final String timestamp;
  final String? note;

  const WaypointItem({
    required this.title,
    required this.location,
    required this.status,
    required this.timestamp,
    this.note,
  });
}

class ShipmentItem {
  final String id;
  final String trackingNumber;
  final String consignee;
  final String origin;
  final String destination;
  final int containersCount;
  final String containerCodes;
  final String cargoType;
  final String weight;
  final String status; // 'In-Transit', 'Customs Cleared', 'Port Discharged', 'Border Gate'
  final String eta;
  final String riskLevel; // 'Low', 'Medium', 'Critical'
  final String eSealNumber;
  final String declarationRef;
  final String demurrageCountdown;
  final List<WaypointItem> waypoints;

  const ShipmentItem({
    required this.id,
    required this.trackingNumber,
    required this.consignee,
    required this.origin,
    required this.destination,
    required this.containersCount,
    required this.containerCodes,
    required this.cargoType,
    required this.weight,
    required this.status,
    required this.eta,
    required this.riskLevel,
    required this.eSealNumber,
    required this.declarationRef,
    required this.demurrageCountdown,
    required this.waypoints,
  });
}

class CorridorAlertItem {
  final String id;
  final String title;
  final String location;
  final String timestamp;
  final String description;
  final String severity; // 'critical', 'warning', 'info'
  final String iconCode;

  const CorridorAlertItem({
    required this.id,
    required this.title,
    required this.location,
    required this.timestamp,
    required this.description,
    required this.severity,
    required this.iconCode,
  });
}

class TelemetryData {
  final int speedKmh;
  final double reeferTempCelsius;
  final double targetTempCelsius;
  final int fuelPercent;
  final int batteryPercent;
  final String gpsLockStatus;
  final bool isESealSecured;
  final String currentCorridor;
  final int distanceRemainingKm;
  final String nextWaypoint;
  final String etaNextWaypoint;

  const TelemetryData({
    required this.speedKmh,
    required this.reeferTempCelsius,
    required this.targetTempCelsius,
    required this.fuelPercent,
    required this.batteryPercent,
    required this.gpsLockStatus,
    required this.isESealSecured,
    required this.currentCorridor,
    required this.distanceRemainingKm,
    required this.nextWaypoint,
    required this.etaNextWaypoint,
  });
}

class DriverProfile {
  final String driverId;
  final String name;
  final String avatarInitials;
  final String tenantName;
  final String tenantCode;
  final String truckPlate;
  final String truckModel;
  final String licenseClass;
  final String hosRemaining; // Hours of service
  final double rating;
  final int completedTrips;
  final bool isOnline;
  final int pendingSyncCount;

  const DriverProfile({
    required this.driverId,
    required this.name,
    required this.avatarInitials,
    required this.tenantName,
    required this.tenantCode,
    required this.truckPlate,
    required this.truckModel,
    required this.licenseClass,
    required this.hosRemaining,
    required this.rating,
    required this.completedTrips,
    required this.isOnline,
    required this.pendingSyncCount,
  });
}
