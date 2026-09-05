# MVP Scope & Boundaries — Campaign SaaS

## 1. Document Control
- **Title**: Minimum Viable Product (MVP) Scope
- **Purpose**: Menetapkan batasan tegas antara fitur yang WAJIB diimplementasikan (In-Scope) dan DILARANG diimplementasikan (Out-of-Scope) pada MVP.
- **Status**: ACCEPTED
- **Scope**: MVP Release Boundaries

---

## 2. Definisi Sukses MVP
MVP dianggap berhasil apabila 3–5 agensi eksternal dapat menyelesaikan siklus hidup campaign penuh:
```text
Login
  ↓
Create Client
  ↓
Create Campaign
  ↓
Add Creator
  ↓
Assign Creator to Campaign
  ↓
Create Deliverable
  ↓
Creator Submits Content
  ↓
Reviewer Reviews & Requests Revision (if needed)
  ↓
Creator Resubmits
  ↓
Content Approved
  ↓
Creator Posts & Submits Live URL
  ↓
Campaign Manager Inputs Metrics
  ↓
Generate PDF Report
```
Seluruh alur di atas berjalan mulus tanpa intervensi manual developer di database/server.

---

## 3. Matriks Ruang Lingkup MVP

| Modul | In-Scope (MVP) | Explicit Out-of-Scope (Post-MVP) |
| :--- | :--- | :--- |
| **Identity & Access** | Email/Password Auth, JWT, Org Multi-tenancy, RBAC 4 Roles | Social Login, Enterprise SSO (SAML/Okta), 2FA SMS |
| **Client** | CRUD Client, Multi-PIC Contact Information | CRM Pipeline Deals, Contract Signing e-Signature |
| **Campaign** | Campaign Lifecycle (6 Status), Budget, Dates, Brief | Automated Campaign Generator, AI Campaign Planner |
| **Creator** | Creator Profile, Social Account Info (manual), Niche, Status | Public Marketplace, Automated Scraping, Discovery Engine |
| **Campaign Creator** | Roster Assignment, 7 Workflow Statuses, Agreed Rate | Dynamic Negotiation Chat Room, Automated Contract Gen |
| **Deliverable** | Deliverable Types (IG, TikTok, YT), Deadlines, 6 Statuses | Direct Social Auto-Publishing API |
| **Content** | Media Upload (MinIO), Versioning (V1, V2), Submission Log | In-browser Video Trimming / Video Editing Canvas |
| **Approval** | Approve, Reject, Request Revision, Comment Threads | Client External Approval Link (Shared Public Reviewer) |
| **Reporting** | Manual Metrics (Reach, Impr, Likes, dll.), Derived KPIs, PDF | Automated Meta/TikTok API Sync, Real-time Graph Sync |
| **Notification** | In-app notification system | WhatsApp Bot Notification, SMS Gateway |
| **Infrastructure** | Linux VPS, Docker Compose, PostgreSQL, Redis, RabbitMQ, Camunda, MinIO, jsreport | Kubernetes Cluster, Multi-region Active-Active |

---

## 4. Kebijakan Anti Scope-Creep
1. AI Coding Agent dan Developer **DILARANG** menambahkan endpoint API, tabel database, atau tampilan UI di luar daftar In-Scope di atas tanpa Architectural Decision Record (ADR) yang disetujui.
2. Fitur sekunder seperti integrasi payment gateway dan automated scraping dicatat di backlog pasca-MVP.
