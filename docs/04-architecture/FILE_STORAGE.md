# File Storage Architecture (MinIO) — Campaign SaaS

## 1. Document Control
- **Title**: Object Storage & Asset Management
- **Purpose**: Arsitektur penyimpanan file binary media, dokumen brief, dan laporan PDF menggunakan MinIO.
- **Status**: ACCEPTED
- **Scope**: Object Storage Architecture

---

## 2. Logical Bucket Layout
MinIO dikonfigurasi dengan 5 bucket logis terisolasi:

```plaintext
minio-storage/
├── campaign-assets/       # Brief attachments, guideline PDF dari brand
├── creator-content/       # Video draft (.mp4, .mov), photo draft (.jpg, .png)
├── reports/               # Output PDF laporan yang dihasilkan jsreport
├── documents/             # Dokumen pendukung agensi
└── exports/               # File ekspor data CSV/Excel sementara
```

---

## 3. Object Key Naming Convention
Untuk menjamin isolasi multi-tenant dan mencegah penumpukan file:
```text
{bucket-name}/orgs/{organization_id}/campaigns/{campaign_id}/{deliverable_id}/v{version}/{file_uuid}_{filename}
```

---

## 4. Alur Presigned Upload & Download

```mermaid
sequenceDiagram
    autonumber
    actor User as Angular SPA
    participant API as .NET 10 API
    participant MinIO as MinIO Object Storage

    User->>API: 1. POST /api/v1/files/presigned-upload (file_name, size, content_type)
    API->>API: 2. Validasi Kuota Org & File Whitelist
    API->>MinIO: 3. Generate Presigned PUT URL (Expiry: 15 min)
    MinIO-->>API: 4. Presigned Upload URL
    API-->>User: 5. Return Presigned URL & ObjectKey

    User->>MinIO: 6. HTTP PUT Binary File langsung ke MinIO
    MinIO-->>User: 7. 200 OK Upload Success

    User->>API: 8. POST /api/v1/deliverables/{id}/submissions (ObjectKey, Caption)
    API->>API: 9. Simpan Metadata Submission ke PostgreSQL
```
