# Campaign SaaS — Backend Development Conventions

Panduan ini dibaca sebelum menulis atau mengubah kode di modul backend **Campaign SaaS** (.NET 10, Modular Monolith). Wajib dipatuhi bersama `AGENTS.md` sebagai source of truth utama.

## Konteks Produk

Platform B2B SaaS untuk Influencer/KOL Agency. Core workflow:
```
Client → Campaign → Creator Selection → Creator Assignment → Deliverable →
Content Submission → Review → Revision → Approval → Publishing → Reporting
```

## Struktur Modular Monolith

Project terbagi jadi 9 modul independen, masing-masing punya struktur layer sendiri (Domain/Application/Infrastructure/Endpoints):

```
Modules/
├── Identity/          # Auth, Organizations (tenant boundary), Users, RBAC
├── Client/            # Client brands & PIC contacts
├── Campaign/          # Campaign lifecycle, budget, dates, status
├── Creator/           # Creator CRM database & social accounts
├── Deliverable/       # Deliverable items, deadlines, platforms
├── Approval/          # Content submission (MinIO), versioning, review loop
├── Reporting/         # Manual metrics, jsreport PDF generation
├── Notification/      # In-app notifications
└── Audit/             # Audit trail logging
```

Setiap modul mengikuti internal layering:
```
Modules/<NamaModul>/
├── Domain/            # Entity, Value Object, Domain Event, Enum
├── Application/       # MediatR Commands/Queries/Handlers, Validators, DTOs
├── Infrastructure/    # DbContext-per-modul, Repository, EF Configurations
└── Endpoints/         # Minimal API endpoint (thin, tanpa business logic)
```

## STRICT MODULE RULES — Non-Negotiable

Sebelum menulis kode lintas modul, cek dulu:

1. **Jangan** akses `DbContext` atau repository modul lain secara langsung.
2. **Jangan** referensikan Entity modul lain sebagai navigation property.
3. **Jangan** buat circular dependency antar modul.
4. Business rule harus tinggal di modul domain yang bersangkutan — jangan bocor ke modul lain.
5. Referensi lintas modul WAJIB pakai primitive identifier (`Guid`), bukan entity reference.
   - Benar: `Deliverable.CreatorId` (Guid)
   - Salah: `Deliverable.Creator` (navigation ke entity modul lain)
6. Infrastructure layer tidak boleh mengandung business logic.
7. Endpoint/Controller tidak boleh mengandung business logic — hanya menerima request → kirim ke MediatR → kembalikan response.
8. Jangan membuat microservice baru selama fase MVP.
9. Jangan memecah database per modul selama MVP (tetap single PostgreSQL, terpisah secara logical schema/namespace saja).
10. Jangan menambahkan teknologi baru tanpa architectural justification (ADR).

Jika mengerjakan task yang butuh data dari modul lain, gunakan salah satu dari:
- MediatR request lintas modul (in-process), atau
- Domain Event via MassTransit (Outbox pattern), atau
- Contract/interface yang di-expose modul pemilik data.

## CQRS dengan MediatR

- Setiap use case = satu Command atau Query + satu Handler.
- Command untuk write, Query untuk read — jangan dicampur.
- Naming: `CreateCampaignCommand`, `CreateCampaignCommandHandler`, `GetCampaignByIdQuery`, dst.
- Command/Query diletakkan di `Application/Commands` atau `Application/Queries`, dikelompokkan per fitur (vertical slice), bukan per teknis layer.

```csharp
public record CreateCampaignCommand(string Name, Guid ClientId, DateOnly StartDate) : IRequest<ErrorOr<CampaignResponse>>;

public class CreateCampaignCommandHandler : IRequestHandler<CreateCampaignCommand, ErrorOr<CampaignResponse>>
{
    // constructor injection: repository/interface, bukan DbContext langsung
}
```

## Validation (FluentValidation)

- Satu Validator per Command/Query, naming `<NamaCommand>Validator`.
- Didaftarkan otomatis lewat MediatR Pipeline Behavior — jangan panggil `.Validate()` manual di handler.
- Validasi format/required di sini; validasi business rule (misalnya "budget tidak boleh melebihi kontrak") tetap di domain/handler.

## Result Pattern (ErrorOr)

- Semua Handler mengembalikan `ErrorOr<T>`, bukan throw exception untuk expected error (not found, validation, conflict).
- Exception hanya untuk unexpected/unhandled error.
- Endpoint me-mapping `ErrorOr<T>` ke RFC 7807 `ProblemDetails` lewat extension method terpusat — jangan mapping manual di tiap endpoint.

```csharp
return result.Match(
    success => Results.Ok(success),
    errors => Results.Problem(errors.ToProblemDetails())
);
```

## Endpoint (Minimal API)

- Endpoint didefinisikan di `Endpoints/`, dikelompokkan per resource, pakai `MapGroup`.
- Endpoint hanya: bind request → kirim `IMediator.Send()` → return hasil mapping.
- Tidak ada logic kondisional bisnis di endpoint.

## Multi-Tenancy — WAJIB di Setiap Entity Baru

- Setiap entity bisnis WAJIB punya kolom `OrganizationId`.
- Setiap `DbContext` WAJIB menerapkan global query filter:
```csharp
modelBuilder.Entity<T>().HasQueryFilter(e =>
    e.OrganizationId == _tenantContext.OrganizationId && !e.IsDeleted);
```
- Server-side enforcement wajib dicek eksplisit di handler untuk operasi sensitif (bukan hanya mengandalkan query filter):
```csharp
if (CurrentUser.OrganizationId != resource.OrganizationId)
    return Error.Forbidden();
```
- Jangan pernah percaya `organizationId` dari request body/query param untuk otorisasi — selalu ambil dari `CurrentUser`/tenant context yang sudah tervalidasi token.

## File Storage (MinIO)

- Jangan simpan binary file di database.
- Upload/download selalu lewat Pre-Signed URL, di-generate oleh backend.
- Validasi izin akses (organization + role) dilakukan server-side SEBELUM generate presigned URL, bukan mengandalkan URL yang sulit ditebak sebagai satu-satunya proteksi.
- Content submission di modul Approval wajib mendukung versioning (jangan overwrite file lama).

## Messaging & Outbox (MassTransit + RabbitMQ)

- Event antar modul dikirim sebagai Domain Event → di-publish lewat MassTransit Outbox (transactional dengan DB write, bukan fire-and-forget terpisah).
- Consumer diletakkan di modul yang butuh bereaksi terhadap event, bukan di modul pemilik event.
- Naming event: past-tense, contoh `CampaignApprovedEvent`, `DeliverableSubmittedEvent`.
- Retry policy didefinisikan eksplisit di consumer configuration, jangan andalkan default.

## Workflow Engine (Camunda BPMN)

- Camunda dipakai HANYA untuk orkestrasi proses (review-revision-approval loop), bukan untuk business logic detail.
- Business rule tetap di domain/handler; BPMN hanya mengatur urutan state & siapa yang bertanggung jawab di tiap step.
- Jangan taruh kalkulasi atau validasi kompleks di dalam BPMN script task — panggil balik ke service/handler.

## Reporting & Document Generation (jsreport)

- PDF generation berjalan asinkron via RabbitMQ worker, bukan synchronous request-response.
- Metrics (Reach, Impression, Views, Engagement) di-input manual — jangan asumsikan ada integrasi API sosial media otomatis (eksplisit out-of-scope MVP).

## Architecture Testing (NetArchTest)

- Setiap PR yang menyentuh struktur modul WAJIB lolos architecture test yang memverifikasi:
  - Modul tidak mereferensikan `DbContext`/repository modul lain.
  - Infrastructure tidak direferensikan dari Domain.
  - Endpoint tidak mereferensikan Infrastructure secara langsung (harus lewat Application).
- Architecture test dijalankan di CI (GitHub Actions), kegagalan = build gagal, bukan warning.

## Testing

- Unit test: **xUnit** + **FluentAssertions**, satu class test per handler/service.
- Integration test: **Testcontainers for .NET** — spin up PostgreSQL, RabbitMQ, Redis, MinIO asli di container, jangan mock infrastruktur untuk integration test.
- E2E test: **Playwright**, mencakup core workflow utama (Campaign → Deliverable → Approval).
- Penamaan method test: `MethodName_Scenario_ExpectedResult`.
- **Jangan hapus unit test** tanpa alasan eksplisit (lihat AGENTS.md Section 8).

## Explicit MVP Out-of-Scope — Jangan Dikerjakan

Kalau ada request yang menyentuh area ini, tandai sebagai `OPEN QUESTION`, jangan langsung diimplementasikan:
- Influencer marketplace / public creator discovery
- Automated social media scraping / direct social API integration
- Payment gateway & automated creator payouts
- Mobile native app (iOS/Android)
- White-label custom domain
- Kubernetes cluster setup

## Naming Convention Ringkas

- Class/Method/Property: `PascalCase`
- Parameter/local variable: `camelCase`
- Interface: prefix `I`
- Private field: `_camelCase`
- Async method: suffix `Async`
- Command/Query: suffix `Command`/`Query`, Handler suffix `Handler`
- Domain Event: past-tense + suffix `Event`

## Operating Principle (dari AGENTS.md)

Prioritas saat ada trade-off:
**Correctness > Clarity > Consistency > Scope Control > Maintainability > Speed**

- **Do Not Guess** — kalau requirement belum jelas, tandai sebagai `OPEN QUESTION` atau `PROPOSED DECISION`, jangan asumsi sendiri.
- **Traceability** — setiap fitur harus bisa ditelusuri: PRD → Domain → Database → API → UI → Test.
- Sebelum perubahan arsitektur, cek dokumen terkait di `docs/04-architecture/` (ARCHITECTURE.md, DOMAIN_MODEL.md, DATABASE_DESIGN.md, ADR).

## Checklist Sebelum Commit/PR

- [ ] Tidak melanggar STRICT MODULE RULES (cek dependency antar modul)
- [ ] Entity baru punya `OrganizationId` + global query filter
- [ ] Otorisasi organization dicek eksplisit di handler, bukan cuma query filter
- [ ] Command/Query pakai `ErrorOr<T>`, bukan throw untuk expected error
- [ ] Endpoint tetap thin — tidak ada business logic
- [ ] FluentValidation validator dibuat untuk Command/Query baru
- [ ] Architecture test (NetArchTest) tidak gagal
- [ ] Unit test ditambahkan untuk handler baru, tidak ada test lama yang dihapus tanpa alasan
- [ ] Fitur di luar MVP scope ditandai `OPEN QUESTION`, bukan langsung dikerjakan