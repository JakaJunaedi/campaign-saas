# User Acceptance Criteria (Gherkin Scenarios) — Campaign SaaS

## 1. Document Control
- **Title**: User Acceptance Criteria (UAC) Specification
- **Purpose**: Kriteria penerimaan fungsional berbasis format Gherkin Given-When-Then.
- **Status**: ACCEPTED
- **Scope**: Core Workflow Scenarios

---

## 2. Core Workflow Scenarios

### Skenario 1: Creator Mengunggah Draft Konten
```gherkin
Scenario: Creator mengunggah draft video untuk deliverable yang ditugaskan
  Given Creator telah login ke Creator Portal
  And Creator memiliki deliverable dengan status "PENDING"
  When Creator memilih file video "draft_v1.mp4" dan memasukkan teks caption
  And Creator menekan tombol "Submit Draft"
  Then File berhasil diunggah ke MinIO Object Storage
  And Record Submission baru dibuat dengan Version Number 1
  And Status deliverable berubah menjadi "SUBMITTED"
  And Campaign Manager menerima notifikasi in-app
```

### Skenario 2: Content Reviewer Meminta Revisi Konten
```gherkin
Scenario: Reviewer meminta revisi dengan catatan feedback
  Given Content Reviewer membuka halaman review deliverable yang berstatus "SUBMITTED"
  When Reviewer memilih keputusan "REVISION_REQUESTED"
  And Reviewer memasukkan catatan feedback "Tolong ganti lagu latar dan perjelas logo brand di detik ke-5"
  And Reviewer menekan tombol "Submit Review"
  Then Status deliverable berubah menjadi "REVISION"
  And Log audit mencatat keputusan reviewer dan catatan feedback
  And Creator menerima notifikasi instruksi revisi
```

### Skenario 3: Isolasi Data Lintas Tenant Ditolak
```gherkin
Scenario: User Org A mencoba mengakses campaign milik Org B
  Given User terautentikasi dengan token Organization "Org-Alpha"
  When User mengirimkan request "GET /api/v1/campaigns/{id-milik-Org-Beta}"
  Then Sistem menolak akses dan mengembalikan HTTP 404 Not Found
  And Tidak ada data dari Org-Beta yang bocor di respons
```
