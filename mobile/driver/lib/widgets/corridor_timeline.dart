import 'package:flutter/material.dart';
import '../models/logistics_models.dart';
import '../theme/app_colors.dart';

class CorridorTimeline extends StatelessWidget {
  final List<WaypointItem> waypoints;

  const CorridorTimeline({super.key, required this.waypoints});

  @override
  Widget build(BuildContext context) {
    return Column(
      children: List.generate(waypoints.length, (index) {
        final item = waypoints[index];
        final isLast = index == waypoints.length - 1;
        final isCompleted = item.status == 'completed';
        final isActive = item.status == 'active';

        Color dotColor;
        Color lineColor;
        Widget dotChild;

        if (isCompleted) {
          dotColor = AppColors.success;
          lineColor = AppColors.success.withValues(alpha: 0.5);
          dotChild = const Icon(Icons.check, size: 10, color: Colors.white);
        } else if (isActive) {
          dotColor = AppColors.cyanLight;
          lineColor = AppColors.borderSubtle;
          dotChild = Container(
            width: 8,
            height: 8,
            decoration: const BoxDecoration(
              color: Colors.white,
              shape: BoxShape.circle,
            ),
          );
        } else {
          dotColor = AppColors.bgSurface;
          lineColor = AppColors.borderSubtle;
          dotChild = const SizedBox();
        }

        return IntrinsicHeight(
          child: Row(
            crossAxisAlignment: CrossAxisAlignment.start,
            children: [
              // Indicator column
              SizedBox(
                width: 24,
                child: Column(
                  children: [
                    Container(
                      width: 20,
                      height: 20,
                      decoration: BoxDecoration(
                        color: dotColor,
                        shape: BoxShape.circle,
                        border: Border.all(
                          color: isActive
                              ? AppColors.cyanLight
                              : (isCompleted ? AppColors.success : AppColors.borderMedium),
                          width: 2,
                        ),
                        boxShadow: isActive
                            ? [
                                BoxShadow(
                                  color: AppColors.cyanLight.withValues(alpha: 0.5),
                                  blurRadius: 8,
                                  spreadRadius: 1,
                                )
                              ]
                            : null,
                      ),
                      alignment: Alignment.center,
                      child: dotChild,
                    ),
                    if (!isLast)
                      Expanded(
                        child: Container(
                          width: 2,
                          color: lineColor,
                          margin: const EdgeInsets.symmetric(vertical: 4),
                        ),
                      ),
                  ],
                ),
              ),
              const SizedBox(width: 14),
              // Content
              Expanded(
                child: Padding(
                  padding: EdgeInsets.only(bottom: isLast ? 0 : 20),
                  child: Column(
                    crossAxisAlignment: CrossAxisAlignment.start,
                    children: [
                      Row(
                        mainAxisAlignment: MainAxisAlignment.spaceBetween,
                        children: [
                          Expanded(
                            child: Text(
                              item.title,
                              style: TextStyle(
                                color: isActive ? AppColors.cyanLight : AppColors.textMain,
                                fontSize: 13,
                                fontWeight: isActive ? FontWeight.w700 : FontWeight.w600,
                              ),
                            ),
                          ),
                          Text(
                            item.timestamp,
                            style: const TextStyle(
                              color: AppColors.textMuted,
                              fontSize: 11,
                              fontWeight: FontWeight.w500,
                            ),
                          ),
                        ],
                      ),
                      const SizedBox(height: 2),
                      Text(
                        item.location,
                        style: const TextStyle(
                          color: AppColors.textSecondary,
                          fontSize: 12,
                        ),
                      ),
                      if (item.note != null) ...[
                        const SizedBox(height: 4),
                        Container(
                          padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                          decoration: BoxDecoration(
                            color: AppColors.bgSurface,
                            borderRadius: BorderRadius.circular(6),
                            border: Border.all(color: AppColors.borderSubtle),
                          ),
                          child: Text(
                            item.note!,
                            style: const TextStyle(
                              color: AppColors.textMuted,
                              fontSize: 11,
                              fontStyle: FontStyle.italic,
                            ),
                          ),
                        ),
                      ],
                    ],
                  ),
                ),
              ),
            ],
          ),
        );
      }),
    );
  }
}
