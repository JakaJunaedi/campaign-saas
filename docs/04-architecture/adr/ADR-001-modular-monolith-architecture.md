# ADR-001: Adoption of Modular Monolith Architecture for MVP

## Status
ACCEPTED

## Context
Platform Campaign SaaS membutuhkan kecepatan rilis (time-to-market), batasan operasional yang sederhana (single-node VPS deployment), dan kemudahan pengelolaan data transaksional yang konsisten (ACID). Pendekatan Microservices di fase awal akan memperkenalkan kompleksitas terdistribusi berlebih (distributed tracing, network latency, distributed transactions, dan overhead operasional). Namun, sistem harus tetap siap diekstraksi menjadi microservices di masa depan seiring pertumbuhan skala bisnis.

## Decision
Kami memutuskan untuk membangun sistem backend sebagai **Modular Monolith** berbasis .NET 10 (C# 14).
1. Sistem dideploy sebagai satu executable host API monolitik tunggal.
2. Logika bisnis dipecah secara ketat ke dalam 9 modul domain independen (*Identity, Client, Campaign, Creator, Deliverable, Approval, Reporting, Notification, Audit*).
3. Setiap modul memiliki 4-layer architecture (*Domain, Application, Infrastructure, Contracts*).
4. Komunikasi lintas modul diatur via contract interfaces (in-process) dan event-driven integration events (RabbitMQ).
5. Modul tidak boleh mengakses repository atau Entity modul lain secara langsung.

## Alternatives Considered
- **Pure Monolith (Single Layered/Spaghetti)**: Ditolak karena sulit dipelihara dan rawan tight coupling antar domain.
- **Microservices Deployment dari Hari Pertama**: Ditolak karena overhead infrastruktur (Kubernetes, Service Mesh, Network partitions) tidak proporsional untuk MVP.

## Consequences
- **Positif**:
  - Pengembangan sangat cepat dengan kemudahan debugging lokal dan refactoring terpadu.
  - Deployment sederhana (1 Docker container API + 1 container SPA + container dependencies).
  - Transaksi data tetap konsisten tanpa beban 2-phase commit (2PC) / saga orchestration di semua endpoint.
  - Batas modul yang tegas memungkinkan ekstraksi ke microservices di masa depan menggunakan *Strangler Pattern* tanpa penulisan ulang arsitektur besar.
- **Negatif**:
  - Membutuhkan disiplin tinggi dari tim dan AI Coding Agent agar tidak melanggar *Strict Module Rules* (seperti bypass DbContext atau entity navigation langsung).

## Implementation Impact
- Menggunakan Roslyn analyzers atau Architecture Unit Tests (NetArchTest) untuk memverifikasi isolasi modul pada pipeline CI.
