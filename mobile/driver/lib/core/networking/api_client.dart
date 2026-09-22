import 'package:dio/dio.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:uuid/uuid.dart';

class ApiClient {
  final Dio _dio;
  final FlutterSecureStorage _secureStorage;
  static const String _tokenKey = 'access_token';
  static const String _refreshTokenKey = 'refresh_token';

  ApiClient({required String baseUrl, required FlutterSecureStorage secureStorage})
      : _secureStorage = secureStorage,
        _dio = Dio(BaseOptions(
          baseUrl: baseUrl,
          connectTimeout: const Duration(seconds: 15),
          receiveTimeout: const Duration(seconds: 15),
        )) {
    _dio.interceptors.add(_createAuthInterceptor());
    _dio.interceptors.add(_createRetryInterceptor());
  }

  Interceptor _createAuthInterceptor() {
    return InterceptorsWrapper(
      onRequest: (options, handler) async {
        final token = await _secureStorage.read(key: _tokenKey);
        if (token != null) {
          options.headers['Authorization'] = 'Bearer $token';
        }
        
        // Inject Idempotency-Key for POST/PUT requests
        if (options.method == 'POST' || options.method == 'PUT') {
          options.headers['Idempotency-Key'] = const Uuid().v4();
        }
        
        // Inject CorrelationId
        options.headers['X-Correlation-ID'] = const Uuid().v4();
        
        return handler.next(options);
      },
      onError: (DioException e, handler) async {
        if (e.response?.statusCode == 401) {
          // Attempt token refresh logic here
          // If refresh fails, clear session and throw specific AuthException
          await _secureStorage.delete(key: _tokenKey);
          await _secureStorage.delete(key: _refreshTokenKey);
          // throw AuthException('Session expired');
        }
        return handler.next(e);
      },
    );
  }

  Interceptor _createRetryInterceptor() {
    return InterceptorsWrapper(
      onError: (DioException e, handler) {
        // Implement exponential backoff for 5xx and 429
        if (e.response?.statusCode == 429 || (e.response?.statusCode ?? 0) >= 500) {
           // Queue for retry logic
        }
        return handler.next(e);
      }
    );
  }
}
