import 'package:flutter/material.dart';
import '../theme/app_colors.dart';

class StatusBadge extends StatelessWidget {
  final String label;
  final Color backgroundColor;
  final Color textColor;
  final Color borderColor;
  final IconData? icon;
  final bool hasPulseDot;

  const StatusBadge({
    super.key,
    required this.label,
    required this.backgroundColor,
    required this.textColor,
    required this.borderColor,
    this.icon,
    this.hasPulseDot = false,
  });

  factory StatusBadge.success(String label, {IconData? icon, bool pulse = false}) {
    return StatusBadge(
      label: label,
      backgroundColor: AppColors.successBg,
      textColor: AppColors.successLight,
      borderColor: AppColors.successBorder,
      icon: icon,
      hasPulseDot: pulse,
    );
  }

  factory StatusBadge.warning(String label, {IconData? icon, bool pulse = false}) {
    return StatusBadge(
      label: label,
      backgroundColor: AppColors.warningBg,
      textColor: AppColors.warningLight,
      borderColor: AppColors.warningBorder,
      icon: icon,
      hasPulseDot: pulse,
    );
  }

  factory StatusBadge.danger(String label, {IconData? icon, bool pulse = false}) {
    return StatusBadge(
      label: label,
      backgroundColor: AppColors.dangerBg,
      textColor: AppColors.dangerLight,
      borderColor: AppColors.dangerBorder,
      icon: icon,
      hasPulseDot: pulse,
    );
  }

  factory StatusBadge.info(String label, {IconData? icon, bool pulse = false}) {
    return StatusBadge(
      label: label,
      backgroundColor: const Color(0x1F0EA5E9),
      textColor: AppColors.cyanLight,
      borderColor: const Color(0x400EA5E9),
      icon: icon,
      hasPulseDot: pulse,
    );
  }

  factory StatusBadge.purple(String label, {IconData? icon}) {
    return StatusBadge(
      label: label,
      backgroundColor: AppColors.purpleBg,
      textColor: AppColors.purpleLight,
      borderColor: const Color(0x40A855F7),
      icon: icon,
    );
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      padding: const EdgeInsets.symmetric(horizontal: 9, vertical: 4),
      decoration: BoxDecoration(
        color: backgroundColor,
        borderRadius: BorderRadius.circular(20),
        border: Border.all(color: borderColor, width: 1),
      ),
      child: Row(
        mainAxisSize: MainAxisSize.min,
        children: [
          if (hasPulseDot) ...[
            Container(
              width: 6,
              height: 6,
              decoration: BoxDecoration(
                color: textColor,
                shape: BoxShape.circle,
                boxShadow: [
                  BoxShadow(
                    color: textColor.withValues(alpha: 0.6),
                    blurRadius: 4,
                    spreadRadius: 1,
                  ),
                ],
              ),
            ),
            const SizedBox(width: 5),
          ] else if (icon != null) ...[
            Icon(icon, size: 12, color: textColor),
            const SizedBox(width: 4),
          ],
          Text(
            label,
            style: TextStyle(
              color: textColor,
              fontSize: 11,
              fontWeight: FontWeight.w600,
              letterSpacing: 0.2,
            ),
          ),
        ],
      ),
    );
  }
}
