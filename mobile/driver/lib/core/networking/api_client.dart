import 'package:dio/dio.dart';
import 'package:flutter/foundation.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:uuid/uuid.dart';

class ApiClient {
  final Dio _dio;
  final FlutterSecureStorage _secureStorage;
  static const String _tokenKey = 'access_token';
  static const String _refreshTokenKey = 'refresh_token';

  ApiClient({required String baseUrl, required this._secureStorage})
      : _dio = Dio(BaseOptions(
          baseUrl: baseUrl,
          connectTimeout: const Duration(seconds: 15),
          receiveTimeout: const Duration(seconds: 15),
        )) {
    _dio.interceptors.add(_createAuthInterceptor());
    _dio.interceptors.add(_createRetryInterceptor());
  }

  /// The configured [Dio] instance, exposed for tests so they can attach a
  /// mock [HttpClientAdapter] (http_mock_adapter) and observe the real
  /// interceptor pipeline without a live backend.
  @visibleForTesting
  Dio get dio => _dio;

  /// Refresh token endpoint – backend should set new access token in the response body
  Future<bool> refreshToken() async {
    try {
      final response = await _dio.get('/api/auth/refresh');
      if (response.statusCode == 200) {
        final newToken = response.data['accessToken'] as String?;
        if (newToken != null) {
          await _secureStorage.write(key: _tokenKey, value: newToken);
          return true;
        }
      }
    } catch (_) {}
    return false;
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
          final success = await refreshToken();
          if (success) {
            // Retry the original request with the new token
            final options = e.requestOptions;
            final token = await _secureStorage.read(key: _tokenKey);
            options.headers['Authorization'] = 'Bearer $token';

            try {
              final response = await _dio.request(
                options.path,
                data: options.data,
                queryParameters: options.queryParameters,
                options: Options(
                  method: options.method,
                  headers: options.headers,
                ),
              );
              return handler.resolve(response);
            } catch (retryError) {
              return handler.next(retryError is DioException ? retryError : DioException(
                requestOptions: options,
                error: retryError,
              ));
            }
          } else {
            await _secureStorage.delete(key: _tokenKey);
            await _secureStorage.delete(key: _refreshTokenKey);
            return handler.next(e);
          }
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
