import 'package:flutter/material.dart';
import '../../models/logistics_models.dart';
import '../../services/mock_data_service.dart';
import '../../theme/app_colors.dart';
import '../../widgets/corridor_timeline.dart';
import '../../widgets/glass_card.dart';
import '../../widgets/metric_card.dart';
import '../../widgets/status_badge.dart';
import '../shipment_detail_sheet.dart';

class CockpitTab extends StatefulWidget {
  final VoidCallback onNavigateToEpod;

  const CockpitTab({super.key, required this.onNavigateToEpod});

  @override
  State<CockpitTab> createState() => _CockpitTabState();
}

class _CockpitTabState extends State<CockpitTab> {
  final TelemetryData _telemetry = MockDataService.activeTelemetry;
  final ShipmentItem _activeShipment = MockDataService.shipments.first;

  @override
  Widget build(BuildContext context) {
    return ListView(
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
      children: [
        // Active Mission Header Banner
        GlassCard(
          backgroundColor: AppColors.bgSurface,
          borderColor: AppColors.primaryLight.withValues(alpha: 0.4),
          padding: const EdgeInsets.all(16),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Row(
                    children: [
                      Container(
                        padding: const EdgeInsets.all(6),
                        decoration: BoxDecoration(
                          color: AppColors.primaryGlow,
                          borderRadius: BorderRadius.circular(8),
                        ),
                        child: const Icon(Icons.navigation, color: AppColors.cyanLight, size: 16),
                      ),
                      const SizedBox(width: 8),
                      const Text(
                        'ACTIVE CONVOY MISSION',
                        style: TextStyle(
                          color: AppColors.cyanLight,
                          fontSize: 11,
                          fontWeight: FontWeight.w700,
                          letterSpacing: 0.6,
                        ),
                      ),
                    ],
                  ),
                  StatusBadge.success('Corridor Online', pulse: true),
                ],
              ),
              const SizedBox(height: 12),
              Text(
                _activeShipment.consignee,
                style: const TextStyle(
                  color: AppColors.textMain,
                  fontSize: 18,
                  fontWeight: FontWeight.w800,
                  letterSpacing: -0.3,
                ),
              ),
              const SizedBox(height: 4),
              Text(
                'Waybill: ${_activeShipment.trackingNumber} • ${_activeShipment.containersCount} TEU ISO Containers',
                style: const TextStyle(
                  color: AppColors.textSecondary,
                  fontSize: 12,
                ),
              ),
              const SizedBox(height: 14),
              // Route path
              Container(
                padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 10),
                decoration: BoxDecoration(
                  color: AppColors.bgCard,
                  borderRadius: BorderRadius.circular(10),
                  border: Border.all(color: AppColors.borderSubtle),
                ),
                child: Row(
                  children: [
                    const Icon(Icons.trip_origin, size: 14, color: AppColors.successLight),
                    const SizedBox(width: 6),
                    const Text('Berbera Port', style: TextStyle(color: AppColors.textMain, fontSize: 12, fontWeight: FontWeight.w600)),
                    const Expanded(
                      child: Padding(
                        padding: EdgeInsets.symmetric(horizontal: 8),
                        child: Divider(color: AppColors.borderMedium, thickness: 1.5),
                      ),
                    ),
                    const Icon(Icons.near_me, size: 14, color: AppColors.cyanLight),
                    const SizedBox(width: 6),
                    const Text('Wajaale Gate', style: TextStyle(color: AppColors.cyanLight, fontSize: 12, fontWeight: FontWeight.w700)),
                    const Expanded(
                      child: Padding(
                        padding: EdgeInsets.symmetric(horizontal: 8),
                        child: Divider(color: AppColors.borderMedium, thickness: 1.5),
                      ),
                    ),
                    const Icon(Icons.location_on, size: 14, color: AppColors.dangerLight),
                    const SizedBox(width: 6),
                    const Text('Mojo Hub', style: TextStyle(color: AppColors.textMain, fontSize: 12, fontWeight: FontWeight.w600)),
                  ],
                ),
              ),
              const SizedBox(height: 14),
              Row(
                children: [
                  Expanded(
                    child: ElevatedButton.icon(
                      onPressed: () => ShipmentDetailSheet.show(context, _activeShipment),
                      icon: const Icon(Icons.info_outline, size: 16),
                      label: const Text('Inspect Waybill'),
                      style: ElevatedButton.styleFrom(
                        backgroundColor: AppColors.bgCardHover,
                        foregroundColor: AppColors.textMain,
                        side: const BorderSide(color: AppColors.borderMedium),
                      ),
                    ),
                  ),
                  const SizedBox(width: 10),
                  Expanded(
                    child: ElevatedButton.icon(
                      onPressed: widget.onNavigateToEpod,
                      icon: const Icon(Icons.fact_check, size: 16),
                      label: const Text('Handover e-POD'),
                      style: ElevatedButton.styleFrom(
                        backgroundColor: AppColors.primary,
                      ),
                    ),
                  ),
                ],
              ),
            ],
          ),
        ),
        const SizedBox(height: 18),

        // Live Telemetry Matrix Grid
        const Text(
          'REAL-TIME CABIN & CARGO TELEMETRY',
          style: TextStyle(
            color: AppColors.textMuted,
            fontSize: 11,
            fontWeight: FontWeight.w700,
            letterSpacing: 0.6,
          ),
        ),
        const SizedBox(height: 10),
        GridView.count(
          crossAxisCount: 2,
          shrinkWrap: true,
          physics: const NeverScrollableScrollPhysics(),
          mainAxisSpacing: 10,
          crossAxisSpacing: 10,
          childAspectRatio: 1.45,
          children: [
            MetricCard(
              title: 'GPS Velocity',
              value: '${_telemetry.speedKmh}',
              unit: 'KM/H',
              subtitle: 'Limit: 70 KM/H (Normal)',
              icon: Icons.speed,
              iconColor: AppColors.cyanLight,
              accentColor: AppColors.cyanLight,
            ),
            MetricCard(
              title: 'Reefer Temp',
              value: '${_telemetry.reeferTempCelsius}',
              unit: '°C',
              subtitle: 'Target: ${_telemetry.targetTempCelsius}°C (Optimal)',
              icon: Icons.ac_unit,
              iconColor: AppColors.successLight,
              accentColor: AppColors.successLight,
            ),
            MetricCard(
              title: 'IoT Electronic Seal',
              value: 'LOCKED',
              subtitle: 'ID: ${_activeShipment.eSealNumber}',
              icon: Icons.lock_outline,
              iconColor: AppColors.successLight,
              accentColor: AppColors.successLight,
            ),
            MetricCard(
              title: 'Galileo 5G Telemetry',
              value: 'SYNCED',
              subtitle: 'Lock precision: 3.8m (100%)',
              icon: Icons.satellite_alt,
              iconColor: AppColors.purpleLight,
              accentColor: AppColors.purpleLight,
            ),
          ],
        ),
        const SizedBox(height: 18),

        // Next Waypoint & Navigation Status
        GlassCard(
          padding: const EdgeInsets.all(14),
          borderColor: AppColors.cyanLight.withValues(alpha: 0.3),
          child: Row(
            children: [
              Container(
                padding: const EdgeInsets.all(10),
                decoration: BoxDecoration(
                  color: AppColors.cyanLight.withValues(alpha: 0.12),
                  borderRadius: BorderRadius.circular(10),
                ),
                child: const Icon(Icons.turn_sharp_right, color: AppColors.cyanLight, size: 24),
              ),
              const SizedBox(width: 12),
              Expanded(
                child: Column(
                  crossAxisAlignment: CrossAxisAlignment.start,
                  children: [
                    const Text(
                      'NEXT CORRIDOR WAYPOINT',
                      style: TextStyle(
                        color: AppColors.textMuted,
                        fontSize: 10,
                        fontWeight: FontWeight.w700,
                        letterSpacing: 0.5,
                      ),
                    ),
                    const SizedBox(height: 2),
                    Text(
                      _telemetry.nextWaypoint,
                      style: const TextStyle(
                        color: AppColors.textMain,
                        fontSize: 13,
                        fontWeight: FontWeight.w700,
                      ),
                      maxLines: 1,
                      overflow: TextOverflow.ellipsis,
                    ),
                    const SizedBox(height: 2),
                    Text(
                      'ETA: ${_telemetry.etaNextWaypoint} • ${_telemetry.distanceRemainingKm} km remaining',
                      style: const TextStyle(
                        color: AppColors.cyanLight,
                        fontSize: 11,
                        fontWeight: FontWeight.w500,
                      ),
                    ),
                  ],
                ),
              ),
              IconButton(
                icon: const Icon(Icons.navigation, color: AppColors.cyanLight),
                onPressed: () {
                  ScaffoldMessenger.of(context).showSnackBar(
                    const SnackBar(
                      content: Text('Turn-by-turn corridor guidance refreshed.'),
                      backgroundColor: AppColors.bgCardHover,
                    ),
                  );
                },
              ),
            ],
          ),
        ),
        const SizedBox(height: 18),

        // Corridor Waypoints Timeline
        const Text(
          'CORRIDOR MILESTONES',
          style: TextStyle(
            color: AppColors.textMuted,
            fontSize: 11,
            fontWeight: FontWeight.w700,
            letterSpacing: 0.6,
          ),
        ),
        const SizedBox(height: 12),
        GlassCard(
          padding: const EdgeInsets.all(16),
          child: CorridorTimeline(waypoints: _activeShipment.waypoints),
        ),
        const SizedBox(height: 18),

        // Quick Incident / Delay Trigger
        Row(
          children: [
            Expanded(
              child: OutlinedButton.icon(
                onPressed: () => _showDelayDialog(context),
                icon: const Icon(Icons.traffic, size: 16, color: AppColors.warningLight),
                label: const Text('Report Delay', style: TextStyle(color: AppColors.warningLight, fontSize: 13)),
                style: OutlinedButton.styleFrom(
                  side: const BorderSide(color: AppColors.warningBorder),
                  padding: const EdgeInsets.symmetric(vertical: 12),
                  shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
                ),
              ),
            ),
            const SizedBox(width: 10),
            Expanded(
              child: OutlinedButton.icon(
                onPressed: () => _showSosDialog(context),
                icon: const Icon(Icons.emergency, size: 16, color: AppColors.dangerLight),
                label: const Text('Corridor SOS', style: TextStyle(color: AppColors.dangerLight, fontSize: 13)),
                style: OutlinedButton.styleFrom(
                  side: const BorderSide(color: AppColors.dangerBorder),
                  padding: const EdgeInsets.symmetric(vertical: 12),
                  shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(10)),
                ),
              ),
            ),
          ],
        ),
        const SizedBox(height: 20),
      ],
    );
  }

  void _showDelayDialog(BuildContext context) {
    showDialog(
      context: context,
      builder: (ctx) => AlertDialog(
        backgroundColor: AppColors.bgSurface,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(16),
          side: const BorderSide(color: AppColors.borderMedium),
        ),
        title: const Row(
          children: [
            Icon(Icons.warning_amber_rounded, color: AppColors.warningLight),
            SizedBox(width: 8),
            Text('Report Transit Delay', style: TextStyle(fontSize: 16, fontWeight: FontWeight.w700)),
          ],
        ),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            const Text(
              'Select cause of delay to broadcast to Central Control Tower:',
              style: TextStyle(color: AppColors.textSecondary, fontSize: 13),
            ),
            const SizedBox(height: 12),
            _buildDelayOption(ctx, 'Border Gate Queue Congestion'),
            _buildDelayOption(ctx, 'Weighbridge Calibration Queue'),
            _buildDelayOption(ctx, 'Mechanical / Tire Inspection'),
            _buildDelayOption(ctx, 'Highway Roadwork Detour'),
          ],
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(ctx),
            child: const Text('Cancel', style: TextStyle(color: AppColors.textMuted)),
          ),
        ],
      ),
    );
  }

  Widget _buildDelayOption(BuildContext ctx, String reason) {
    return InkWell(
      onTap: () {
        Navigator.pop(ctx);
        ScaffoldMessenger.of(context).showSnackBar(
          SnackBar(
            content: Text('Delay broadcasted to Control Tower: $reason'),
            backgroundColor: AppColors.warning,
          ),
        );
      },
      borderRadius: BorderRadius.circular(8),
      child: Container(
        margin: const EdgeInsets.symmetric(vertical: 4),
        padding: const EdgeInsets.symmetric(horizontal: 12, vertical: 10),
        decoration: BoxDecoration(
          color: AppColors.bgCard,
          borderRadius: BorderRadius.circular(8),
          border: Border.all(color: AppColors.borderSubtle),
        ),
        child: Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            Text(reason, style: const TextStyle(color: AppColors.textMain, fontSize: 12)),
            const Icon(Icons.chevron_right, size: 16, color: AppColors.textMuted),
          ],
        ),
      ),
    );
  }

  void _showSosDialog(BuildContext context) {
    showDialog(
      context: context,
      builder: (ctx) => AlertDialog(
        backgroundColor: AppColors.bgSurface,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(16),
          side: const BorderSide(color: AppColors.dangerBorder),
        ),
        title: const Row(
          children: [
            Icon(Icons.emergency, color: AppColors.dangerLight),
            SizedBox(width: 8),
            Text('Corridor Emergency SOS', style: TextStyle(fontSize: 16, fontWeight: FontWeight.w700)),
          ],
        ),
        content: const Text(
          'This will transmit an urgent telemetry alert to Port Authority, Highway Patrol, and Fleet Dispatch with your exact satellite coordinates.\n\nAre you sure you want to trigger SOS?',
          style: TextStyle(color: AppColors.textSecondary, fontSize: 13),
        ),
        actions: [
          TextButton(
            onPressed: () => Navigator.pop(ctx),
            child: const Text('Cancel', style: TextStyle(color: AppColors.textMuted)),
          ),
          ElevatedButton(
            onPressed: () {
              Navigator.pop(ctx);
              ScaffoldMessenger.of(context).showSnackBar(
                const SnackBar(
                  content: Text('CRITICAL: SOS Transmitted to Horn Emergency Response Center.'),
                  backgroundColor: AppColors.danger,
                ),
              );
            },
            style: ElevatedButton.styleFrom(backgroundColor: AppColors.danger),
            child: const Text('TRANSMIT SOS'),
          ),
        ],
      ),
    );
  }
}
