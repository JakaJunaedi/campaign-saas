# Campaign SaaS — Influencer & KOL Agency Platform

Campaign SaaS adalah platform multi-tenant berbasis **.NET 10** dan **Angular 21 (Standalone)** yang dirancang khusus untuk **Influencer / KOL Agency** guna mengelola, mengorkestrasi, dan mereport kampanye pemasaran digital dari awal hingga selesai.

---

## 🚀 Core Workflow

```text
Client
  ↓
Campaign
  ↓
Creator Selection
  ↓
Creator Assignment
  ↓
Deliverable
  ↓
Content Submission (MinIO Versioning)
  ↓
Review & Revision (Camunda BPMN)
  ↓
Approval
  ↓
Publishing & Live Proof
  ↓
Manual Metrics & Reporting (jsreport PDF)
```

---

## 🛠️ Complete Technology Stack

### Backend (.NET 10 / C# 14)
- **Core**: ASP.NET Core 10, Entity Framework Core 10 (PostgreSQL 16+)
- **Messaging & Outbox**: RabbitMQ + **MassTransit**
- **CQRS & Validation**: **MediatR** + **FluentValidation** + **ErrorOr**
- **Workflow**: Camunda BPMN Platform 7.20
- **Object Storage**: MinIO (**Minio.AspNetCore**)
- **Document Engine**: jsreport
- **Cache**: Redis
- **Architecture Testing**: NetArchTest.eNhancedEdition

### Frontend (Angular 21 Standalone)
- **Core**: Angular 21 (Standalone Components, Signals, Modern Control Flow)
- **Styling & UI**: **Tailwind CSS v4**, **Lucide Angular**, **@angular/cdk/drag-drop**
- **Notifications**: ngx-sonner

### DevOps & QA
- **Runtime**: Linux VPS, Docker Compose, **Caddy Server** (Auto-SSL)
- **Testing**: xUnit, FluentAssertions, **Testcontainers for .NET**, **Playwright**

---

## 📁 Repository & Documentation Structure

```plaintext
campaign-saas/
├── AGENTS.md                          # Source of Truth & Guide untuk AI Coding Agents
├── README.md                          # Ringkasan proyek & navigasi
├── docs/
│   ├── 01-product/                    # PRD, Vision, ICP, Personas, MVP Scope, Open Questions
│   ├── 02-business/                   # Business Model, Pricing, KPIs
│   ├── 03-design/                     # User Flows, UX Requirements, UI Screen Map
│   ├── 04-architecture/               # Architecture, Domain Model, DB Design, Security, BPMN, ADRs
│   ├── 05-api/                        # API Contract, RFC 7807 Error Handling
│   ├── 06-implementation/             # Implementation Plan, Roadmap, Local Dev Guide
│   └── 07-quality/                    # Master Test Plan, Acceptance Criteria, Release Checklist
```

---

## 🚦 Memulai Pengembangan

Lihat panduan lengkap:
1. [LOCAL_DEVELOPMENT_GUIDE.md](file:///d:/Jack/dotnet/campaign-saas/docs/06-implementation/LOCAL_DEVELOPMENT_GUIDE.md) — Panduan Docker Compose & environment lokal.
2. [IMPLEMENTATION_PLAN.md](file:///d:/Jack/dotnet/campaign-saas/docs/06-implementation/IMPLEMENTATION_PLAN.md) — Urutan pengerjaan fitur per modul.
3. [AGENTS.md](file:///d:/Jack/dotnet/campaign-saas/AGENTS.md) — Aturan wajib pengembang dan AI agent.

---

## 📊 Module Implementation Status

Penomoran phase mengacu pada [IMPLEMENTATION_PLAN.md](file:///d:/Jack/dotnet/campaign-saas/docs/06-implementation/IMPLEMENTATION_PLAN.md) (12 phase).

- [x] **Phase 0: Foundation Setup** (Solution Structure, SharedKernel, EF Core, Docker Compose)
- [x] **Phase 1: Identity & Multi-Tenancy** (Org, Users, JWT Auth, RBAC, Tenant Query Filters)
- [x] **Phase 2: Client Module** (Client CRUD, Contacts JSONB)
- [x] **Phase 3: Campaign Module** (Campaign Entity, Status Lifecycle, Dates & Budget)
- [x] **Phase 4: Creator Module** (Creator CRM, Social Accounts JSONB)
- [x] **Phase 5: Campaign Creator Roster** (Assignment, Status Transition)
- [x] **Phase 6: Deliverable Module** (Deliverable Items, Deadlines, Platform Types)
- [x] **Phase 7: Content Submission & MinIO Storage** (Presigned Upload, Versioning V1/V2)
- [x] **Phase 8: Approval & Review Module** (Review Decision, Feedback Comments, Submission History)
- [x] **Phase 9: Publishing & Manual Metrics** (Live URLs, Reach/Impressions/Views/Likes/Comments/Engagement)
- [x] **Phase 10: Reporting Module** (jsreport PDF Generation, Async Reporting, Download URLs)
- [ ] **Phase 11: Dashboard, Notifications & Audit Trail**
- [ ] **Phase 12: Security Hardening, E2E Testing & Pilot Release**

> **Catatan**: command `SubmitPublishProof` (submit Live URL + bukti tayang) sudah diimplementasikan di modul Deliverable — bagian "publishing" dari Phase 9 sebagian sudah ada; yang belum adalah input manual metrics.

