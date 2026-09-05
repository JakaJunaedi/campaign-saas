# Epics & User Stories Breakdown — Campaign SaaS

## 1. Document Control
- **Title**: Epics & User Stories Specification
- **Purpose**: Dekomposisi kebutuhan fungsional ke dalam Epics dan User Stories siap eksekusi.
- **Status**: ACCEPTED
- **Scope**: MVP Epics

---

## 2. Daftar Epics Utama

### EPIC-01: Organization & Identity Management
- **US-01.1**: Sebagai Agency Owner, saya dapat mendaftarkan organisasi baru dan login secara aman.
- **US-01.2**: Sebagai Agency Owner, saya dapat mengundang anggota tim dan menetapkan perannya (`CampaignManager`, `ContentReviewer`).

### EPIC-02: Client & Campaign Management
- **US-02.1**: Sebagai Campaign Manager, saya dapat membuat profil brand client baru.
- **US-02.2**: Sebagai Campaign Manager, saya dapat membuat campaign dengan menentukan budget, tanggal, dan brief.

### EPIC-03: Creator Management & Campaign Roster
- **US-03.1**: Sebagai Campaign Manager, saya dapat mendaftarkan profil creator ke database internal agensi.
- **US-03.2**: Sebagai Campaign Manager, saya dapat memasukkan creator ke dalam roster campaign dan menyepakati rate fee.

### EPIC-04: Deliverable & Content Approval Engine
- **US-04.1**: Sebagai Campaign Manager, saya dapat membuat item deliverable untuk creator terdaftar.
- **US-04.2**: Sebagai Creator, saya dapat mengunggah file draft video/foto dan caption sebelum deadline.
- **US-04.3**: Sebagai Content Reviewer, saya dapat mereview draft, meminta revisi dengan feedback, atau menyetujui draft.

### EPIC-05: Post-Publishing & Reporting Engine
- **US-05.1**: Sebagai Creator, saya dapat mengirimkan live link postingan media sosial sebagai bukti tayang.
- **US-05.2**: Sebagai Campaign Manager, saya dapat menginput metrik engagement dan mengekspor laporan PDF resmi kampanye.
