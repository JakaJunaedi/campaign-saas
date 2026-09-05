# ADR-003: MinIO Object Storage with Direct Pre-Signed URL Client Uploads

## Status
ACCEPTED

## Context
Platform campaign influencer mengelola ratusan berkas media berukuran besar (video MP4/MOV hingga 500 MB per submission dan gambar foto resolusi tinggi). Mengunggah file besar melalui API backend ASP.NET Core akan menghabiskan bandwidth server, membebani memori API worker, dan menyebabkan request timeout.

## Decision
Kami memutuskan untuk menggunakan **MinIO Object Storage** dengan mekanisme **Direct Pre-Signed URL Upload**:
1. Klien (Angular) meminta upload authorization ke API Backend (`POST /api/v1/files/presigned-upload`).
2. API Backend memverifikasi kuota tenant, MIME whitelist, dan mengembalikan Pre-Signed PUT URL (expired dalam 15 menit) beserta `ObjectKey` unik.
3. Klien mengunggah file binary langsung ke MinIO via HTTP PUT dengan progress bar.
4. Klien memberi tahu Backend setelah upload selesai untuk mencatat metadata ke PostgreSQL.
5. MinIO bucket berstatus Private; pengunduhan media juga wajib menggunakan Pre-Signed GET URL yang diotorisasi server.

## Alternatives Considered
- **Upload Melalui ASP.NET Core Stream Multipart**: Ditolak karena menjadi bottleneck CPU dan network bandwidth pada instance API.
- **Menyimpan Binary di Database (BYTEA / BLOB)**: Ditolak keras karena merusak performa database PostgreSQL.

## Consequences
- **Positif**:
  - Server API .NET bebas dari beban transfer file media besar.
  - Upload lebih cepat dan dapat di-resume / dilacak progress-nya langsung dari browser.
  - Isolasi data tenant terjamin melalui folder path `/orgs/{org_id}/...`.
- **Negatif**:
  - Klien membutuhkan dua langkah request (Request Presigned URL -> Upload Binary -> Register Metadata).
