import 'package:flutter/material.dart';
import '../../models/logistics_models.dart';
import '../../services/mock_data_service.dart';
import '../../theme/app_colors.dart';
import '../../widgets/glass_card.dart';
import '../../widgets/status_badge.dart';
import '../shipment_detail_sheet.dart';

class ManifestTab extends StatefulWidget {
  const ManifestTab({super.key});

  @override
  State<ManifestTab> createState() => _ManifestTabState();
}

class _ManifestTabState extends State<ManifestTab> {
  String _selectedFilter = 'All';
  String _searchQuery = '';
  final List<ShipmentItem> _allShipments = MockDataService.shipments;

  List<ShipmentItem> get _filteredShipments {
    return _allShipments.where((item) {
      final matchesFilter = _selectedFilter == 'All' ||
          item.status.toLowerCase().contains(_selectedFilter.toLowerCase());
      final matchesSearch = _searchQuery.isEmpty ||
          item.trackingNumber.toLowerCase().contains(_searchQuery.toLowerCase()) ||
          item.consignee.toLowerCase().contains(_searchQuery.toLowerCase()) ||
          item.destination.toLowerCase().contains(_searchQuery.toLowerCase()) ||
          item.cargoType.toLowerCase().contains(_searchQuery.toLowerCase());
      return matchesFilter && matchesSearch;
    }).toList();
  }

  @override
  Widget build(BuildContext context) {
    return Column(
      children: [
        // Search & Filter Header
        Padding(
          padding: const EdgeInsets.fromLTRB(16, 12, 16, 8),
          child: Column(
            children: [
              // Search Input
              TextField(
                onChanged: (val) => setState(() => _searchQuery = val),
                decoration: InputDecoration(
                  hintText: 'Search waybill, consignee, or cargo...',
                  prefixIcon: const Icon(Icons.search, color: AppColors.textMuted, size: 20),
                  suffixIcon: _searchQuery.isNotEmpty
                      ? IconButton(
                          icon: const Icon(Icons.clear, size: 18, color: AppColors.textMuted),
                          onPressed: () => setState(() => _searchQuery = ''),
                        )
                      : null,
                ),
              ),
              const SizedBox(height: 10),
              // Filter Chips
              SingleChildScrollView(
                scrollDirection: Axis.horizontal,
                child: Row(
                  children: [
                    _buildFilterChip('All', _allShipments.length),
                    _buildFilterChip('In-Transit', 2),
                    _buildFilterChip('Customs Cleared', 1),
                    _buildFilterChip('Border Gate', 1),
                  ],
                ),
              ),
            ],
          ),
        ),

        // List of Shipments
        Expanded(
          child: _filteredShipments.isEmpty
              ? const Center(
                  child: Text(
                    'No consignments matching criteria.',
                    style: TextStyle(color: AppColors.textMuted),
                  ),
                )
              : ListView.builder(
                  padding: const EdgeInsets.fromLTRB(16, 4, 16, 20),
                  itemCount: _filteredShipments.length,
                  itemBuilder: (context, index) {
                    final item = _filteredShipments[index];
                    return _buildShipmentCard(context, item);
                  },
                ),
        ),
      ],
    );
  }

  Widget _buildFilterChip(String label, int count) {
    final isSelected = _selectedFilter == label;
    return Padding(
      padding: const EdgeInsets.only(right: 8),
      child: ChoiceChip(
        label: Text('$label ($count)'),
        selected: isSelected,
        onSelected: (selected) {
          if (selected) setState(() => _selectedFilter = label);
        },
        selectedColor: AppColors.primary,
        backgroundColor: AppColors.bgSurface,
        labelStyle: TextStyle(
          color: isSelected ? Colors.white : AppColors.textSecondary,
          fontSize: 12,
          fontWeight: isSelected ? FontWeight.w700 : FontWeight.w500,
        ),
        side: BorderSide(
          color: isSelected ? AppColors.primaryLight : AppColors.borderSubtle,
        ),
        shape: RoundedRectangleBorder(borderRadius: BorderRadius.circular(20)),
      ),
    );
  }

  Widget _buildShipmentCard(BuildContext context, ShipmentItem item) {
    return Padding(
      padding: const EdgeInsets.only(bottom: 12),
      child: GlassCard(
        onTap: () => ShipmentDetailSheet.show(context, item),
        padding: const EdgeInsets.all(16),
        child: Column(
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            // Top Row: Tracking & Status Badge
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Row(
                  children: [
                    Container(
                      padding: const EdgeInsets.all(6),
                      decoration: BoxDecoration(
                        color: AppColors.primaryGlow,
                        borderRadius: BorderRadius.circular(6),
                      ),
                      child: const Icon(Icons.local_shipping, size: 14, color: AppColors.cyanLight),
                    ),
                    const SizedBox(width: 8),
                    Text(
                      item.trackingNumber,
                      style: const TextStyle(
                        color: AppColors.cyanLight,
                        fontSize: 12,
                        fontWeight: FontWeight.w700,
                        letterSpacing: 0.3,
                      ),
                    ),
                  ],
                ),
                _buildStatusBadge(item.status),
              ],
            ),
            const SizedBox(height: 10),

            // Consignee & Cargo
            Text(
              item.consignee,
              style: const TextStyle(
                color: AppColors.textMain,
                fontSize: 15,
                fontWeight: FontWeight.w700,
              ),
            ),
            const SizedBox(height: 2),
            Text(
              '${item.cargoType} • ${item.weight}',
              style: const TextStyle(
                color: AppColors.textSecondary,
                fontSize: 12,
              ),
            ),
            const SizedBox(height: 12),

            // Route representation
            Container(
              padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 8),
              decoration: BoxDecoration(
                color: AppColors.bgSurface,
                borderRadius: BorderRadius.circular(8),
                border: Border.all(color: AppColors.borderSubtle),
              ),
              child: Row(
                children: [
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.start,
                      children: [
                        const Text('FROM', style: TextStyle(color: AppColors.textMuted, fontSize: 9, fontWeight: FontWeight.w700)),
                        Text(
                          item.origin,
                          style: const TextStyle(color: AppColors.textMain, fontSize: 11, fontWeight: FontWeight.w500),
                          maxLines: 1,
                          overflow: TextOverflow.ellipsis,
                        ),
                      ],
                    ),
                  ),
                  const Padding(
                    padding: EdgeInsets.symmetric(horizontal: 8),
                    child: Icon(Icons.arrow_forward, size: 14, color: AppColors.textMuted),
                  ),
                  Expanded(
                    child: Column(
                      crossAxisAlignment: CrossAxisAlignment.end,
                      children: [
                        const Text('TO', style: TextStyle(color: AppColors.textMuted, fontSize: 9, fontWeight: FontWeight.w700)),
                        Text(
                          item.destination,
                          style: const TextStyle(color: AppColors.textMain, fontSize: 11, fontWeight: FontWeight.w500),
                          maxLines: 1,
                          overflow: TextOverflow.ellipsis,
                        ),
                      ],
                    ),
                  ),
                ],
              ),
            ),
            const SizedBox(height: 12),

            // Bottom Footer
            Row(
              mainAxisAlignment: MainAxisAlignment.spaceBetween,
              children: [
                Row(
                  children: [
                    const Icon(Icons.inventory_2_outlined, size: 14, color: AppColors.textMuted),
                    const SizedBox(width: 4),
                    Text(
                      '${item.containersCount} Containers',
                      style: const TextStyle(color: AppColors.textSecondary, fontSize: 11),
                    ),
                  ],
                ),
                Row(
                  children: [
                    Text(
                      'ETA: ${item.eta}',
                      style: const TextStyle(
                        color: AppColors.cyanLight,
                        fontSize: 11,
                        fontWeight: FontWeight.w600,
                      ),
                    ),
                    const SizedBox(width: 4),
                    const Icon(Icons.chevron_right, size: 16, color: AppColors.textMuted),
                  ],
                ),
              ],
            ),
          ],
        ),
      ),
    );
  }

  Widget _buildStatusBadge(String status) {
    switch (status) {
      case 'Customs Cleared':
        return StatusBadge.success('Cleared', icon: Icons.check_circle);
      case 'In-Transit':
        return StatusBadge.info('In-Transit', icon: Icons.local_shipping, pulse: true);
      case 'Border Gate':
        return StatusBadge.warning('Border Inspection', icon: Icons.warning);
      default:
        return StatusBadge.info('Port Yard', icon: Icons.anchor);
    }
  }
}
