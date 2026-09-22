# End-to-End Test Matrix

| Requirement | Workflow | Test | Service | Expected Result | Actual Result | Status | Evidence |
| :--- | :--- | :--- | :--- | :--- | :--- | :--- | :--- |
| ECTN Approval | Trader A creates ECTN | `SubmitEctn_ValidatesAndApproves` | ECTN | ECTN Issued | N/A | BLOCKED | Missing .NET SDK |
| Declaration | Trader A creates Declaration | `SubmitDeclaration_TriggersCustoms` | Declaration | Declaration Submitted | N/A | BLOCKED | Missing .NET SDK |
| Customs Config | Govt changes Tariff | `PublishTariff_PreservesHistory` | Customs | Historic=10%, New=15% | N/A | BLOCKED | Missing .NET SDK |
| Customs Risk | Evaluate High Risk | `EvaluateRisk_RequiresInspection` | Customs | Inspection Required | N/A | BLOCKED | Missing .NET SDK |
| Payment | Trader pays Assessment | `ConfirmPayment_EnablesClearance` | Payment | Payment Confirmed | N/A | BLOCKED | Missing .NET SDK |
| Release | Customs Authorizes Release | `AuthorizeRelease_EnablesFleet` | Customs | Release Authorized | N/A | BLOCKED | Missing .NET SDK |
| Fleet Dispatch | Logistics assigns Vehicle | `ApproveDispatch_TriggersTrip` | Fleet | Vehicle Dispatched | N/A | BLOCKED | Missing .NET SDK |
| Trip Tracking | Driver GPS updates | `ReceiveGps_UpdatesCorridor` | GPS/Corridor | Location Updated | N/A | BLOCKED | Missing .NET SDK |
| Offline GPS | Network Disconnected | `QueueGps_SyncsOnRestore` | GPS/Mobile | Events Synced Ordered | N/A | BLOCKED | Missing Flutter SDK |
| ANPR Gate | Vehicle arrives at border | `AnprRecognized_AuthorizesGate` | ANPR | Gate Authorized | N/A | BLOCKED | Missing .NET SDK |
| Delivery | Driver confirms POD | `ConfirmDelivery_CompletesTrip` | Corridor | Trip Completed | N/A | BLOCKED | Missing .NET SDK |
| Realtime UI | Event triggers SignalR | `ControlTower_UpdatesLiveMap` | BFF/Frontend| UI Re-renders automatically | N/A | BLOCKED | Missing .NET SDK |
| Tenant Isolation | Logistics A requests Trip B | `AccessTrip_CrossTenant_Denies` | All Services| 403 Forbidden / 404 | N/A | BLOCKED | Missing .NET SDK |
