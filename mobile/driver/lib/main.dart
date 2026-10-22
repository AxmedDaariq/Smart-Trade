import 'package:flutter/material.dart';
import 'screens/home_dashboard_screen.dart';
import 'theme/app_theme.dart';

void main() {
  WidgetsFlutterBinding.ensureInitialized();
  runApp(const SmartTradeDriverApp());
}

class SmartTradeDriverApp extends StatelessWidget {
  const SmartTradeDriverApp({super.key});

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      title: 'Smart Trade Logistics Driver',
      debugShowCheckedModeBanner: false,
      theme: AppTheme.darkTheme,
      home: const HomeDashboardScreen(),
    );
  }
}
