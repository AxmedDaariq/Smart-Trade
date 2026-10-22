import 'package:flutter/material.dart';
import '../../models/logistics_models.dart';
import '../../services/mock_data_service.dart';
import '../../theme/app_colors.dart';
import '../../widgets/glass_card.dart';
import '../../widgets/status_badge.dart';

class ControlTowerTab extends StatefulWidget {
  const ControlTowerTab({super.key});

  @override
  State<ControlTowerTab> createState() => _ControlTowerTabState();
}

class _ControlTowerTabState extends State<ControlTowerTab> {
  final List<CorridorAlertItem> _alerts = MockDataService.corridorAlerts;
  bool _isRefreshing = false;

  Future<void> _handleRefresh() async {
    setState(() => _isRefreshing = true);
    await Future.delayed(const Duration(milliseconds: 600));
    if (mounted) setState(() => _isRefreshing = false);
  }

  @override
  Widget build(BuildContext context) {
    return ListView(
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
      children: [
        // Radar / Control Tower Hero Card
        GlassCard(
          backgroundColor: AppColors.bgSurface,
          borderColor: AppColors.primaryLight.withValues(alpha: 0.35),
          padding: const EdgeInsets.all(18),
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
                          color: AppColors.dangerBg,
                          borderRadius: BorderRadius.circular(8),
                        ),
                        child: const Icon(Icons.radar, color: AppColors.dangerLight, size: 18),
                      ),
                      const SizedBox(width: 8),
                      const Text(
                        'CORRIDOR COMMAND TOWER',
                        style: TextStyle(
                          color: AppColors.dangerLight,
                          fontSize: 11,
                          fontWeight: FontWeight.w700,
                          letterSpacing: 0.6,
                        ),
                      ),
                    ],
                  ),
                  StatusBadge.success('Satellite Lock (4.2ms)', pulse: true),
                ],
              ),
              const SizedBox(height: 14),
              const Text(
                'Horn of Africa Trade Flow',
                style: TextStyle(
                  color: AppColors.textMain,
                  fontSize: 20,
                  fontWeight: FontWeight.w800,
                  letterSpacing: -0.4,
                ),
              ),
              const SizedBox(height: 4),
              const Text(
                'Berbera Port ⇄ Tog-Wajaale ⇄ Dire Dawa ⇄ Mojo Hub',
                style: TextStyle(
                  color: AppColors.cyanLight,
                  fontSize: 12,
                  fontWeight: FontWeight.w600,
                ),
              ),
              const SizedBox(height: 16),

              // KPI stats row
              Row(
                children: [
                  Expanded(
                    child: Container(
                      padding: const EdgeInsets.all(10),
                      decoration: BoxDecoration(
                        color: AppColors.bgCard,
                        borderRadius: BorderRadius.circular(10),
                        border: Border.all(color: AppColors.borderSubtle),
                      ),
                      child: const Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text('ACTIVE CONVOYS', style: TextStyle(color: AppColors.textMuted, fontSize: 9, fontWeight: FontWeight.w700)),
                          SizedBox(height: 2),
                          Text('38 Units', style: TextStyle(color: AppColors.textMain, fontSize: 14, fontWeight: FontWeight.w800)),
                        ],
                      ),
                    ),
                  ),
                  const SizedBox(width: 8),
                  Expanded(
                    child: Container(
                      padding: const EdgeInsets.all(10),
                      decoration: BoxDecoration(
                        color: AppColors.bgCard,
                        borderRadius: BorderRadius.circular(10),
                        border: Border.all(color: AppColors.borderSubtle),
                      ),
                      child: const Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text('THROUGHPUT', style: TextStyle(color: AppColors.textMuted, fontSize: 9, fontWeight: FontWeight.w700)),
                          SizedBox(height: 2),
                          Text('124 TEU/hr', style: TextStyle(color: AppColors.cyanLight, fontSize: 14, fontWeight: FontWeight.w800)),
                        ],
                      ),
                    ),
                  ),
                  const SizedBox(width: 8),
                  Expanded(
                    child: Container(
                      padding: const EdgeInsets.all(10),
                      decoration: BoxDecoration(
                        color: AppColors.bgCard,
                        borderRadius: BorderRadius.circular(10),
                        border: Border.all(color: AppColors.borderSubtle),
                      ),
                      child: const Column(
                        crossAxisAlignment: CrossAxisAlignment.start,
                        children: [
                          Text('BORDER WAIT', style: TextStyle(color: AppColors.textMuted, fontSize: 9, fontWeight: FontWeight.w700)),
                          SizedBox(height: 2),
                          Text('35 mins', style: TextStyle(color: AppColors.warningLight, fontSize: 14, fontWeight: FontWeight.w800)),
                        ],
                      ),
                    ),
                  ),
                ],
              ),
            ],
          ),
        ),
        const SizedBox(height: 18),

        // Corridor Gateways Status
        const Text(
          'CORRIDOR GATEWAY CLEARANCE STATUS',
          style: TextStyle(
            color: AppColors.textMuted,
            fontSize: 11,
            fontWeight: FontWeight.w700,
            letterSpacing: 0.6,
          ),
        ),
        const SizedBox(height: 10),
        _buildGatewayNode(
          name: 'Berbera Container Terminal (DP World)',
          type: 'Deep Sea Maritime Port',
          status: 'Operating Smoothly',
          statusType: 'success',
          metric: '2,800 TEU MV Maersk Seletar Berthed',
          icon: Icons.anchor,
        ),
        const SizedBox(height: 8),
        _buildGatewayNode(
          name: 'Tog-Wajaale Border Crossing Gate B',
          type: 'Somaliland / Ethiopia Cross-Border Node',
          status: 'Queue Congestion (35m wait)',
          statusType: 'warning',
          metric: 'Optical Scanner Calibration in progress',
          icon: Icons.alt_route,
        ),
        const SizedBox(height: 8),
        _buildGatewayNode(
          name: 'Mojo Multimodal Dryport Terminal',
          type: 'Primary Inland Clearance Depot',
          status: 'High Volume Clearance',
          statusType: 'info',
          metric: 'Average Dwell Time: 2.8 Days',
          icon: Icons.warehouse,
        ),
        const SizedBox(height: 20),

        // Real-time Alerts Feed
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            const Text(
              'LIVE CORRIDOR TELEMETRY ALERTS',
              style: TextStyle(
                color: AppColors.textMuted,
                fontSize: 11,
                fontWeight: FontWeight.w700,
                letterSpacing: 0.6,
              ),
            ),
            InkWell(
              onTap: _handleRefresh,
              child: Row(
                children: [
                  Icon(
                    Icons.sync,
                    size: 14,
                    color: _isRefreshing ? AppColors.cyanLight : AppColors.textMuted,
                  ),
                  const SizedBox(width: 4),
                  Text(
                    _isRefreshing ? 'Syncing...' : 'Live Stream',
                    style: TextStyle(
                      color: _isRefreshing ? AppColors.cyanLight : AppColors.textMuted,
                      fontSize: 11,
                    ),
                  ),
                ],
              ),
            ),
          ],
        ),
        const SizedBox(height: 10),

        ..._alerts.map((alert) => _buildAlertCard(alert)),
        const SizedBox(height: 20),
      ],
    );
  }

  Widget _buildGatewayNode({
    required String name,
    required String type,
    required String status,
    required String statusType,
    required String metric,
    required IconData icon,
  }) {
    Color badgeColor = statusType == 'success'
        ? AppColors.successLight
        : (statusType == 'warning' ? AppColors.warningLight : AppColors.cyanLight);

    return GlassCard(
      padding: const EdgeInsets.all(12),
      child: Row(
        children: [
          Container(
            padding: const EdgeInsets.all(8),
            decoration: BoxDecoration(
              color: badgeColor.withValues(alpha: 0.12),
              borderRadius: BorderRadius.circular(8),
            ),
            child: Icon(icon, size: 18, color: badgeColor),
          ),
          const SizedBox(width: 12),
          Expanded(
            child: Column(
              crossAxisAlignment: CrossAxisAlignment.start,
              children: [
                Text(
                  name,
                  style: const TextStyle(color: AppColors.textMain, fontSize: 12, fontWeight: FontWeight.w700),
                ),
                const SizedBox(height: 2),
                Text(
                  type,
                  style: const TextStyle(color: AppColors.textMuted, fontSize: 10),
                ),
                const SizedBox(height: 4),
                Text(
                  metric,
                  style: TextStyle(color: badgeColor, fontSize: 11, fontWeight: FontWeight.w500),
                ),
              ],
            ),
          ),
          Container(
            padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 3),
            decoration: BoxDecoration(
              color: badgeColor.withValues(alpha: 0.1),
              borderRadius: BorderRadius.circular(6),
              border: Border.all(color: badgeColor.withValues(alpha: 0.3)),
            ),
            child: Text(
              statusType.toUpperCase(),
              style: TextStyle(color: badgeColor, fontSize: 9, fontWeight: FontWeight.w700),
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildAlertCard(CorridorAlertItem alert) {
    Color alertColor = alert.severity == 'critical'
        ? AppColors.dangerLight
        : (alert.severity == 'warning' ? AppColors.warningLight : AppColors.cyanLight);

    return Padding(
      padding: const EdgeInsets.only(bottom: 10),
      child: GlassCard(
        borderColor: alertColor.withValues(alpha: 0.25),
        padding: const EdgeInsets.all(14),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Row(
                  children: [
                    Container(
                      width: 8,
                      height: 8,
                      decoration: BoxDecoration(
                        color: alertColor,
                        shape: BoxShape.circle,
                      ),
                    ),
                    const SizedBox(width: 8),
                    Text(
                      alert.title,
                      style: TextStyle(
                        color: alertColor,
                        fontSize: 13,
                        fontWeight: FontWeight.w700,
                      ),
                    ),
                  ],
                ),
                Text(
                  alert.timestamp,
                  style: const TextStyle(color: AppColors.textMuted, fontSize: 11),
                ),
              ],
            ),
            const SizedBox(height: 6),
            Text(
              alert.location,
              style: const TextStyle(
                color: AppColors.textSecondary,
                fontSize: 11,
                fontWeight: FontWeight.w600,
              ),
            ),
            const SizedBox(height: 4),
            Text(
              alert.description,
              style: const TextStyle(
                color: AppColors.textMuted,
                fontSize: 12,
                height: 1.35,
              ),
            ),
          ],
        ),
      ),
    );
  }
}
