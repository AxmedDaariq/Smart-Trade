import 'package:flutter/material.dart';
import '../theme/app_colors.dart';

class SignaturePad extends StatefulWidget {
  final ValueChanged<bool>? onSignatureChanged;

  const SignaturePad({super.key, this.onSignatureChanged});

  @override
  State<SignaturePad> createState() => SignaturePadState();
}

class SignaturePadState extends State<SignaturePad> {
  final List<List<Offset>> _strokes = [];
  List<Offset> _currentStroke = [];

  bool get hasSignature => _strokes.isNotEmpty;

  void clear() {
    setState(() {
      _strokes.clear();
      _currentStroke.clear();
    });
    widget.onSignatureChanged?.call(false);
  }

  @override
  Widget build(BuildContext context) {
    return Container(
      height: 160,
      width: double.infinity,
      decoration: BoxDecoration(
        color: AppColors.bgSurface,
        borderRadius: BorderRadius.circular(12),
        border: Border.all(
          color: hasSignature ? AppColors.cyanLight : AppColors.borderMedium,
          width: 1.5,
        ),
      ),
      child: Stack(
        children: [
          // Background guidelines
          Positioned(
            bottom: 36,
            left: 20,
            right: 20,
            child: Row(
              children: [
                const Text(
                  'X ',
                  style: TextStyle(
                    color: AppColors.textMuted,
                    fontSize: 16,
                    fontWeight: FontWeight.w700,
                  ),
                ),
                Expanded(
                  child: Container(
                    height: 1,
                    color: AppColors.borderMedium,
                  ),
                ),
              ],
            ),
          ),
          if (!hasSignature && _currentStroke.isEmpty)
            const Center(
              child: Text(
                'Sign with stylus or finger here',
                style: TextStyle(
                  color: AppColors.textMuted,
                  fontSize: 12,
                  fontStyle: FontStyle.italic,
                ),
              ),
            ),
          // Interactive drawing gesture detector
          GestureDetector(
            onPanStart: (details) {
              setState(() {
                final RenderBox renderBox = context.findRenderObject() as RenderBox;
                final localPosition = renderBox.globalToLocal(details.globalPosition);
                _currentStroke = [localPosition];
                _strokes.add(_currentStroke);
              });
              widget.onSignatureChanged?.call(true);
            },
            onPanUpdate: (details) {
              setState(() {
                final RenderBox renderBox = context.findRenderObject() as RenderBox;
                final localPosition = renderBox.globalToLocal(details.globalPosition);
                _currentStroke.add(localPosition);
              });
            },
            onPanEnd: (details) {
              _currentStroke = [];
            },
            child: CustomPaint(
              size: Size.infinite,
              painter: _SignaturePainter(strokes: _strokes),
            ),
          ),
          // Clear Button in Top Right
          if (hasSignature)
            Positioned(
              top: 8,
              right: 8,
              child: InkWell(
                onTap: clear,
                borderRadius: BorderRadius.circular(6),
                child: Container(
                  padding: const EdgeInsets.symmetric(horizontal: 8, vertical: 4),
                  decoration: BoxDecoration(
                    color: AppColors.bgCardHover,
                    borderRadius: BorderRadius.circular(6),
                    border: Border.all(color: AppColors.borderSubtle),
                  ),
                  child: const Row(
                    mainAxisSize: MainAxisSize.min,
                    children: [
                      Icon(Icons.refresh, size: 12, color: AppColors.textSecondary),
                      SizedBox(width: 4),
                      Text(
                        'Clear',
                        style: TextStyle(
                          color: AppColors.textSecondary,
                          fontSize: 11,
                          fontWeight: FontWeight.w500,
                        ),
                      ),
                    ],
                  ),
                ),
              ),
            ),
        ],
      ),
    );
  }
}

class _SignaturePainter extends CustomPainter {
  final List<List<Offset>> strokes;

  _SignaturePainter({required this.strokes});

  @override
  void paint(Canvas canvas, Size size) {
    final paint = Paint()
      ..color = AppColors.cyanLight
      ..strokeCap = StrokeCap.round
      ..strokeWidth = 2.5
      ..style = PaintingStyle.stroke;

    for (final stroke in strokes) {
      if (stroke.length > 1) {
        for (int i = 0; i < stroke.length - 1; i++) {
          canvas.drawLine(stroke[i], stroke[i + 1], paint);
        }
      } else if (stroke.length == 1) {
        canvas.drawCircle(stroke[0], 1.25, paint);
      }
    }
  }

  @override
  bool shouldRepaint(covariant _SignaturePainter oldDelegate) => true;
}
