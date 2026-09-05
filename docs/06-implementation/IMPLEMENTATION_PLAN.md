# Implementation Plan — Campaign SaaS

## 1. Document Control
- **Title**: Step-by-Step Technical Implementation Plan
- **Purpose**: Panduan urutan pengerjaan fitur berbasis dependensi modul untuk tim pengembang dan AI Coding Agents.
- **Status**: ACCEPTED
- **Scope**: Complete MVP Build Order

---

## 2. Urutan Pengerjaan Berbasis Dependensi (Phased Dependency Graph)

```text
Phase 0: Foundation Setup (Solution Structure, SharedKernel, EF Core, Docker Compose)
   ↓
Phase 1: Identity & Multi-Tenancy Module (Org, Users, JWT, RBAC, Tenant Filter)
   ↓
Phase 2: Client Module (Client CRUD, Contacts JSON)
   ↓
Phase 3: Campaign Module (Campaign Entity, Status Lifecycle, Dates & Budget)
   ↓
Phase 4: Creator Module (Creator CRM Database, Social Accounts)
   ↓
Phase 5: Campaign Creator Roster (Assignment, Status Transition)
   ↓
Phase 6: Deliverable Module (Deliverable Items, Due Dates, Platform Types)
   ↓
Phase 7: Content Submission & MinIO Storage (Presigned Upload, Versioning V1/V2)
   ↓
Phase 8: Approval & Review Module (Review Decision, Feedback Comments, Camunda Workflow)
   ↓
Phase 9: Publishing & Manual Metrics Module (Live URLs, Reach/Impressions/Engagement)
   ↓
Phase 10: Reporting Module (jsreport Async Worker via RabbitMQ, PDF Generation)
   ↓
Phase 11: Dashboard, Notifications & Audit Trail
   ↓
Phase 12: Security Hardening, E2E Testing & Pilot Release
```

---

## 3. Implementation Rules for AI Agents
1. Bangun modul secara berurutan sesuai dependency graph di atas. Jangan mengimplementasikan modul hilir (contoh: Reporting) sebelum modul hulu (Deliverable & Metrics) selesai dan memiliki unit tests.
2. Setiap kali modul baru dibuat, sertakan:
   - Domain Model & Invariants.
   - Application Commands & Queries (CQRS).
   - EF Core Configuration & Migration.
   - Minimal API Endpoints.
   - Unit & Integration Tests.
