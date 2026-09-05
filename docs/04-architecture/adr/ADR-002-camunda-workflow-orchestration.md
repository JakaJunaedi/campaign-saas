# ADR-002: Separation of Domain Business Rules and Camunda BPMN Orchestration

## Status
ACCEPTED

## Context
Alur kerja persetujuan konten kreator (*Creator Assignment, Submission, Review, Revision, Approval, Publishing*) memiliki tahapan yang dapat berulang (looping revisi), memiliki batas waktu (*deadlines*), dan membutuhkan visibilitas audit visual. Jika seluruh alur ini di-hardcode dalam if-else atau custom state machine, sistem akan sulit diaudit dan sulit dimodifikasi jika ada perubahan alur bisnis. Namun, jika seluruh aturan bisnis dipindahkan ke engine BPMN, logika domain akan tercecer di luar codebase C#.

## Decision
Kami memutuskan untuk memisahkan secara tegas antara **Domain Business Rules** dan **Workflow Orchestration**:
1. **Domain Layer (.NET 10)**: Memiliki 100% kepemilikan aturan validasi bisnis, invariant status, dan data persistence di PostgreSQL.
2. **Camunda BPMN Engine**: Hanya digunakan untuk *Workflow Orchestration* (urutan langkah, timer reminder batas waktu, user task assignment, dan visual execution history).
3. Aplikasi .NET 10 bertindak sebagai *External Task Worker* yang mengambil tugas dari Camunda REST API / gRPC.

## Alternatives Considered
- **Custom Code State Machine (Stateless / C# Enums)**: Cepat dibuat tetapi kurang memiliki kemampuan visual auditing dan pengelolaan timer asinkron yang tangguh.
- **Full Business Logic in BPMN (Embedded Scripts / DMN)**: Ditolak karena mempersulit unit testing dan melanggar prinsip Single Responsibility pada Domain Model.

## Consequences
- **Positif**:
  - Logika domain tetap dapat diuji secara terisolasi dengan unit test standar xUnit.
  - Manajemen alur revisi dan batas waktu deadline ditangani secara handal oleh Camunda scheduler.
  - Agensi mendapatkan visibilitas alur kerja visual.
- **Negatif**:
  - Menambah dependensi satu runtime service (Camunda platform container) yang membutuhkan alokasi RAM ~1.5 - 2 GB.

## Implementation Impact
- Backend .NET mengimplementasikan Background Worker untuk memproses Camunda External Tasks.
