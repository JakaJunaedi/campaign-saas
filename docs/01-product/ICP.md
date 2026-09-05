# Ideal Customer Profile (ICP) — Campaign SaaS

## 1. Document Control
- **Title**: Ideal Customer Profile (ICP)
- **Purpose**: Mendefinisikan profil pelanggan ideal untuk agensi sasaran Campaign SaaS.
- **Status**: ACCEPTED
- **Scope**: Market Definition & Customer Segmentation

---

## 2. Target Market Segments

### Segment Utama: Boutique & Mid-Scale Influencer / KOL Agencies
- **Ukuran Tim**: 5 - 50 karyawan (Campaign Managers, Account Executives, Content Reviewers).
- **Volume Campaign**: 10 - 80 campaign aktif per bulan.
- **Creator Network**: Mengelola database 100 - 5.000 kontak KOL/Creator.
- **Karakteristik Operasional**: Mengalami *operational bottleneck* akibat koordinasi ribuan materi konten, revisi, dan reporting melalui Google Drive & WhatsApp.

---

## 3. Pain Points & Solution Mapping

| Pain Point Agensi | Dampak Bisnis | Solusi Campaign SaaS |
| :--- | :--- | :--- |
| **Penyimpanan Draft Konten Tercecer** | Salah posting versi video/revisi lama, komplain dari brand | Centralized Content Submission & Versioning dengan MinIO Storage |
| **Bottleneck Approval & Revisi** | Campaign molor dari timeline peluncuran brand | Dedicated Review & Revision Loop dengan Camunda Workflow & Comments |
| **Penyusunan Report Manual** | Butuh 2-3 hari untuk membuat deck laporan performa | One-Click PDF Report Generation via jsreport & Manual Metrics Input |
| **Data Creator Tercecer di Spreadsheet** | Kehilangan kontak, duplikasi rate card, tidak ada riwayat performa | Centralized Creator CRM & Historical Campaign Roster |

---

## 4. Disqualified Customers (Non-Target untuk MVP)
- Bisnis direct-to-consumer (D2C) yang hanya mencari 1-2 influencer sekali pakai tanpa agensi.
- Agensi yang mewajibkan automated scraping tanpa persetujuan API resmi platform sosial media.
- Perusahaan yang membutuhkan sistem akuntansi perbankan terintegrasi penuh pada hari pertama.
