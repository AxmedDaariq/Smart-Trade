import 'package:flutter_test/flutter_test.dart';
import 'package:integration_test/integration_test.dart';
import 'package:flutter/material.dart';
import 'package:smart_trade_driver/main.dart' as app; // Adjusted to actual app entry point

void main() {
  IntegrationTestWidgetsFlutterBinding.ensureInitialized();

  group('Driver E2E Journey', () {
    testWidgets('Driver should be able to login, view assignment, and confirm delivery', (WidgetTester tester) async {
      // 1. Start the app
      app.main();
      await tester.pumpAndSettle();

      // 2. Login Flow
      await tester.enterText(find.byType(TextField).at(0), 'driver_test@smarttrade.com');
      await tester.enterText(find.byType(TextField).at(1), 'DriverPass123!');
      await tester.tap(find.byType(ElevatedButton));
      await tester.pumpAndSettle();

      // Verify Login success (redirect to Home/Assignments)
      expect(find.text('My Assignments'), findsOneWidget);

      // 3. View Assigned Shipment
      // Assuming the first shipment card is clicked
      await tester.tap(find.byType(ListTile).first);
      await tester.pumpAndSettle();

      // Verify shipment details are visible
      expect(find.text('Shipment Details'), findsOneWidget);

      // 4. Confirm Delivery
      await tester.tap(find.text('Confirm Delivery'));
      await tester.pumpAndSettle();

      // Verify delivery confirmation message
      expect(find.text('Delivery Confirmed Successfully'), findsOneWidget);
    });
  });
}
