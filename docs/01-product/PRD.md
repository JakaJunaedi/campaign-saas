# Product Requirements Document (PRD) — Campaign Management SaaS

## 1. Document Control
- **Title**: Product Requirements Document (PRD)
- **Purpose**: Menjadi Single Source of Truth untuk kebutuhan fungsional, non-fungsional, dan batasan produk B2B SaaS Campaign Management Platform untuk Influencer & KOL Agency.
- **Status**: ACCEPTED (Core MVP Baseline)
- **Scope**: MVP Scope (In-Scope vs Explicit Out-of-Scope)
- **Owner**: Product & Architecture Team

---

## 2. Product Context & Overview
Platform ini dirancang khusus untuk **Influencer Agency, KOL Agency, dan Campaign Management Agency** skala kecil hingga menengah. Masalah utama yang diselesaikan adalah inefisiensi koordinasi campaign manual (via WhatsApp, Google Sheets, dan Google Drive) yang sering menyebabkan kesalahan versi konten, revisi tercecer, keterlambatan posting, dan lamanya waktu pembuatan laporan.

### 2.1 Core Workflow
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
Content Submission
  ↓
Review
  ↓
Revision
  ↓
Approval
  ↓
Publishing
  ↓
Reporting
```

---

## 3. Primary Personas
1. **Agency Owner**: Pemilik agensi yang membutuhkan visibilitas bisnis menyeluruh, manajemen user/tim, pengaturan organisasi/tenant, serta ringkasan campaign dan reporting.
2. **Campaign Manager**: Penanggung jawab operasional campaign, seleksi & penugasan creator, pembuatan deliverable, pemantauan timeline, dan eksekusi workflow.
3. **Content Reviewer**: Tim internal agensi yang bertugas mereview konten kiriman creator, memberikan feedback revisi, dan memberikan approval akhir.
4. **Creator (KOL / Influencer)**: Pengguna eksternal yang menerima penugasan campaign, melihat rincian deliverable, mengunggah draft konten, menerima catatan revisi, dan melihat status approval.

---

## 4. Functional Requirements (MVP In-Scope)

### 4.1 Platform, Identity & Multi-Tenancy
- **REQ-PLT-01**: Multi-tenancy dengan isolasi data ketat berbasis `OrganizationId` (Tenant boundary).
- **REQ-PLT-02**: Otentikasi berbasis JWT dengan Refresh Token.
- **REQ-PLT-03**: Role-Based Access Control (RBAC) dengan 4 peran: `AgencyOwner`, `CampaignManager`, `ContentReviewer`, dan `Creator`.

### 4.2 Client Management
- **REQ-CLI-01**: CRUD Client (Brand pengiklan) dalam organisasi agensi.
- **REQ-CLI-02**: Menyimpan informasi kontak PIC klien (Nama, Email, Nomor Telepon, Jabatan).

### 4.3 Campaign Management
- **REQ-CMP-01**: Pembuatan dan pengelolaan campaign terikat ke Client tertentu.
- **REQ-CMP-02**: Atribut campaign: Judul, Deskripsi, Budget, Tanggal Mulai, Tanggal Selesai, dan Status.
- **REQ-CMP-03**: Status campaign: `DRAFT`, `PLANNED`, `ACTIVE`, `PAUSED`, `COMPLETED`, `CANCELLED`.

### 4.4 Creator Management (CRM)
- **REQ-CTR-01**: Database profil creator internal agensi.
- **REQ-CTR-02**: Profil creator: Nama Lengkap, Email, Nomor Kontak (WhatsApp), Kategori/Niche, Status, serta Akun Media Sosial (Platform, Handle, Follower Count manual).

### 4.5 Campaign Creator (Assignment & Roster)
- **REQ-CCR-01**: Pemilihan creator ke dalam roster campaign.
- **REQ-CCR-02**: Status alur penugasan creator:
  `SHORTLISTED` -> `INVITED` -> `ACCEPTED` / `REJECTED` -> `NEGOTIATION` -> `CONFIRMED` -> `COMPLETED`.
- **REQ-CCR-03**: Pencatatan agreed rate / fee creator per campaign.

### 4.6 Deliverable Management
- **REQ-DLV-01**: Deliverable terikat pada Creator yang telah `CONFIRMED`.
- **REQ-DLV-02**: Atribut deliverable: Judul, Channel/Platform (Instagram, TikTok, YouTube), Tipe Konten (Reel, Story, Video), Brief, Deadline Draft, Deadline Posting.
- **REQ-DLV-03**: Status deliverable:
  `PENDING` -> `SUBMITTED` -> `REVISION` -> `APPROVED` -> `PUBLISHED` -> `REJECTED`.

### 4.7 Content Submission & Versioning
- **REQ-CNT-01**: Creator mengunggah draft media (video/gambar/dokumen naskah) dan caption.
- **REQ-CNT-02**: Media disimpan di MinIO Object Storage; metadata disimpan di database.
- **REQ-CNT-03**: Dukungan versioning otomatis (V1, V2, dst.) pada setiap submission ulang.

### 4.8 Review, Revision & Approval
- **REQ-APP-01**: Reviewer melihat preview media dan riwayat versi submission.
- **REQ-APP-02**: Reviewer dapat menyetujui (`APPROVED`), menolak (`REJECTED`), atau meminta revisi (`REVISION`) disertai komentar spesifik.
- **REQ-APP-03**: Riwayat approval dan komentar tersimpan lengkap.

### 4.9 Publishing & Proof Submission
- **REQ-PUB-01**: Creator mengunggah Live URL / Link Posting dan screenshot bukti posting setelah deliverable `APPROVED`.
- **REQ-PUB-02**: Status deliverable otomatis beralih ke `PUBLISHED`.

### 4.10 Reporting & Manual Metrics Input
- **REQ-REP-01**: Input metrik performa manual per deliverable: `Reach`, `Impressions`, `Views`, `Likes`, `Comments`, `Shares`, `Clicks`, `Engagement`.
- **REQ-REP-02**: Perhitungan Derived Metrics otomatis:
  - Engagement Rate (%) = (Total Engagement / Total Impressions) * 100
  - Cost per Engagement (CPE) = Creator Cost / Total Engagement
  - Cost per View (CPV) = Creator Cost / Total Views
- **REQ-REP-03**: Generate dokumen laporan PDF asinkron via jsreport & RabbitMQ.

### 4.11 Dashboard, Notifications & Audit Trail
- **REQ-DSH-01**: Dashboard metrik campaign aktif, status deliverable, dan ringkasan budget.
- **REQ-NTF-01**: In-app notifications untuk event penting workflow.
- **REQ-AUD-01**: Audit trail untuk perubahan entitas dan status bisnis kritis.

---

## 5. Explicit Out of Scope (MVP)
Fitur berikut **DILARANG** diimplementasikan pada fase MVP:
1. Influencer / Creator Public Discovery Marketplace.
2. Automated Social Media API Integration (Meta Graph API, TikTok API, YouTube Data API).
3. Automated Social Media Analytics Scraping.
4. Payment Gateway & Automated Creator Payout.
5. Full Accounting / Invoicing Ledger System.
6. AI Creator Matching, AI Campaign / Content Generator.
7. Mobile Application (iOS/Android Native).
8. White-label Custom Domain Platform.
9. Enterprise SSO (SAML/Okta) & Subscription Billing System.
10. Kubernetes deployment.

---

## 6. Non-Functional Requirements (NFR)
- **NFR-SEC-01**: Isolasi data multi-tenant server-side dijamin pada level database query filter dan token authorization.
- **NFR-PERF-01**: Waktu respons API p95 < 250ms untuk operasi transaksional standar.
- **NFR-RELI-01**: Pengunggahan file besar menggunakan pre-signed URL langsung ke MinIO untuk mencegah bottleneck API server.
- **NFR-ARCH-01**: Backend dibangun dengan Modular Monolith (.NET 10) yang microservices-ready.

---

## 7. Decisions, Assumptions & Open Questions
- **ACCEPTED DECISION**: Arsitektur backend adalah Modular Monolith (.NET 10, C# 14) dengan PostgreSQL sebagai single source of truth.
- **ACCEPTED DECISION**: Camunda BPMN digunakan untuk workflow orchestration alur Campaign & Deliverable Approval.
- **ACCEPTED DECISION**: jsreport + RabbitMQ digunakan untuk PDF report generation asinkron.
- **ASSUMPTION**: Creator mengakses portal web responsif (Angular 21 Standalone) tanpa native app.
- **OPEN QUESTION**: Apakah format ekspor laporan selain PDF (misal: Excel/XLSX) diperlukan pada MVP? (PROPOSED: Ya, ekspor CSV/Excel sederhana disediakan).

---

## 8. Cross References
- [MVP_SCOPE.md](file:///d:/Jack/dotnet/campaign-saas/docs/01-product/MVP_SCOPE.md)
- [USER_PERSONAS.md](file:///d:/Jack/dotnet/campaign-saas/docs/01-product/USER_PERSONAS.md)
- [ARCHITECTURE.md](file:///d:/Jack/dotnet/campaign-saas/docs/04-architecture/ARCHITECTURE.md)
