/// Unit tests for the offline synchronization engine.
///
/// The repository is replaced by an in-memory fake so each event's
/// status transitions can be asserted without storage or a backend.
library;

import 'package:flutter_test/flutter_test.dart';
import 'package:smart_trade_driver/core/synchronization/offline_queue.dart';

/// In-memory [IOfflineQueueRepository] that records every mutation and can
/// be told to fail the "Succeeded" transition for specific events, which is
/// how a rejected API call surfaces in [OfflineSyncEngine.syncPendingEvents].
class _InMemoryQueueRepository implements IOfflineQueueRepository {
  _InMemoryQueueRepository([List<OfflineEvent>? seed])
      : events = List.of(seed ?? const <OfflineEvent>[]);

  final List<OfflineEvent> events;

  /// `'localId:Status.name'` entries in the order they were applied.
  final List<String> transitions = [];

  /// localIds whose "Succeeded" update should throw, as if the API failed.
  final Set<String> failSucceededFor = {};

  @override
  Future<void> enqueue(OfflineEvent event) async => events.add(event);

  @override
  Future<List<OfflineEvent>> getPendingEvents() async =>
      events.where((e) => e.status == OfflineEventStatus.Pending).toList();

  @override
  Future<void> updateEventStatus(
    String localId,
    OfflineEventStatus status,
  ) async {
    if (status == OfflineEventStatus.Succeeded &&
        failSucceededFor.contains(localId)) {
      throw StateError('simulated API rejection for $localId');
    }
    _find(localId).status = status;
    transitions.add('$localId:${status.name}');
  }

  @override
  Future<void> incrementRetry(String localId) async {
    _find(localId).retryCount += 1;
  }

  OfflineEvent _find(String localId) =>
      events.firstWhere((e) => e.localId == localId);
}

OfflineEvent _event({String eventType = 'delivery.picked_up'}) =>
    OfflineEvent(
      eventType: eventType,
      aggregateType: 'Delivery',
      aggregateId: 'delivery-1',
      payload: {'at': '2026-01-01T00:00:00Z'},
    );

void main() {
  group('OfflineEvent', () {
    test('starts Pending with zero retries and a UTC timestamp', () {
      final event = _event();

      expect(event.status, OfflineEventStatus.Pending);
      expect(event.retryCount, 0);
      expect(event.errorCode, isNull);
      expect(event.createdAtUtc.isUtc, isTrue);
      expect(event.payload['at'], '2026-01-01T00:00:00Z');
      expect(event.eventType, 'delivery.picked_up');
      expect(event.aggregateType, 'Delivery');
      expect(event.aggregateId, 'delivery-1');
    });

    test('assigns every event a unique local id', () {
      final ids = <String>{for (var i = 0; i < 25; i++) _event().localId};

      expect(ids.length, 25);
      expect(ids.every((id) => id.isNotEmpty), isTrue);
    });
  });

  group('OfflineSyncEngine.syncPendingEvents', () {
    test('marks each pending event Sending then Succeeded', () async {
      final first = _event();
      final second = _event(eventType: 'delivery.delivered');
      final repo = _InMemoryQueueRepository([first, second]);

      await OfflineSyncEngine(repo).syncPendingEvents();

      expect(first.status, OfflineEventStatus.Succeeded);
      expect(second.status, OfflineEventStatus.Succeeded);
      expect(first.retryCount, 0);
      expect(second.retryCount, 0);
      expect(repo.transitions, <String>[
        '${first.localId}:Sending',
        '${first.localId}:Succeeded',
        '${second.localId}:Sending',
        '${second.localId}:Succeeded',
      ]);
    });

    test('increments the retry count for a failing event and keeps going',
        () async {
      final failing = _event();
      final healthy = _event(eventType: 'delivery.delivered');
      final repo = _InMemoryQueueRepository([failing, healthy]);
      repo.failSucceededFor.add(failing.localId);

      await OfflineSyncEngine(repo).syncPendingEvents();

      // The failed event never reaches Succeeded and gets one retry logged…
      expect(failing.retryCount, 1);
      expect(failing.status, OfflineEventStatus.Sending);
      // …while the engine still processes the remaining events.
      expect(healthy.status, OfflineEventStatus.Succeeded);
      expect(healthy.retryCount, 0);
    });

    test('does nothing when the queue is empty', () async {
      final repo = _InMemoryQueueRepository();

      await OfflineSyncEngine(repo).syncPendingEvents();

      expect(repo.transitions, isEmpty);
      expect(repo.events, isEmpty);
    });

    test('skips events that are not Pending', () async {
      final alreadySucceeded = _event()
        ..status = OfflineEventStatus.Succeeded;
      final pending = _event(eventType: 'delivery.delivered');
      final repo = _InMemoryQueueRepository([alreadySucceeded, pending]);

      await OfflineSyncEngine(repo).syncPendingEvents();

      expect(repo.transitions, <String>[
        '${pending.localId}:Sending',
        '${pending.localId}:Succeeded',
      ]);
      // The already-completed event was never touched.
      expect(alreadySucceeded.status, OfflineEventStatus.Succeeded);
      expect(alreadySucceeded.retryCount, 0);
    });

    test('documents current behaviour: a failed event stays in Sending '
        'and is not re-picked by the next pass', () async {
      final failing = _event();
      final repo = _InMemoryQueueRepository([failing]);
      repo.failSucceededFor.add(failing.localId);

      final engine = OfflineSyncEngine(repo);
      await engine.syncPendingEvents();
      await engine.syncPendingEvents();

      // Second pass: only Pending events are fetched, and the failed one
      // is stuck in Sending, so it is neither retried nor transitioned.
      expect(failing.retryCount, 1);
      expect(failing.status, OfflineEventStatus.Sending);
      expect(repo.transitions, <String>['${failing.localId}:Sending']);
    });
  });
}

