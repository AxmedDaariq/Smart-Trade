// lib/api/api_client.dart
import 'package:dio/dio.dart';
import 'package:dio_cookie_manager/dio_cookie_manager.dart';
import 'package:cookie_jar/cookie_jar.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';

class ApiClient {
  final Dio _dio;
  final FlutterSecureStorage _secureStorage;

  ApiClient({required String baseUrl})
      : _dio = Dio(BaseOptions(baseUrl: baseUrl, connectTimeout: const Duration(seconds: 15), receiveTimeout: const Duration(seconds: 15))),
        _secureStorage = const FlutterSecureStorage() {
    // Persist cookies (e.g., HttpOnly refresh token)
    final cookieJar = PersistCookieJar();
    _dio.interceptors.add(CookieManager(cookieJar));

    // Inject JWT access token into each request
    _dio.interceptors.add(InterceptorsWrapper(onRequest: (options, handler) async {
      final token = await _secureStorage.read(key: 'access_token');
      if (token != null && token.isNotEmpty) {
        options.headers['Authorization'] = 'Bearer $token';
      }
      return handler.next(options);
    }));
  }

  Dio get client => _dio;

  // Refresh token endpoint – backend should set new access token in the response body
  Future<bool> refreshToken() async {
    try {
      final response = await _dio.get('/api/auth/refresh');
      if (response.statusCode == 200) {
        final newToken = response.data['accessToken'] as String?;
        if (newToken != null) {
          await _secureStorage.write(key: 'access_token', value: newToken);
          return true;
        }
      }
    } catch (_) {}
    return false;
  }
}
