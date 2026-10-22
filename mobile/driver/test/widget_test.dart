import 'package:flutter_test/flutter_test.dart';
import 'package:smart_trade_driver/main.dart';

void main() {
  testWidgets('SmartTradeDriverApp smoke test and tab rendering', (WidgetTester tester) async {
    // Build our app and trigger a frame.
    await tester.pumpWidget(const SmartTradeDriverApp());

    // Verify that the title and key components render
    expect(find.text('Smart Trade'), findsOneWidget);
    expect(find.text('ACTIVE CONVOY MISSION'), findsOneWidget);
  });
}
