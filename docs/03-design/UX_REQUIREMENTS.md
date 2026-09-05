# UX Requirements & Frontend Guidelines — Campaign SaaS

## 1. Document Control
- **Title**: User Experience & Frontend Architecture Standards
- **Purpose**: Panduan UX/UI untuk implementasi frontend Angular 21 Standalone Components & Signals.
- **Status**: ACCEPTED
- **Scope**: Frontend UI/UX Standards

---

## 2. Prinsip Desain Utama
1. **Reaktivitas Berbasis Signals**: Tidak ada reload halaman; seluruh status pembaruan (notifikasi, approval status, progress upload) menggunakan Angular Signals (`signal()`, `computed()`, `effect()`).
2. **Optimistic UI & Feedback Cepat**: Tombol aksi memberikan feedback visual langsung (loading spinners, disabled state) saat mutasi API berlangsung.
3. **Pemisahan Pengalaman Agensi vs Creator**:
   - **Agency Portal**: Tampilan desktop-first yang padat informasi (*data-dense tables, kanban roster boards, detail side drawers*).
   - **Creator Portal**: Tampilan mobile-first yang bersih, fokus pada tugas unggah konten dan instruksi brief.

---

## 3. Komponen UX Kritis

### 3.1 Media Preview & Version Comparison
- Pemutar video tersemat (*embedded HTML5 video player*) yang mendukung format MP4, MOV, WebM.
- Komparator versi berdampingan (*side-by-side*) untuk membandingkan naskah/caption V1 vs V2 beserta catatan revisi.

### 3.2 File Upload Interactivity
- Drag-and-drop file uploader dengan indikator progress bar real-time (menggunakan XMLHttpRequest / Axios progress event ke MinIO pre-signed URL).
- Validasi tipe file dan ukuran file di sisi klien sebelum pengunggahan dimulai (Maksimum 500MB untuk video, 20MB untuk gambar).

### 3.3 Status Badges & Color Coding
- `DRAFT` / `PENDING`: Gray / Slate
- `PLANNED` / `SHORTLISTED`: Blue
- `ACTIVE` / `SUBMITTED`: Indigo / Violet
- `REVISION`: Amber / Orange
- `APPROVED` / `CONFIRMED` / `PUBLISHED`: Emerald / Green
- `REJECTED` / `CANCELLED`: Rose / Red
