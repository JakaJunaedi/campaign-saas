# Campaign SaaS — DevOps, Runtime & QA Conventions

Panduan ini dibaca sebelum menulis/mengubah konfigurasi Docker, CI/CD, deployment, atau pipeline testing di project Campaign SaaS. Wajib dipatuhi bersama `AGENTS.md` sebagai source of truth utama.

## Runtime & Infrastructure

- **Target runtime**: Linux VPS, dijalankan via **Docker Compose** (bukan Kubernetes — eksplisit out-of-scope MVP, lihat AGENTS.md Section 6).
- **Reverse Proxy & SSL**: **Caddy Server** (auto-SSL Let's Encrypt) sebagai default; NGINX hanya dipakai kalau ada kebutuhan spesifik yang tidak bisa dipenuhi Caddy.
- Semua service (API, Postgres, RabbitMQ, Redis, MinIO, Camunda/Zeebe, jsreport) didefinisikan sebagai service terpisah di `docker-compose.yml`, bukan digabung dalam satu container.
- Jangan tambahkan orchestrator baru (Kubernetes, Nomad, dll) tanpa ADR baru — ini melanggar STRICT MODULE RULES #10 (no new tech tanpa justifikasi arsitektur).

## Environment & Configuration

- Konfigurasi sensitif (connection string, secret key, credential MinIO/RabbitMQ) WAJIB lewat environment variable / secret manager — jangan pernah hardcode atau commit ke repo.
- Sediakan `docker-compose.override.yml` atau `.env.example` untuk local development, dengan nilai dummy/non-produksi.
- Pisahkan konfigurasi per environment: `local`, `staging`, `production` — jangan pakai satu file config untuk semua environment.

## CI/CD (GitHub Actions)

Pipeline minimal WAJIB mencakup tahapan berikut, berurutan, dan build gagal jika salah satu gagal:

1. **Restore & Build** — `dotnet restore` + `dotnet build` untuk backend, `npm ci` + build untuk frontend.
2. **Architecture Test** — jalankan `NetArchTest.eNhancedEdition` suite; kegagalan berarti ada pelanggaran STRICT MODULE RULES, build harus gagal (bukan warning).
3. **Unit Test** — xUnit (backend) dan unit test Angular (frontend), dengan coverage report.
4. **Integration Test** — pakai **Testcontainers for .NET**, spin up PostgreSQL/RabbitMQ/Redis/MinIO asli di CI runner.
5. **E2E Test (Playwright)** — dijalankan minimal di branch `main`/sebelum merge ke `main`, mencakup core workflow utama.
6. **Docker Build & Push** — hanya dijalankan setelah semua test di atas lulus.

- Jangan skip architecture test atau integration test demi mempercepat pipeline — kegagalan modul boundary harus terdeteksi sebelum merge.
- Gunakan matrix/cache (`actions/cache`) untuk dependency restore supaya pipeline tidak lambat, tapi jangan cache hasil test.

## Logging & Observability

- **Serilog** dipakai untuk structured logging di seluruh backend, dikirim ke **Seq** untuk local/staging, dan/atau **OpenTelemetry** exporter untuk production observability.
- Setiap log WAJIB menyertakan context minimal: `OrganizationId`, `UserId` (kalau ada), `CorrelationId`/`TraceId`.
- Jangan log data sensitif (password, token, PII lengkap) — mask atau exclude field tersebut.
- Setiap request lintas service (API → MassTransit consumer → Camunda) harus bisa ditelusuri lewat satu `CorrelationId` yang sama.

## Messaging Infrastructure (RabbitMQ + MassTransit)

- Queue/exchange naming konsisten: `<modul>.<event-name>` (contoh: `approval.content-submitted`).
- Dead-letter queue WAJIB dikonfigurasi untuk setiap consumer penting — jangan biarkan pesan gagal hilang begitu saja.
- Retry policy dan circuit breaker didefinisikan eksplisit per consumer di konfigurasi MassTransit, bukan mengandalkan default framework.

## Object Storage (MinIO) — Operasional

- Bucket dipisah per keperluan (contoh: `content-submissions`, `reports`), bukan satu bucket besar untuk semua jenis file.
- Lifecycle policy (retention/expiry) untuk file temporary/draft harus dikonfigurasi, supaya storage tidak membengkak tanpa kontrol.
- Backup/replication MinIO menjadi bagian dari runbook disaster recovery — dokumentasikan di `docs/04-architecture/INFRASTRUCTURE.md`.

## Workflow Engine (Camunda/Zeebe) — Operasional

- Camunda dijalankan sebagai service terpisah di Docker Compose, bukan embedded di dalam proses API.
- BPMN diagram disimpan sebagai file versi-terkontrol di repo (bukan hanya di Camunda Modeler lokal developer).
- Perubahan BPMN proses WAJIB melalui review, karena mengubah urutan business process yang berdampak ke banyak modul (Approval, Notification).

## Testing Strategy — Ringkasan Prioritas

| Jenis Test | Tools | Kapan Dijalankan |
|---|---|---|
| Architecture Test | NetArchTest.eNhancedEdition | Setiap PR |
| Unit Test | xUnit, FluentAssertions | Setiap PR |
| Integration Test | Testcontainers for .NET | Setiap PR (bisa lebih lambat, tapi wajib) |
| E2E Test | Playwright | Sebelum merge ke `main` / nightly |

- **Jangan hapus test yang sudah ada** tanpa alasan eksplisit yang didokumentasikan di PR description (lihat AGENTS.md Section 8 — Maintain Test Integrity).
- Integration test tidak boleh mock database/messaging — gunakan Testcontainers supaya perilaku sesuai production.

## Release & Deployment

- Deployment ke production WAJIB lewat pipeline CI/CD, tidak ada manual deploy langsung ke VPS.
- Migration database (`dotnet ef database update`) dijalankan sebagai step terpisah dan eksplisit dalam pipeline deploy, bukan otomatis jalan saat aplikasi start (`Database.Migrate()` di `Program.cs` dihindari untuk production).
- Setiap release harus punya rollback plan minimal (versi image sebelumnya bisa langsung di-redeploy).

## Checklist Sebelum Merge/Deploy

- [ ] Semua tahap CI (build, architecture test, unit test, integration test) lulus
- [ ] Tidak ada secret/credential yang ter-commit ke repo
- [ ] Logging baru menyertakan `OrganizationId`/`CorrelationId` yang relevan
- [ ] Queue/consumer baru punya dead-letter queue & retry policy eksplisit
- [ ] Perubahan BPMN sudah direview terpisah dari perubahan kode biasa
- [ ] Migration database dijalankan sebagai step eksplisit, bukan auto-migrate saat startup
- [ ] Tidak menambahkan orchestrator/teknologi baru tanpa ADR