import 'package:flutter/material.dart';
import '../../models/logistics_models.dart';
import '../../services/mock_data_service.dart';
import '../../theme/app_colors.dart';
import '../../widgets/glass_card.dart';
import '../../widgets/status_badge.dart';

class SyncProfileTab extends StatefulWidget {
  const SyncProfileTab({super.key});

  @override
  State<SyncProfileTab> createState() => _SyncProfileTabState();
}

class _SyncProfileTabState extends State<SyncProfileTab> {
  final DriverProfile _driver = MockDataService.currentDriver;
  bool _isOnline = true;
  int _pendingEvents = 2;
  bool _isSyncing = false;

  Future<void> _triggerManualSync() async {
    setState(() => _isSyncing = true);
    await Future.delayed(const Duration(milliseconds: 1200));
    if (mounted) {
      setState(() {
        _isSyncing = false;
        _pendingEvents = 0;
      });
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Offline queue synchronized with Smart Trade backend (0 pending).'),
          backgroundColor: AppColors.success,
        ),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return ListView(
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
      children: [
        // Driver Identity Hero Card
        GlassCard(
          backgroundColor: AppColors.bgSurface,
          borderColor: AppColors.primaryLight.withValues(alpha: 0.35),
          padding: const EdgeInsets.all(16),
          child: Column(
            children: [
              Row(
                children: [
                  Container(
                    width: 54,
                    height: 54,
                    decoration: BoxDecoration(
                      gradient: const LinearGradient(
                        colors: [AppColors.primary, AppColors.cyanLight],
                        begin: Alignment.topLeft,
                        end: Alignment.bottomRight,
                      ),
                      borderRadius: BorderRadius.circular(16),
                      boxShadow: const [
                        BoxShadow(
                          color: AppColors.primaryGlow,
                          blurRadius: 10,
                          offset: Offset(0, 4),
                        ),
                      ],
                    ),
                    alignment: Alignment.center,
                    child: Text(
                      _driver.avatarInitials,
                      style: const TextStyle(
                        color: Colors.white,
                        fontSize: 20,
                        fontWeight: FontWeight.w800,
                      ),
                    ),
                  ),
                  const SizedBox(width: 14),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        Row(
                          mainAxisAlignment: MainAxisAlignment.spaceBetween,
                          children: [
                            Text(
                              _driver.name,
                              style: const TextStyle(
                                color: AppColors.textMain,
                                fontSize: 16,
                                fontWeight: FontWeight.w800,
                              ),
                            ),
                            StatusBadge.success(_isOnline ? 'Online' : 'Offline', pulse: _isOnline),
                          ],
                        ),
                        const SizedBox(height: 2),
                        Text(
                          'Driver ID: ${_driver.driverId} • Rating: ${_driver.rating} ★',
                          style: const TextStyle(
                            color: AppColors.cyanLight,
                            fontSize: 12,
                            fontWeight: FontWeight.w600,
                          ),
                        ),
                        const SizedBox(height: 2),
                        Text(
                          _driver.licenseClass,
                          style: const TextStyle(
                            color: AppColors.textMuted,
                            fontSize: 11,
                          ),
                        ),
                      ],
                    ),
                  ),
                ],
              ),
              const SizedBox(height: 14),
              const Divider(),
              const SizedBox(height: 10),
              // Organization / Vehicle row
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      const Text('TENANT CONTEXT', style: TextStyle(color: AppColors.textMuted, fontSize: 9, fontWeight: FontWeight.w700)),
                      const SizedBox(height: 2),
                      Text(_driver.tenantName, style: const TextStyle(color: AppColors.textMain, fontSize: 11, fontWeight: FontWeight.w600)),
                    ],
                  ),
                  Column(
                    crossAxisAlignment: CrossAxisAlignment.end,
                    children: [
                      const Text('ASSIGNED TRUCK', style: TextStyle(color: AppColors.textMuted, fontSize: 9, fontWeight: FontWeight.w700)),
                      const SizedBox(height: 2),
                      Text('${_driver.truckPlate} (${_driver.truckModel.split(' ').first})',
                          style: const TextStyle(color: AppColors.textMain, fontSize: 11, fontWeight: FontWeight.w600)),
                    ],
                  ),
                ],
              ),
            ],
          ),
        ),
        const SizedBox(height: 18),

        // Hours of Service (HOS) Regulatory Compliance
        const Text(
          'HOURS OF SERVICE (ELD COMPLIANCE)',
          style: TextStyle(
            color: AppColors.textMuted,
            fontSize: 11,
            fontWeight: FontWeight.w700,
            letterSpacing: 0.6,
          ),
        ),
        const SizedBox(height: 10),
        GlassCard(
          padding: const EdgeInsets.all(14),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  const Text('Driving Clock Remaining:', style: TextStyle(color: AppColors.textSecondary, fontSize: 13)),
                  Text(
                    _driver.hosRemaining,
                    style: const TextStyle(color: AppColors.successLight, fontSize: 15, fontWeight: FontWeight.w800),
                  ),
                ],
              ),
              const SizedBox(height: 8),
              // Progress bar
              ClipRRect(
                borderRadius: BorderRadius.circular(4),
                child: LinearProgressIndicator(
                  value: 0.61,
                  backgroundColor: AppColors.bgSurface,
                  valueColor: const AlwaysStoppedAnimation<Color>(AppColors.successLight),
                  minHeight: 8,
                ),
              ),
              const SizedBox(height: 8),
              const Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  Text('Drive Time: 4h 15m elapsed', style: TextStyle(color: AppColors.textMuted, fontSize: 11)),
                  Text('Shift Limit: 11h 00m', style: TextStyle(color: AppColors.textMuted, fontSize: 11)),
                ],
              ),
            ],
          ),
        ),
        const SizedBox(height: 18),

        // Distributed Offline Sync Engine
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            const Text(
              'OFFLINE SYNCHRONIZATION ENGINE',
              style: TextStyle(
                color: AppColors.textMuted,
                fontSize: 11,
                fontWeight: FontWeight.w700,
                letterSpacing: 0.6,
              ),
            ),
            StatusBadge.purple('$_pendingEvents In Queue'),
          ],
        ),
        const SizedBox(height: 10),
        GlassCard(
          borderColor: _pendingEvents > 0 ? AppColors.purple.withValues(alpha: 0.4) : AppColors.borderSubtle,
          padding: const EdgeInsets.all(16),
          child: Column(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              Row(
                mainAxisAlignment: MainAxisAlignment.spaceBetween,
                children: [
                  const Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Text('Cellular Network Mode', style: TextStyle(color: AppColors.textMain, fontSize: 13, fontWeight: FontWeight.w700)),
                      Text('Simulate remote corridor drop', style: TextStyle(color: AppColors.textMuted, fontSize: 11)),
                    ],
                  ),
                  Switch(
                    value: _isOnline,
                    activeThumbColor: AppColors.cyanLight,
                    onChanged: (val) {
                      setState(() => _isOnline = val);
                      ScaffoldMessenger.of(context).showSnackBar(
                        SnackBar(
                          content: Text(val ? 'Network mode: ONLINE.' : 'Network mode: OFFLINE (Events will queue locally).'),
                          backgroundColor: val ? AppColors.success : AppColors.warning,
                        ),
                      );
                    },
                  ),
                ],
              ),
              const SizedBox(height: 12),
              const Divider(),
              const SizedBox(height: 12),

              // Pending Events detail
              if (_pendingEvents > 0) ...[
                _buildSyncItem(
                  title: 'EVT-9021: Waypoint Check-In',
                  time: '14 mins ago',
                  desc: 'Hargeisa Outer Ring Road weighbridge weight (44.2T)',
                ),
                const SizedBox(height: 8),
                _buildSyncItem(
                  title: 'EVT-9022: Reefer Temp Telemetry',
                  time: '6 mins ago',
                  desc: 'Cold Chain -18.2°C temperature reading captured',
                ),
                const SizedBox(height: 14),
              ] else ...[
                const Padding(
                  padding: EdgeInsets.symmetric(vertical: 8),
                  child: Row(
                    children: [
                      Icon(Icons.check_circle_outline, color: AppColors.successLight, size: 18),
                      SizedBox(width: 8),
                      Text('All local events synced to central database.', style: TextStyle(color: AppColors.textSecondary, fontSize: 12)),
                    ],
                  ),
                ),
                const SizedBox(height: 10),
              ],

              ElevatedButton.icon(
                onPressed: (_pendingEvents > 0 && !_isSyncing) ? _triggerManualSync : null,
                icon: _isSyncing
                    ? const SizedBox(
                        width: 16,
                        height: 16,
                        child: CircularProgressIndicator(strokeWidth: 2, color: Colors.white),
                      )
                    : const Icon(Icons.sync, size: 18),
                label: Text(_isSyncing ? 'Synchronizing Queue...' : 'Force Sync Pending Events'),
                style: ElevatedButton.styleFrom(
                  backgroundColor: AppColors.purple,
                  foregroundColor: Colors.white,
                  minimumSize: const Size.fromHeight(44),
                ),
              ),
            ],
          ),
        ),
        const SizedBox(height: 18),

        // Device & Security Settings
        const Text(
          'TELEMATICS DEVICE & SECURITY',
          style: TextStyle(
            color: AppColors.textMuted,
            fontSize: 11,
            fontWeight: FontWeight.w700,
            letterSpacing: 0.6,
          ),
        ),
        const SizedBox(height: 10),
        GlassCard(
          padding: const EdgeInsets.symmetric(horizontal: 14, vertical: 8),
          child: Column(
            children: [
              ListTile(
                contentPadding: EdgeInsets.zero,
                leading: const Icon(Icons.fingerprint, color: AppColors.cyanLight),
                title: const Text('Biometric Authentication', style: TextStyle(fontSize: 13, fontWeight: FontWeight.w600)),
                subtitle: const Text('Touch ID / Face unlock active', style: TextStyle(fontSize: 11, color: AppColors.textMuted)),
                trailing: const Icon(Icons.check, color: AppColors.successLight, size: 18),
              ),
              const Divider(),
              ListTile(
                contentPadding: EdgeInsets.zero,
                leading: const Icon(Icons.cloud_download_outlined, color: AppColors.cyanLight),
                title: const Text('Offline Map Tiles', style: TextStyle(fontSize: 13, fontWeight: FontWeight.w600)),
                subtitle: const Text('Berbera - Addis Ababa Corridor (240 MB cached)', style: TextStyle(fontSize: 11, color: AppColors.textMuted)),
                trailing: const Icon(Icons.chevron_right, color: AppColors.textMuted),
                onTap: () {},
              ),
              const Divider(),
              ListTile(
                contentPadding: EdgeInsets.zero,
                leading: const Icon(Icons.logout, color: AppColors.dangerLight),
                title: const Text('Sign Out Driver Session', style: TextStyle(fontSize: 13, fontWeight: FontWeight.w600, color: AppColors.dangerLight)),
                onTap: () {
                  ScaffoldMessenger.of(context).showSnackBar(
                    const SnackBar(
                      content: Text('Driver session securely locked.'),
                      backgroundColor: AppColors.bgCardHover,
                    ),
                  );
                },
              ),
            ],
          ),
        ),
        const SizedBox(height: 24),
      ],
    );
  }

  Widget _buildSyncItem({required String title, required String time, required String desc}) {
    return Container(
      padding: const EdgeInsets.all(10),
      decoration: BoxDecoration(
        color: AppColors.bgSurface,
        borderRadius: BorderRadius.circular(8),
        border: Border.all(color: AppColors.borderSubtle),
      ),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Row(
            mainAxisAlignment: MainAxisAlignment.spaceBetween,
            children: [
              Text(title, style: const TextStyle(color: AppColors.cyanLight, fontSize: 11, fontWeight: FontWeight.w700)),
              Text(time, style: const TextStyle(color: AppColors.textMuted, fontSize: 10)),
            ],
          ),
          const SizedBox(height: 2),
          Text(desc, style: const TextStyle(color: AppColors.textSecondary, fontSize: 11)),
        ],
      ),
    );
  }
}
