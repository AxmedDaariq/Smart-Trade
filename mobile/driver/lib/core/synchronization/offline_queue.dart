import 'dart:convert';
import 'package:uuid/uuid.dart';

enum OfflineEventStatus { Pending, Sending, Succeeded, Failed, DeadLetter }

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
        status = OfflineEventStatus.Pending;
}

abstract class IOfflineQueueRepository {
  Future<void> enqueue(OfflineEvent event);
  Future<List<OfflineEvent>> getPendingEvents();
  Future<void> updateEventStatus(String localId, OfflineEventStatus status);
  Future<void> incrementRetry(String localId);
}

class OfflineSyncEngine {
  final IOfflineQueueRepository _repository;
  // final ApiClient _apiClient;

  OfflineSyncEngine(this._repository);

  Future<void> syncPendingEvents() async {
    final pending = await _repository.getPendingEvents();
    for (var event in pending) {
      try {
        await _repository.updateEventStatus(event.localId, OfflineEventStatus.Sending);
        // Simulate API call using ApiClient
        // await _apiClient.post('/api/sync', data: event.payload);
        await _repository.updateEventStatus(event.localId, OfflineEventStatus.Succeeded);
      } catch (e) {
        // Handle permanent vs transient errors
        await _repository.incrementRetry(event.localId);
        // If max retries, move to DeadLetter
      }
    }
  }
}
