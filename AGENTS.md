# AGENTS.md — AI & Developer Operating Guidelines

Selamat datang di repositori **Campaign SaaS** — B2B SaaS Campaign Management Platform untuk Influencer/KOL Agency.

Dokumen ini adalah **Entry Point dan Source of Truth utama bagi AI Coding Agents** (Antigravity, Cursor, Copilot) dan Software Engineers.

---

## 🎯 1. Gambaran Produk & Core Workflow

Platform ini mengotomasi siklus hidup campaign bagi agensi influencer/KOL:
```text
Client → Campaign → Creator Selection → Creator Assignment → Deliverable → Content Submission → Review → Revision → Approval → Publishing → Reporting
```

---

## 🏗️ 2. Technology Stack & Ekosistem Lengkap

### Backend Core & Libraries (.NET 10 / C# 14)
- **Framework**: ASP.NET Core 10, Entity Framework Core 10
- **Database**: PostgreSQL 16+ (Single Source of Truth untuk seluruh data bisnis)
- **Messaging & Outbox**: RabbitMQ + **MassTransit** (Transactional Outbox, Retry, Consumers)
- **CQRS Mediation**: **MediatR**
- **Validation**: **FluentValidation**
- **Result Pattern**: **ErrorOr** / **Ardalis.Result** (RFC 7807 ProblemDetails)
- **Cache & Rate Limit**: Redis
- **Workflow Engine**: Camunda BPMN Platform 7.20 / Zeebe (Orchestration only)
- **Object Storage**: MinIO (**Minio.AspNetCore**) via Pre-Signed URLs
- **Document Engine**: jsreport (PDF generation asinkron via RabbitMQ worker)
- **Logging**: Serilog + Seq / OpenTelemetry
- **Architecture Testing**: **NetArchTest.eNhancedEdition** (Memverifikasi batas modul di CI)

### Frontend Core & Libraries (Angular 21 Standalone)
- **Framework**: Angular 21 (Standalone Components, Signals, Modern Control Flow `@if/@for/@defer`)
- **Language**: TypeScript 5.x
- **Styling**: **Tailwind CSS v4**
- **Icons**: **Lucide Angular**
- **CDK Utilities**: **@angular/cdk/drag-drop** (Kanban Roster Board)
- **Notifications**: **ngx-sonner**

### DevOps, Runtime & QA
- **Runtime**: Linux VPS, Docker, Docker Compose
- **Reverse Proxy & SSL**: **Caddy Server** (Auto-SSL Let's Encrypt) / NGINX
- **CI/CD**: GitHub Actions
- **Testing**: xUnit, FluentAssertions, **Testcontainers for .NET**, **Playwright** (E2E)

---

## 🏛️ 3. Arsitektur: Modular Monolith (Microservices-Ready)

Sistem dibangun sebagai **Modular Monolith** dengan 9 modul independen:
```plaintext
Modules/
├── Identity/          # Auth, Organizations (Tenant Boundary), Users, RBAC Roles
├── Client/            # Client brands & PIC contacts
├── Campaign/          # Campaign lifecycle, budget, dates, status
├── Creator/           # Creator CRM database & social accounts
├── Deliverable/       # Deliverable items, deadlines, platforms
├── Approval/          # Content submissions (MinIO), versioning, review & revision loop
├── Reporting/         # Manual metrics (Reach, Impr, Views, Engagement), jsreport PDF
├── Notification/      # In-app notifications
└── Audit/             # Audit trail logging
```

---

## ⛔ 4. STRICT MODULE RULES (Wajib Dipatuhi)

1. **Rule 1**: Modul TIDAK BOLEH mengakses repository atau `DbContext` modul lain secara langsung.
2. **Rule 2**: Modul TIDAK BOLEH menggunakan Entity modul lain sebagai dependency/foreign entity navigation.
3. **Rule 3**: Hindari circular dependency antar modul.
4. **Rule 4**: Aturan bisnis (*business rules*) harus dimiliki sepenuhnya oleh modul domain yang bersangkutan.
5. **Rule 5**: Gunakan Identifier primitif (`Guid`) dan Contract untuk referensi lintas modul (contoh: `Deliverable.CreatorId`, BUKAN `Deliverable.CreatorEntity`).
6. **Rule 6**: Layer Infrastructure tidak boleh mengandung business logic.
7. **Rule 7**: Controller / Minimal API endpoints tidak boleh mengandung business logic.
8. **Rule 8**: DILARANG membuat microservice baru selama fase MVP.
9. **Rule 9**: DILARANG memecah database menjadi multi-database per modul selama MVP.
10. **Rule 10**: DILARANG menambahkan teknologi baru tanpa architectural justification.

---

## 🛡️ 5. Multi-Tenancy & Security Rules

1. **Organization Boundary**: Seluruh entitas bisnis WAJIB menyertakan kolom `organization_id`.
2. **Server-Side Enforcement**: Backend WAJIB memastikan:
   ```csharp
   CurrentUser.OrganizationId == Resource.OrganizationId
   ```
3. **EF Core Global Query Filter**: Terapkan filter otomatis pada setiap query database:
   ```csharp
   modelBuilder.Entity<T>().HasQueryFilter(e => e.OrganizationId == _tenantContext.OrganizationId && !e.IsDeleted);
   ```
4. **Secure File Access**: Jangan pernah simpan binary file di database. Gunakan MinIO Pre-Signed URL dengan validasi izin akses server-side.

---

## 🚫 6. Explicit MVP Out-of-Scope (Dilarang Dibuat)

- Influencer marketplace / public creator discovery engine
- Automated social media scraping / direct social API integration
- Payment gateway & automated creator payouts
- Mobile native application (iOS/Android)
- White-label custom domain
- Kubernetes cluster setup

---

## 📚 7. Dokumentasi Lengkap & Source of Truth

Sebelum menulis kode atau membuat perubahan arsitektur, AI Agent **WAJIB** membaca dokumen terkait:

- **Product Requirements**: [PRD.md](file:///d:/Jack/dotnet/campaign-saas/docs/01-product/PRD.md)
- **Scope & Boundaries**: [MVP_SCOPE.md](file:///d:/Jack/dotnet/campaign-saas/docs/01-product/MVP_SCOPE.md)
- **System Architecture**: [ARCHITECTURE.md](file:///d:/Jack/dotnet/campaign-saas/docs/04-architecture/ARCHITECTURE.md)
- **Domain Model**: [DOMAIN_MODEL.md](file:///d:/Jack/dotnet/campaign-saas/docs/04-architecture/DOMAIN_MODEL.md)
- **Database Design**: [DATABASE_DESIGN.md](file:///d:/Jack/dotnet/campaign-saas/docs/04-architecture/DATABASE_DESIGN.md)
- **Security & RBAC**: [SECURITY_MODEL.md](file:///d:/Jack/dotnet/campaign-saas/docs/04-architecture/SECURITY_MODEL.md)
- **Workflow & BPMN**: [WORKFLOW_ARCHITECTURE.md](file:///d:/Jack/dotnet/campaign-saas/docs/04-architecture/WORKFLOW_ARCHITECTURE.md)
- **File Storage**: [FILE_STORAGE.md](file:///d:/Jack/dotnet/campaign-saas/docs/04-architecture/FILE_STORAGE.md)
- **Infrastructure**: [INFRASTRUCTURE.md](file:///d:/Jack/dotnet/campaign-saas/docs/04-architecture/INFRASTRUCTURE.md)
- **Architectural Decisions (ADR)**: [ADR-001 hingga ADR-006](file:///d:/Jack/dotnet/campaign-saas/docs/04-architecture/adr/)
- **REST API Contract**: [API_CONTRACT.md](file:///d:/Jack/dotnet/campaign-saas/docs/05-api/API_CONTRACT.md)
- **Error Handling**: [ERROR_HANDLING.md](file:///d:/Jack/dotnet/campaign-saas/docs/05-api/ERROR_HANDLING.md)
- **Implementation Plan**: [IMPLEMENTATION_PLAN.md](file:///d:/Jack/dotnet/campaign-saas/docs/06-implementation/IMPLEMENTATION_PLAN.md)
- **Quality & Test Plan**: [TEST_PLAN.md](file:///d:/Jack/dotnet/campaign-saas/docs/07-quality/TEST_PLAN.md)
- **Acceptance Criteria**: [ACCEPTANCE_CRITERIA.md](file:///d:/Jack/dotnet/campaign-saas/docs/07-quality/ACCEPTANCE_CRITERIA.md)

---

## 🤖 8. AI Agent Operating Principles

> **PRIORITAS UTAMA**:
> **Correctness > Clarity > Consistency > Scope Control > Maintainability > Speed**

- **Do Not Guess**: Jika requirement belum ditentukan, tandai sebagai `OPEN QUESTION` atau `PROPOSED DECISION`.
- **Maintain Test Integrity**: Jangan menghapus unit test tanpa alasan.
- **Traceability**: Setiap requirement harus dapat dilacak dari PRD → Domain → Database → API → UI → Test.
