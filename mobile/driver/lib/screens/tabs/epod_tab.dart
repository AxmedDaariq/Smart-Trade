import 'package:flutter/material.dart';
import '../../models/logistics_models.dart';
import '../../services/mock_data_service.dart';
import '../../theme/app_colors.dart';
import '../../widgets/glass_card.dart';
import '../../widgets/signature_pad.dart';
import '../../widgets/status_badge.dart';

class EpodTab extends StatefulWidget {
  const EpodTab({super.key});

  @override
  State<EpodTab> createState() => _EpodTabState();
}

class _EpodTabState extends State<EpodTab> {
  final GlobalKey<SignaturePadState> _sigKey = GlobalKey<SignaturePadState>();
  final ShipmentItem _selectedShipment = MockDataService.shipments.first;

  // Handover checklist
  bool _chkSealIntact = true;
  bool _chkCargoIntact = true;
  bool _chkTempVerified = true;
  bool _chkCustomsStamp = true;

  // Recipient details
  final TextEditingController _receiverNameCtrl = TextEditingController(text: 'Farhan Omar Guleed');
  final TextEditingController _receiverPhoneCtrl = TextEditingController(text: '+251 91 144 8291');
  final TextEditingController _receiverTerminalCtrl = TextEditingController(text: 'Mojo Dryport Warehouse Gate 3');

  bool _hasSignature = false;
  int _photosAttached = 2;

  @override
  void dispose() {
    _receiverNameCtrl.dispose();
    _receiverPhoneCtrl.dispose();
    _receiverTerminalCtrl.dispose();
    super.dispose();
  }

  void _handleSubmit() {
    if (!_chkSealIntact || !_chkCargoIntact || !_chkTempVerified || !_chkCustomsStamp) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Please verify all mandatory safety & inspection items.'),
          backgroundColor: AppColors.danger,
        ),
      );
      return;
    }

    if (!_hasSignature) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Recipient digital signature is required for e-POD certification.'),
          backgroundColor: AppColors.warning,
        ),
      );
      return;
    }

    showDialog(
      context: context,
      builder: (ctx) => AlertDialog(
        backgroundColor: AppColors.bgSurface,
        shape: RoundedRectangleBorder(
          borderRadius: BorderRadius.circular(16),
          side: const BorderSide(color: AppColors.successBorder),
        ),
        title: const Row(
          children: [
            Icon(Icons.verified, color: AppColors.successLight, size: 28),
            SizedBox(width: 8),
            Text('e-POD Certified', style: TextStyle(fontSize: 18, fontWeight: FontWeight.w700)),
          ],
        ),
        content: Column(
          mainAxisSize: MainAxisSize.min,
          crossAxisAlignment: CrossAxisAlignment.start,
          children: [
            Text(
              'Proof of Delivery for ${_selectedShipment.trackingNumber} successfully generated and queued for distributed ledger sync.',
              style: const TextStyle(color: AppColors.textSecondary, fontSize: 13),
            ),
            const SizedBox(height: 12),
            Container(
              padding: const EdgeInsets.all(10),
              decoration: BoxDecoration(
                color: AppColors.bgCard,
                borderRadius: BorderRadius.circular(8),
                border: Border.all(color: AppColors.borderSubtle),
              ),
              child: const Column(
                crossAxisAlignment: CrossAxisAlignment.start,
                children: [
                  Text('SHA-256 HASH VERIFICATION', style: TextStyle(color: AppColors.textMuted, fontSize: 9, fontWeight: FontWeight.w700)),
                  SizedBox(height: 2),
                  Text('9f8b2c4e1a0d8471...e289bf', style: TextStyle(color: AppColors.cyanLight, fontSize: 11, fontFamily: 'monospace')),
                  SizedBox(height: 4),
                  Text('GPS Handover: Lat 8.5833, Lon 39.1167 @ Mojo Hub', style: TextStyle(color: AppColors.textMuted, fontSize: 10)),
                ],
              ),
            ),
          ],
        ),
        actions: [
          ElevatedButton(
            onPressed: () {
              Navigator.pop(ctx);
              setState(() {
                _sigKey.currentState?.clear();
                _hasSignature = false;
              });
            },
            style: ElevatedButton.styleFrom(backgroundColor: AppColors.primary),
            child: const Text('Complete Handover'),
          ),
        ],
      ),
    );
  }

  @override
  Widget build(BuildContext context) {
    return ListView(
      padding: const EdgeInsets.symmetric(horizontal: 16, vertical: 12),
      children: [
        // Header
        GlassCard(
          backgroundColor: AppColors.bgSurface,
          borderColor: AppColors.cyanLight.withValues(alpha: 0.3),
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
                          color: AppColors.cyanLight.withValues(alpha: 0.15),
                          borderRadius: BorderRadius.circular(8),
                        ),
                        child: const Icon(Icons.assignment_turned_in, color: AppColors.cyanLight, size: 18),
                      ),
                      const SizedBox(width: 8),
                      const Text(
                        'DIGITAL e-POD HANDOVER',
                        style: TextStyle(
                          color: AppColors.cyanLight,
                          fontSize: 11,
                          fontWeight: FontWeight.w700,
                          letterSpacing: 0.6,
                        ),
                      ),
                    ],
                  ),
                  StatusBadge.info('Cryptographic Token', icon: Icons.security),
                ],
              ),
              const SizedBox(height: 12),
              Text(
                'Waybill: ${_selectedShipment.trackingNumber}',
                style: const TextStyle(
                  color: AppColors.textMain,
                  fontSize: 18,
                  fontWeight: FontWeight.w800,
                ),
              ),
              const SizedBox(height: 2),
              Text(
                'Consignee: ${_selectedShipment.consignee}',
                style: const TextStyle(color: AppColors.textSecondary, fontSize: 12),
              ),
              const SizedBox(height: 10),
              Container(
                padding: const EdgeInsets.symmetric(horizontal: 10, vertical: 6),
                decoration: BoxDecoration(
                  color: AppColors.bgCard,
                  borderRadius: BorderRadius.circular(6),
                ),
                child: Row(
                  mainAxisAlignment: MainAxisAlignment.spaceBetween,
                  children: [
                    const Text('Cargo Manifest:', style: TextStyle(color: AppColors.textMuted, fontSize: 11)),
                    Text('${_selectedShipment.containersCount} TEU (${_selectedShipment.cargoType})',
                        style: const TextStyle(color: AppColors.textMain, fontSize: 11, fontWeight: FontWeight.w600)),
                  ],
                ),
              ),
            ],
          ),
        ),
        const SizedBox(height: 18),

        // Step 1: Pre-Delivery Checklist
        const Text(
          '1. MANDATORY SAFETY & COMPLIANCE CHECKLIST',
          style: TextStyle(
            color: AppColors.textMuted,
            fontSize: 11,
            fontWeight: FontWeight.w700,
            letterSpacing: 0.6,
          ),
        ),
        const SizedBox(height: 10),
        GlassCard(
          padding: const EdgeInsets.symmetric(vertical: 6, horizontal: 10),
          child: Column(
            children: [
              _buildCheckTile(
                title: 'E-Seal Cryptographic Lock Intact',
                subtitle: 'Seal code #${_selectedShipment.eSealNumber} matches manifest',
                value: _chkSealIntact,
                onChanged: (v) => setState(() => _chkSealIntact = v!),
              ),
              const Divider(),
              _buildCheckTile(
                title: 'Container Shell & Latch Integrity',
                subtitle: 'Zero structural puncture, rust deformation or moisture leak',
                value: _chkCargoIntact,
                onChanged: (v) => setState(() => _chkCargoIntact = v!),
              ),
              const Divider(),
              _buildCheckTile(
                title: 'Cold Chain Temp Logged (-18.2°C)',
                subtitle: 'Pharma / food grade threshold intact through corridor',
                value: _chkTempVerified,
                onChanged: (v) => setState(() => _chkTempVerified = v!),
              ),
              const Divider(),
              _buildCheckTile(
                title: 'Customs & Port Authority Green Release',
                subtitle: 'ASYCUDA declaration ${_selectedShipment.declarationRef}',
                value: _chkCustomsStamp,
                onChanged: (v) => setState(() => _chkCustomsStamp = v!),
              ),
            ],
          ),
        ),
        const SizedBox(height: 18),

        // Step 2: Photo Capture Evidence
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            const Text(
              '2. CARGO CONDITION EVIDENCE PHOTOS',
              style: TextStyle(
                color: AppColors.textMuted,
                fontSize: 11,
                fontWeight: FontWeight.w700,
                letterSpacing: 0.6,
              ),
            ),
            Text(
              '$_photosAttached / 4 Attached',
              style: const TextStyle(color: AppColors.cyanLight, fontSize: 11, fontWeight: FontWeight.w600),
            ),
          ],
        ),
        const SizedBox(height: 10),
        Row(
          children: [
            _buildPhotoSlot(
              title: 'Container Seal',
              hasPhoto: true,
              icon: Icons.lock,
            ),
            const SizedBox(width: 8),
            _buildPhotoSlot(
              title: 'Gate Pass Stamped',
              hasPhoto: true,
              icon: Icons.document_scanner,
            ),
            const SizedBox(width: 8),
            _buildPhotoSlot(
              title: 'Add Photo',
              hasPhoto: false,
              icon: Icons.add_a_photo,
              onTap: () {
                setState(() => _photosAttached = (_photosAttached + 1).clamp(0, 4));
                ScaffoldMessenger.of(context).showSnackBar(
                  const SnackBar(
                    content: Text('Gate inspection photo captured with GPS timestamp watermark.'),
                    backgroundColor: AppColors.bgCardHover,
                  ),
                );
              },
            ),
          ],
        ),
        const SizedBox(height: 18),

        // Step 3: Recipient Information
        const Text(
          '3. RECIPIENT / DRYPORT YARD AGENT',
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
            children: [
              TextField(
                controller: _receiverNameCtrl,
                decoration: const InputDecoration(
                  labelText: 'Receiving Officer Name',
                  prefixIcon: Icon(Icons.person_outline, size: 18),
                ),
              ),
              const SizedBox(height: 10),
              TextField(
                controller: _receiverPhoneCtrl,
                decoration: const InputDecoration(
                  labelText: 'Contact Phone / National ID',
                  prefixIcon: Icon(Icons.phone_outlined, size: 18),
                ),
              ),
              const SizedBox(height: 10),
              TextField(
                controller: _receiverTerminalCtrl,
                decoration: const InputDecoration(
                  labelText: 'Delivery Depot / Bay Location',
                  prefixIcon: Icon(Icons.warehouse_outlined, size: 18),
                ),
              ),
            ],
          ),
        ),
        const SizedBox(height: 18),

        // Step 4: Digital Signature Canvas
        Row(
          mainAxisAlignment: MainAxisAlignment.spaceBetween,
          children: [
            const Text(
              '4. RECIPIENT DIGITAL SIGNATURE',
              style: TextStyle(
                color: AppColors.textMuted,
                fontSize: 11,
                fontWeight: FontWeight.w700,
                letterSpacing: 0.6,
              ),
            ),
            if (_hasSignature)
              const Row(
                children: [
                  Icon(Icons.check_circle, size: 14, color: AppColors.successLight),
                  SizedBox(width: 4),
                  Text('Signed', style: TextStyle(color: AppColors.successLight, fontSize: 11, fontWeight: FontWeight.w600)),
                ],
              ),
          ],
        ),
        const SizedBox(height: 10),
        SignaturePad(
          key: _sigKey,
          onSignatureChanged: (hasSig) {
            setState(() => _hasSignature = hasSig);
          },
        ),
        const SizedBox(height: 24),

        // Submission Button
        ElevatedButton.icon(
          onPressed: _handleSubmit,
          icon: const Icon(Icons.verified_outlined, size: 18),
          label: const Text('ISSUE SIGNED e-POD & CERTIFICATE'),
          style: ElevatedButton.styleFrom(
            backgroundColor: AppColors.primary,
            minimumSize: const Size.fromHeight(50),
          ),
        ),
        const SizedBox(height: 20),
      ],
    );
  }

  Widget _buildCheckTile({
    required String title,
    required String subtitle,
    required bool value,
    required ValueChanged<bool?> onChanged,
  }) {
    return CheckboxListTile(
      value: value,
      onChanged: onChanged,
      activeColor: AppColors.cyanLight,
      checkColor: AppColors.bgSurface,
      contentPadding: EdgeInsets.zero,
      title: Text(
        title,
        style: TextStyle(
          color: value ? AppColors.textMain : AppColors.textSecondary,
          fontSize: 13,
          fontWeight: FontWeight.w600,
        ),
      ),
      subtitle: Text(
        subtitle,
        style: const TextStyle(color: AppColors.textMuted, fontSize: 11),
      ),
    );
  }

  Widget _buildPhotoSlot({
    required String title,
    required bool hasPhoto,
    required IconData icon,
    VoidCallback? onTap,
  }) {
    return Expanded(
      child: InkWell(
        onTap: onTap,
        borderRadius: BorderRadius.circular(10),
        child: Container(
          height: 80,
          decoration: BoxDecoration(
            color: hasPhoto ? AppColors.bgCard : AppColors.bgSurface,
            borderRadius: BorderRadius.circular(10),
            border: Border.all(
              color: hasPhoto ? AppColors.cyanLight.withValues(alpha: 0.4) : AppColors.borderMedium,
              style: hasPhoto ? BorderStyle.solid : BorderStyle.solid,
            ),
          ),
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              Icon(icon, size: 22, color: hasPhoto ? AppColors.cyanLight : AppColors.textMuted),
              const SizedBox(height: 6),
              Text(
                title,
                style: TextStyle(
                  color: hasPhoto ? AppColors.textMain : AppColors.textMuted,
                  fontSize: 10,
                  fontWeight: FontWeight.w600,
                ),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
