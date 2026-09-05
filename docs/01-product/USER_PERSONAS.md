# User Personas — Campaign SaaS

## 1. Document Control
- **Title**: User Personas Specification
- **Purpose**: Mendefinisikan profil pengguna mendalam, tanggung jawab, kebutuhan, dan batasan hak akses.
- **Status**: ACCEPTED
- **Scope**: Core User Roles (Agency Owner, Campaign Manager, Content Reviewer, Creator)

---

## 2. Persona 0: SuperAdmin (Platform Root Administrator)
- **Nama Tipikal**: Alex (Head of Infrastructure & Platform Operations)
- **Tanggung Jawab**:
  - Manajemen seluruh tenant/organisasi di platform (pembuatan tenant, aktivasi, penangguhan (*suspend*), dan alokasi kuota).
  - Monitoring stabilitas sistem, antrean RabbitMQ, storage MinIO, dan kinerja worker global.
  - Audit investigasi kepatuhan dan keamanan lintas tenant (*cross-tenant audit trail*).
  - Melakukan impersonation / support access atas persetujuan tenant untuk troubleshooting teknis.
- **Kebutuhan Utama**:
  - Dashboard administrasi platform terpusat (*Platform Admin Console*).
  - Kemampuan melihat metrik penggunaan global (Total MRR, Active Orgs, Storage Consumption).
  - Bypass filter tenant secara terkontrol untuk pemeliharaan platform.
- **Hak Akses Sistem**: Full Root Platform Access (`Role: SuperAdmin`).

---

## 3. Persona 1: Agency Owner (Executive / Admin)
- **Nama Tipikal**: Ryan (Managing Director / Owner)
- **Tanggung Jawab**:
  - Pertumbuhan bisnis agensi, retensi klien, dan profitabilitas campaign.
  - Mengelola user akun agensi, alokasi peran (RBAC), dan konfigurasi organisasi.
  - Memantau dashboard performa bisnis dan utilisasi tim.
- **Kebutuhan Utama**:
  - Visibilitas menyeluruh atas semua campaign aktif.
  - Laporan komprehensif untuk klien brand besar.
  - Keamanan dan isolasi data agensi yang terjamin.
- **Hak Akses Sistem**: Full Organization Control (`Role: AgencyOwner`).

---

## 4. Persona 2: Campaign Manager (Operational Lead)
- **Nama Tipikal**: Sarah (Senior Campaign Executive)
- **Tanggung Jawab**:
  - Membuat dan mengelola timeline campaign dari awal hingga selesai.
  - Melakukan shortlisting creator, mengirim undangan, dan menyepakati deliverable.
  - Menginput metrik performa manual dan mengekspor laporan akhir.
- **Kebutuhan Utama**:
  - Kemudahan mengelola roster creator dan status deliverable dalam format Kanban / List.
  - Notifikasi otomatis saat creator mengunggah materi konten.
- **Hak Akses Sistem**: Campaign, Creator, Deliverable, Reporting (`Role: CampaignManager`).

---

## 5. Persona 3: Content Reviewer (Quality & Brand Compliance)
- **Nama Tipikal**: Dimas (Creative Lead / Quality Assurance)
- **Tanggung Jawab**:
  - Memeriksa kesesuaian materi video/foto/caption dengan brief brand.
  - Memberikan feedback revisi spesifik dan menyetujui draft final.
- **Kebutuhan Utama**:
  - Media player/previewer yang cepat untuk file video resolusi tinggi.
  - Fitur perbandingan versi (V1 vs V2) dan riwayat komentar.
- **Hak Akses Sistem**: Deliverable Review, Approval, Revision Feedback (`Role: ContentReviewer`).

---

## 6. Persona 4: Creator / KOL (External Contributor)
- **Nama Tipikal**: Amanda (Beauty & Lifestyle Content Creator)
- **Tanggung Jawab**:
  - Melihat brief deliverable dan tenggat waktu (deadline).
  - Mengunggah draft video/foto dan caption sebelum tanggal tayang.
  - Mengunggah link posting dan bukti tayang setelah disetujui.
- **Kebutuhan Utama**:
  - Tampilan portal web mobile-friendly yang sederhana dan jelas.
  - Informasi status approval instan tanpa harus chat manual bolak-balik.
- **Hak Akses Sistem**: Restricted Portal: View Assigned Deliverables & Submit Content (`Role: Creator`).
