# ADR-006: Supporting Libraries and Technical Framework Ecosystem

## Status
ACCEPTED

## Context
Untuk membangun Modular Monolith .NET 10 dan frontend Angular 21 yang tangguh, aman, dan dapat diuji secara terisolasi tanpa menciptakan duplikasi boilerplate, dibutuhkan pustaka standar industri (*battle-tested supporting libraries*) yang melengkapi fondasi utama (PostgreSQL, Redis, RabbitMQ, Camunda, MinIO, jsreport).

## Decision
Kami menyepakati ekosistem pustaka dan tools pendukung berikut:

### 1. Backend (.NET 10)
- **Message Bus & Outbox**: **MassTransit** — Mengabstraksi RabbitMQ, mengelola Transactional Outbox Pattern, retry policies, dan consumer lifecycle otomatis.
- **In-Process Mediation / CQRS**: **MediatR** — Mengatur pemisahan command dan query use-cases secara terisolasi per modul.
- **Input Validation**: **FluentValidation** — Validasi payload DTO sebelum masuk ke handler bisnis.
- **Result & Error Pattern**: **ErrorOr** / **Ardalis.Result** — Standarisasi functional error return tanpa throwing exceptions pada alur bisnis normal.
- **MinIO Integration**: **Minio.AspNetCore** — SDK resmi integrasi MinIO Object Storage.
- **Logging & Diagnostics**: **Serilog** terintegrasi dengan **Seq** (local/dev) dan OpenTelemetry standard.
- **Architecture Integrity**: **NetArchTest.eNhancedEdition** — Architecture tests otomatis untuk menegakkan aturan *Strict Module Rules* pada pipeline CI.

### 2. Frontend (Angular 21 Standalone)
- **Styling**: **Tailwind CSS v4** — Utility-first styling framework modern.
- **Icons**: **Lucide Angular** — Icon set SVG modern dan tree-shakeable.
- **Component Primitives & Drag-Drop**: **@angular/cdk/drag-drop** — Untuk interaksi Kanban board status creator roster dan modal dialogs.
- **Toast Notifications**: **ngx-sonner** — Toast notification ringan dan reaktif.

### 3. DevOps & Testing
- **Reverse Proxy & Auto-SSL**: **Caddy Server** — Reverse proxy modern dengan automasi sertifikat SSL Let's Encrypt zero-config.
- **Integration Testing**: **Testcontainers for .NET** — Menjalankan container PostgreSQL, Redis, dan RabbitMQ asli saat test runner berjalan.
- **E2E Testing**: **Playwright** — Otomasi pengujian browser end-to-end.

## Consequences
- **Positif**:
  - Menghilangkan boilerplate code manual untuk outbox pattern, validasi input, dan pesan queue.
  - Memastikan batas modular monolith tidak bocor (*enforced by NetArchTest*).
  - Mengurangi kompleksitas konfigurasi SSL/HTTPS pada Linux VPS dengan Caddy.
  - Pengujian integrasi menggunakan dependency nyata via Testcontainers menjamin keakuratan test.
- **Negatif**:
  - Menambah beberapa NuGet package dependencies yang harus dijaga versinya.
