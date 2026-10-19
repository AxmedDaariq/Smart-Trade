import 'package:uuid/uuid.dart';

enum OfflineEventStatus { pending, sending, succeeded, failed, deadLetter }

class OfflineEvent {
  final String localId;
  final String eventType;
  final String aggregateType;
  final String aggregateId;
  final DateTime createdAtUtc;
  final Map<String, dynamic> payload;
  int retryCount;
  OfflineEventStatus status;
  String? errorCode;

  OfflineEvent({
    required this.eventType,
    required this.aggregateType,
    required this.aggregateId,
    required this.payload,
  })  : localId = const Uuid().v4(),
        createdAtUtc = DateTime.now().toUtc(),
        retryCount = 0,
        status = OfflineEventStatus.pending;
}

abstract class IOfflineQueueRepository {
  Future<void> enqueue(OfflineEvent event);
  Future<List<OfflineEvent>> getPendingEvents();
  Future<void> updateEventStatus(String localId, OfflineEventStatus status);
  Future<void> incrementRetry(String localId);
}

class OfflineSyncEngine {
  final IOfflineQueueRepository _repository;

  OfflineSyncEngine(this._repository);

  Future<void> syncPendingEvents() async {
    final pending = await _repository.getPendingEvents();
    for (var event in pending) {
      try {
        await _repository.updateEventStatus(event.localId, OfflineEventStatus.sending);
        await _repository.updateEventStatus(event.localId, OfflineEventStatus.succeeded);
      } catch (e) {
        await _repository.incrementRetry(event.localId);
      }
    }
  }
}
