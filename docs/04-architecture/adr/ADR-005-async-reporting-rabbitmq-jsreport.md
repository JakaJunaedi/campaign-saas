# ADR-005: Asynchronous PDF Report Generation via RabbitMQ and jsreport

## Status
ACCEPTED

## Context
Pembuatan laporan akhir campaign melibatkan agregasi data metrik, kompilasi thumbnail foto/video, penghitungan engagement rate/CPE, dan rendering layout PDF multi-halaman. Proses rendering PDF via headless Chromium / jsreport memakan waktu antara 3 hingga 15 detik dan membutuhkan konsumsi CPU yang tinggi. Menjalankan proses ini secara synchronous pada HTTP request akan menyebabkan HTTP timeout dan memblokir thread pool Web API.

## Decision
Kami memutuskan untuk menggunakan pola **Asynchronous Worker Pipeline**:
1. User meminta pembuatan report melalui `POST /api/v1/campaigns/{id}/reports/generate`.
2. API langsung mencatat record permintaan laporan dengan status `PENDING` dan mem-publish integration event `GenerateCampaignReportCommand` ke RabbitMQ queue `reporting-jobs`.
3. API segera mengembalikan respons HTTP `202 Accepted` dengan jobId.
4. Background Worker mengonsumsi event, mengompilasi data, memanggil service jsreport untuk merender PDF, menyimpan file PDF ke MinIO bucket `reports`, dan mengupdate status laporan menjadi `COMPLETED`.
5. User menerima in-app notification saat laporan selesai dan dapat mengunduh via Presigned URL.

## Alternatives Considered
- **Synchronous In-Memory PDF Generation (QuestPDF / iText7 langsung di API)**: Ditolak karena membebani memori API dan rentan timeout saat report berukuran besar.

## Consequences
- **Positif**:
  - API tetap responsif tanpa risiko timeout.
  - Beban komputasi jsreport terisolasi pada worker thread / container terpisah.
  - Retry otomatis via RabbitMQ jika terjadi kegagalan rendering sementara.
- **Negatif**:
  - Membutuhkan polling frontend atau WebSocket/SignalR/In-App Notification untuk memberitahu user saat report selesai.
