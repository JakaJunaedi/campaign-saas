# ADR-002: Separation of Domain Business Rules and Camunda BPMN Orchestration

## Status
ACCEPTED (Phased: MVP In-Process DDD State Machine + MassTransit Outbox | Phase 6 Camunda Orchestration)

## Context
Alur kerja persetujuan konten kreator (*Creator Assignment, Submission, Review, Revision, Approval, Publishing*) memiliki tahapan yang dapat berulang (looping revisi), memiliki batas waktu (*deadlines*), dan membutuhkan visibilitas audit visual. Jika seluruh alur ini di-hardcode dalam if-else atau custom state machine tanpa struktur domain yang jelas, sistem akan sulit diaudit. Namun, jika seluruh aturan bisnis dipindahkan ke engine BPMN, logika domain akan tercecer di luar codebase C#.

## Decision
Kami memutuskan untuk memisahkan secara tegas antara **Domain Business Rules** dan **Workflow Orchestration** dengan strategi bertahap:
1. **MVP Phase (Phase 1-5)**: Menggunakan **In-Process DDD State Machine** pada Domain Entity (`Deliverable`, `ContentSubmission`, `ApprovalReview`) yang dipadukan dengan **MassTransit Transactional Outbox + RabbitMQ** untuk *event-driven choreography* dan audit trail tanpa dependensi wajib ke engine eksternal.
2. **Post-MVP / Phase 6**: Menghubungkan **Camunda BPMN Engine (7.20 / Zeebe)** sebagai *External Orchestrator* untuk SLA timers, reminder escalations, dan diagram visual execution history.
3. **Domain Sovereignty**: Seluruh aturan validasi bisnis, status invariants, dan data persistence PostgreSQL 100% tetap berada di Domain Layer (.NET 10). Backend .NET bertindak sebagai *External Task Worker*.

## Alternatives Considered
- **Custom Code State Machine (Stateless / C# Enums)**: Cepat dibuat tetapi kurang memiliki kemampuan visual auditing dan pengelolaan timer asinkron yang tangguh.
- **Full Business Logic in BPMN (Embedded Scripts / DMN)**: Ditolak karena mempersulit unit testing dan melanggar prinsip Single Responsibility pada Domain Model.

## Consequences
- **Positif**:
  - Logika domain tetap dapat diuji secara terisolasi dengan unit test standar xUnit (127+ tests).
  - MVP dapat berjalan ringan dan handal tanpa hard dependency pada Camunda runtime di awal.
  - Skalabilitas ke Camunda BPMN visual orchestration tetap terakomodasi di Phase 6.
- **Negatif**:
  - Di fase MVP, visual audit trail bergantung pada tabel `audit_logs` dan Angular activity feed, sebelum dashboard BPMN Camunda diaktifkan penuh di Phase 6.

## Implementation Impact
- Phase 1-5: Domain aggregate state transitions, domain events (`DeliverableStatusChangedDomainEvent`), MassTransit outbox consumers.
- Phase 6: Background Worker `Camunda.Worker` untuk mengonsumsi external tasks dan mengendalikan BPMN process instances.
