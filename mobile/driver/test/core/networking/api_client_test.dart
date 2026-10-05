/// Unit tests for the [ApiClient] interceptor pipeline.
///
/// Requests are served by `http_mock_adapter`, so the real interceptors
/// (bearer auth, idempotency/correlation ids, 401 session cleanup, 5xx
/// pass-through) run against a mock adapter instead of a live backend.
library;

import 'package:dio/dio.dart';
import 'package:flutter_secure_storage/flutter_secure_storage.dart';
import 'package:flutter_test/flutter_test.dart';
import 'package:http_mock_adapter/http_mock_adapter.dart';
import 'package:smart_trade_driver/core/networking/api_client.dart';

/// Canonical RFC-4122 v4 UUID, as produced by `package:uuid`.
final RegExp _uuidV4 = RegExp(
  r'^[0-9a-f]{8}-[0-9a-f]{4}-4[0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$',
  caseSensitive: false,
);

/// In-memory [FlutterSecureStorage] so tests never reach the platform
/// keychain, and so token deletions can be asserted directly.
class _FakeSecureStorage extends FlutterSecureStorage {
  final Map<String, String> store = {};
  final List<String> deletedKeys = [];

  @override
  Future<String?> read({
    required String key,
    IOSOptions? iOptions,
    AndroidOptions? aOptions,
    LinuxOptions? lOptions,
    WebOptions? webOptions,
    MacOsOptions? mOptions,
    WindowsOptions? wOptions,
  }) async =>
      store[key];

  @override
  Future<void> delete({
    required String key,
    IOSOptions? iOptions,
    AndroidOptions? aOptions,
    LinuxOptions? lOptions,
    WebOptions? webOptions,
    MacOsOptions? mOptions,
    WindowsOptions? wOptions,
  }) async {
    deletedKeys.add(key);
    store.remove(key);
  }
}

/// Case-insensitive header lookup — Dio keeps the casing given by the
/// interceptor but adapters may normalise it, so never rely on it.
String? _header(Map<String, dynamic> headers, String name) {
  for (final entry in headers.entries) {
    if (entry.key.toLowerCase() == name.toLowerCase()) {
      return entry.value?.toString();
    }
  }
  return null;
}

void main() {
  const baseUrl = 'https://api.smarttrade.test';

  late _FakeSecureStorage storage;
  late ApiClient client;
  late DioAdapter adapter;

  /// Options of the last request that reached the (mock) adapter.
  RequestOptions? lastRequest;

  setUp(() {
    storage = _FakeSecureStorage();
    client = ApiClient(baseUrl: baseUrl, secureStorage: storage);
    adapter = DioAdapter(dio: client.dio);
    lastRequest = null;
  });

  /// Registers a GET [route] and records the request seen by the adapter.
  void mockGet(String route, {int statusCode = 200, Object? body}) {
    adapter.onGet(
      route,
      (server) => server.replyCallback(statusCode, (options) {
        lastRequest = options;
        return body ?? <String, dynamic>{};
      }),
    );
  }

  /// Registers a POST [route] (any body) and records the request seen.
  void mockPost(String route, {int statusCode = 201}) {
    adapter.onPost(
      route,
      (server) => server.replyCallback(statusCode, (options) {
        lastRequest = options;
        return <String, dynamic>{'ok': true};
      }),
      data: Matchers.any,
    );
  }

  group('request headers', () {
    test('adds "Authorization: Bearer" when an access token is stored',
        () async {
      storage.store['access_token'] = 'token-abc';
      mockGet('/api/jobs');

      final response = await client.dio.get<Map<String, dynamic>>('/api/jobs');

      expect(response.statusCode, 200);
      expect(_header(lastRequest!.headers, 'Authorization'), 'Bearer token-abc');
    });

    test('omits "Authorization" when no access token is stored', () async {
      mockGet('/api/jobs');

      await client.dio.get<Map<String, dynamic>>('/api/jobs');

      expect(_header(lastRequest!.headers, 'Authorization'), isNull);
    });

    test('adds a valid UUIDv4 "X-Correlation-ID" to every request', () async {
      mockGet('/api/jobs');

      await client.dio.get<Map<String, dynamic>>('/api/jobs');

      final correlationId = _header(lastRequest!.headers, 'X-Correlation-ID');
      expect(correlationId, isNotNull);
      expect(correlationId, matches(_uuidV4));
    });

    test('generates a fresh correlation id per request', () async {
      mockGet('/api/jobs');
      final seen = <String?>[];

      await client.dio.get<Map<String, dynamic>>('/api/jobs');
      seen.add(_header(lastRequest!.headers, 'X-Correlation-ID'));
      await client.dio.get<Map<String, dynamic>>('/api/jobs');
      seen.add(_header(lastRequest!.headers, 'X-Correlation-ID'));

      expect(seen[0], isNot(equals(seen[1])));
    });

    test('adds an "Idempotency-Key" (UUIDv4) to POST but not to GET',
        () async {
      mockPost('/api/sync');
      mockGet('/api/jobs');

      await client.dio.get<Map<String, dynamic>>('/api/jobs');
      expect(_header(lastRequest!.headers, 'Idempotency-Key'), isNull);

      await client.dio.post<Map<String, dynamic>>(
        '/api/sync',
        data: {'deliveryId': 'd-1'},
      );
      final idempotencyKey = _header(lastRequest!.headers, 'Idempotency-Key');
      expect(idempotencyKey, matches(_uuidV4));
    });

    test('POST carries both correlation and idempotency ids', () async {
      mockPost('/api/sync');

      await client.dio.post<Map<String, dynamic>>(
        '/api/sync',
        data: {'deliveryId': 'd-1'},
      );

      expect(
        _header(lastRequest!.headers, 'X-Correlation-ID'),
        matches(_uuidV4),
      );
      expect(
        _header(lastRequest!.headers, 'Idempotency-Key'),
        matches(_uuidV4),
      );
    });
  });

  group('request target', () {
    test('resolves the route against the configured base url', () async {
      mockGet('/api/jobs');

      await client.dio.get<Map<String, dynamic>>('/api/jobs');

      expect(lastRequest!.uri.toString(), '$baseUrl/api/jobs');
    });
  });

  group('401 handling', () {
    test('clears access and refresh tokens and surfaces a DioException',
        () async {
      storage.store['access_token'] = 'stale-token';
      storage.store['refresh_token'] = 'stale-refresh';
      adapter.onGet('/api/secure', (server) {
        server.reply(401, {'error': 'expired'});
      });

      await expectLater(
        client.dio.get<Map<String, dynamic>>('/api/secure'),
        throwsA(isA<DioException>()),
      );

      expect(
        storage.deletedKeys,
        containsAll(<String>['access_token', 'refresh_token']),
      );
      expect(storage.store, isEmpty);
    });

    test('leaves stored tokens untouched when the request succeeds', () async {
      storage.store['access_token'] = 'fresh-token';
      storage.store['refresh_token'] = 'fresh-refresh';
      mockGet('/api/jobs');

      await client.dio.get<Map<String, dynamic>>('/api/jobs');

      expect(storage.deletedKeys, isEmpty);
      expect(storage.store['access_token'], 'fresh-token');
    });
  });

  group('5xx handling', () {
    test('propagates 500 to the caller without wiping the session', () async {
      storage.store['access_token'] = 'keep-me';
      adapter.onGet('/api/reports', (server) {
        server.reply(500, {'error': 'boom'});
      });

      await expectLater(
        client.dio.get<Map<String, dynamic>>('/api/reports'),
        throwsA(
          isA<DioException>().having(
            (e) => e.response?.statusCode,
            'response.statusCode',
            500,
          ),
        ),
      );

      // The retry interceptor must not swallow the error, and a server
      // fault is not an auth failure — the session survives.
      expect(storage.deletedKeys, isEmpty);
      expect(storage.store['access_token'], 'keep-me');
    });
  });
}



