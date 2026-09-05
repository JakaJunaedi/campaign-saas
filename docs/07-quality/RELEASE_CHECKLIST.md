# Release Checklist & Deployment Verification — Campaign SaaS

## 1. Document Control
- **Title**: Production Release & Verification Checklist
- **Purpose**: Prosedur standar verifikasi sebelum dan sesudah deployment ke lingkungan staging / production VPS.
- **Status**: ACCEPTED
- **Scope**: Release Verification

---

## 2. Pre-Release Checklist (Staging Gate)
- [ ] Seluruh unit tests dan integration tests lulus 100% di GitHub Actions.
- [ ] Database migration EF Core telah diuji rollback dan apply di environment staging.
- [ ] Bucket MinIO dan permission policy telah dikonfigurasi.
- [ ] Template jsreport telah diuji coba dengan data mock komprehensif.
- [ ] Koneksi Camunda REST API / BPMN diagram telah di-deploy dan terverifikasi.
- [ ] Environment variables produksi telah dikonfigurasi aman di VPS (tidak ada default passwords).

---

## 3. Post-Deployment Verification (Smoke Tests)
- [ ] Health Check API endpoint `/healthz` mengembalikan `200 OK` dengan status database, redis, rabbitmq, minio `Healthy`.
- [ ] Berhasil login dengan akun Agency Owner uji coba.
- [ ] Berhasil membuat 1 client, 1 campaign, 1 creator, dan 1 deliverable.
- [ ] Berhasil upload 1 file media uji coba ke MinIO via presigned URL.
- [ ] Berhasil generate 1 sample PDF report via jsreport.
- [ ] Audit logs mencatat aktivitas smoke test dengan benar.
