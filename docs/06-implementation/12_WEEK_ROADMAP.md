# 12-Week MVP Roadmap — Campaign SaaS

## 1. Document Control
- **Title**: 12-Week MVP Development Roadmap
- **Purpose**: Jadwal dan target rilis mingguan untuk mencapai milestone MVP pilot.
- **Status**: ACCEPTED
- **Scope**: 12-Week Timeline

---

## 2. Timeline Breakdown

| Minggu | Fokus Utama | Deliverables Kunci |
| :--- | :--- | :--- |
| **Week 1** | **Product & Architecture Baseline** | Finalisasi seluruh dokumentasi, ADR, dan arsitektur Modular Monolith |
| **Week 2** | **Project Scaffolding & DevOps** | .NET 10 Solution setup, Angular 21 scaffold, Docker Compose stack lokal |
| **Week 3** | **Identity & Multi-Tenancy** | Auth JWT, Multi-tenant global query filter, User & Role management |
| **Week 4** | **Client & Campaign Modules** | CRUD Client, CRUD Campaign, Campaign status state machine |
| **Week 5** | **Creator CRM Module** | Database profil creator, social accounts, search & filter |
| **Week 6** | **Campaign Creator Roster** | Penugasan creator ke campaign, alur status (Shortlisted -> Confirmed) |
| **Week 7** | **Deliverable Module** | Manajemen deliverable items, brief, deadlines |
| **Week 8** | **Content Upload & Approval** | MinIO presigned upload, Versioning (V1, V2), Approval & Review loop |
| **Week 9** | **Publishing & Metrics Input** | Live URL submission, manual metrics input, derived calculation |
| **Week 10** | **Reporting Engine (jsreport)** | Async worker RabbitMQ, template jsreport, PDF export download |
| **Week 11** | **Dashboard & Hardening** | In-app notification, audit trail, security audit, tenant isolation tests |
| **Week 12** | **Pilot Deployment & Onboarding** | Deploy ke Linux VPS, onboarding 3–5 agensi pilot partner |
