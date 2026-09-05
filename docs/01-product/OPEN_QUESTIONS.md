# Centralized Open Questions & Decision Log — Campaign SaaS

## 1. Document Control
- **Title**: Open Questions & Architectural Decisions Log
- **Purpose**: Melacak seluruh pertanyaan terbuka, status usulan keputusan, dan kesepakatan dengan stakeholders produk.
- **Status**: ACTIVE TRACKER
- **Scope**: Product & Architecture Clarifications

---

## 2. Decision Status Taxonomy
- `ACCEPTED`: Keputusan telah disetujui resmi dan wajib dipatuhi.
- `PROPOSED`: Solusi teknis/bisnis telah diusulkan namun menunggu konfirmasi final.
- `OPEN QUESTION`: Kebutuhan belum terdefinisi secara jelas, dilarang membuat asumsi bebas.
- `DEFERRED`: Keputusan ditunda hingga fase pasca-MVP.

---

## 3. Log Pertanyaan & Keputusan

| ID | Kategori | Ringkasan Pertanyaan / Keputusan | Status Saat Ini | Rencana Aksi / Rekomendasi |
| :--- | :--- | :--- | :--- | :--- |
| **Q-01** | **Reporting** | Apakah perlu menyediakan ekspor laporan dalam format Excel (.xlsx / .csv) selain PDF resmi? | `PROPOSED` | Menyediakan endpoint ekspor CSV mentah untuk tabel metrik deliverable pada Sprint 10. |
| **Q-02** | **Storage** | Berapa lama batas retensi penyimpanan draft video creator di MinIO setelah status campaign `COMPLETED`? | `OPEN QUESTION` | Diusulkan (`PROPOSED`): 90 hari setelah campaign selesai, sebelum file dipindahkan ke cold archive atau dihapus. |
| **Q-03** | **Commercial** | Bagaimana kebijakan overage jika agensi melampaui batas kuota storage paket mereka? | `PROPOSED` | Soft-cap dengan email notifikasi peringatan saat mencapai 90% dan 100%, tanpa memblokir akses baca. |
| **Q-04** | **Creator Access** | Apakah Creator membutuhkan password akun penuh atau cukup magic link / invite link berbatas waktu? | `PROPOSED` | Magic link berbatas waktu untuk MVP onboarding, dengan opsi setup password bagi creator yang ingin login berulang. |
| **Q-05** | **Camunda** | Menggunakan Camunda 7 Community Edition Run atau Camunda 8 (Zeebe)? | `ACCEPTED` | Camunda 7 Platform Run (`camunda/camunda-bpm-platform:run-latest`) via REST API External Worker untuk kesederhanaan deployment single-node VPS pada MVP. |
