# REST API Contract & Specification — Campaign SaaS

## 1. Document Control
- **Title**: REST API Contract Specification
- **Purpose**: Menjadi kontrak resmi antarmuka HTTP RESTful antara backend ASP.NET Core 10 dan frontend Angular 21.
- **Status**: ACCEPTED
- **Scope**: Core MVP Endpoints

---

## 2. API Design Conventions
- **Base URL**: `/api/v1`
- **Authentication**: Bearer Token (`Authorization: Bearer <JWT>`) pada setiap request terproteksi.
- **Tenant Context**: Diresolusi otomatis dari JWT Claim `org_id` (atau header `X-Organization-Id` jika diotorisasi).
- **Format Data**: JSON (`Content-Type: application/json`).
- **Response Envelope**: Standar Result Wrapper:
  ```json
  {
    "success": true,
    "data": { ... },
    "error": null,
    "timestamp": "2026-09-05T16:00:00Z"
  }
  ```

---

## 3. Core API Endpoint Groups

### 3.0 SuperAdmin (Platform Management)
- `GET    /api/v1/admin/overview` — Global platform KPIs (Total Tenants, Active Campaigns, Storage Usage).
- `GET    /api/v1/admin/organizations` — Master list seluruh organisasi tenant (search, pagination, status filter).
- `POST   /api/v1/admin/organizations` — Membuat organisasi tenant baru secara manual.
- `PATCH  /api/v1/admin/organizations/{id}/status` — Mengubah status organisasi (`Active`, `Suspended`, `Trial`).
- `PUT    /api/v1/admin/organizations/{id}/quota` — Mengatur kuota batas storage dan active campaign tenant.
- `POST   /api/v1/admin/impersonate/{orgId}` — Generate temporary token untuk support troubleshooting (audit logged).
- `GET    /api/v1/admin/audit-logs` — Cross-tenant global security and access logs.

### 3.1 Identity & Organization
- `POST /api/v1/auth/login` — Login email & password, mengembalikan JWT & Refresh Token.
- `POST /api/v1/auth/refresh` — Refresh token untuk memperpanjang sesi JWT.
- `GET  /api/v1/organizations/current` — Informasi detail organisasi tenant aktif.
- `GET  /api/v1/organizations/users` — Daftar anggota tim organisasi (RBAC).

### 3.2 Client Management
- `GET  /api/v1/clients` — Daftar client brand dengan pagination & search.
- `POST /api/v1/clients` — Membuat client baru beserta kontak PIC.
- `GET  /api/v1/clients/{id}` — Detail client dan riwayat campaign.
- `PUT  /api/v1/clients/{id}` — Update informasi client.

### 3.3 Campaign Management
- `GET  /api/v1/campaigns` — Daftar campaign (filter by status, client, date).
- `POST /api/v1/campaigns` — Membuat campaign baru.
- `GET  /api/v1/campaigns/{id}` — Detail campaign, budget, dan status lifecycle.
- `PATCH /api/v1/campaigns/{id}/status` — Mengubah status campaign (`Planned`, `Active`, `Paused`, dll.).

### 3.4 Creator Management & Roster
- `GET  /api/v1/creators` — Database creator CRM internal.
- `POST /api/v1/creators` — Mendaftarkan creator baru ke database CRM.
- `GET  /api/v1/campaigns/{id}/roster` — Roster creator pada campaign tertentu.
- `POST /api/v1/campaigns/{id}/roster` — Menambahkan creator ke roster (`Shortlisted`).
- `PATCH /api/v1/campaigns/{id}/roster/{creatorId}/status` — Update status roster (`Invited`, `Accepted`, `Confirmed`).

### 3.5 Deliverable & Content Submission
- `GET  /api/v1/campaigns/{id}/deliverables` — Daftar deliverable campaign.
- `POST /api/v1/campaigns/{id}/deliverables` — Membuat item deliverable baru.
- `POST /api/v1/files/presigned-upload` — Request presigned PUT URL ke MinIO.
- `POST /api/v1/deliverables/{id}/submissions` — Submit draft konten (media key & caption).
- `GET  /api/v1/deliverables/{id}/submissions` — Riwayat versi submission (V1, V2, dst.).

### 3.6 Approval & Review
- `POST /api/v1/submissions/{id}/reviews` — Submit review keputusan (`Approved`, `RevisionRequested`, `Rejected`) + komentar feedback.
- `POST /api/v1/deliverables/{id}/publish-proof` — Submit live URL postingan dan screenshot bukti tayang.

### 3.7 Metrics & Reporting
- `PUT  /api/v1/deliverables/{id}/metrics` — Input metrik manual (Reach, Views, Likes, dll.).
- `POST /api/v1/campaigns/{id}/reports/generate` — Trigger async PDF report generation via jsreport & RabbitMQ.
- `GET  /api/v1/campaigns/{id}/reports/download` — Mengambil presigned download URL file PDF laporan akhir.
