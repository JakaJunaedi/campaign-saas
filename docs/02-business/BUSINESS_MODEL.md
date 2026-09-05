# Business Model & Commercial Strategy — Campaign SaaS

## 1. Document Control
- **Title**: Business Model & Commercial Strategy
- **Purpose**: Mendefinisikan model bisnis B2B SaaS, value metric, dan struktur lisensi.
- **Status**: ACCEPTED (Core Concept) / PROPOSED (Tier Details)
- **Scope**: Commercial & Revenue Architecture

---

## 2. Revenue Model: B2B Multi-Tenant Subscription
Campaign SaaS mengadopsi model langganan bulanan / tahunan (*Monthly/Annual Recurring Revenue - MRR/ARR*).
- **Tenant Entity**: Setiap agensi berlangganan sebagai 1 `Organization`.
- **Value Metric**:
  1. Jumlah active campaign simultan per bulan.
  2. Jumlah seat internal agensi (Campaign Managers & Reviewers).
  3. Kuota penyimpanan media (MinIO storage capacity).

*Catatan*: Akun Creator/KOL tidak dikenakan biaya lisensi (Free Guest Contributor) untuk mempercepat adopsi dan kemudahan onboarding.

---

## 3. Unit Economics & Cost Drivers
- **Infrastruktur VPS & Storage**: Fixed cost server VPS + variable cost MinIO storage per GB.
- **jsreport Document Generation**: CPU usage worker per laporan.
- **Gross Margin Target**: > 80% pada kapasitas normal.

---

## 4. Decisions & Open Questions
- **ACCEPTED DECISION**: Penagihan subscription otomatis berada di luar cakupan MVP (dikelola secara manual / invoice direct selama pilot 3-5 agensi).
- **OPEN QUESTION**: Apakah kuota storage akan di-hard-cap atau diizinkan overage billing pada fase komersial? (`PROPOSED DECISION`: Soft-cap dengan peringatan email saat mencapai 90% kapasitas).
