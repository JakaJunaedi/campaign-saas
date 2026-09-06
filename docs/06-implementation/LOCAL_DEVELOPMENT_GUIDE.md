# Local Development & Docker Setup Guide — Campaign SaaS

## 1. Document Control
- **Title**: Local Development & Environment Setup Guide
- **Purpose**: Panduan teknis langkah-demi-langkah bagi Software Engineers dan AI Coding Agents untuk mengonfigurasi dan menjalankan environment development lokal.
- **Status**: ACCEPTED
- **Scope**: Local Development Environment

---

## 2. Prasyarat Sistem (Prerequisites)
Pastikan perangkat kerja telah terpasang:
- [.NET 10 SDK](https://dotnet.microsoft.com/download) (`dotnet --version` -> `10.0.x`)
- [Node.js](https://nodejs.org/) v20+ / v22 LTS & npm
- [Angular CLI](https://angular.dev/tools/cli) v21 (`npm install -g @angular/cli`)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (dengan Docker Compose v2)
- Git

---

## 3. Docker Compose Local Infrastructure Stack

Buat file `docker-compose.dev.yml` di root repositori untuk menjalankan seluruh service pendukung:

```yaml
version: '3.8'

services:
  postgres:
    image: postgres:16-alpine
    container_name: campaign_saas_postgres
    environment:
      POSTGRES_USER: saas_admin
      POSTGRES_PASSWORD: DevPassword123!
      POSTGRES_DB: campaign_saas_dev
    ports:
      - "5432:5432"
    volumes:
      - pgdata_dev:/var/lib/postgresql/data

  redis:
    image: redis:7-alpine
    container_name: campaign_saas_redis
    ports:
      - "6379:6379"

  rabbitmq:
    image: rabbitmq:3-management-alpine
    container_name: campaign_saas_rabbitmq
    environment:
      RABBITMQ_DEFAULT_USER: rabbit_user
      RABBITMQ_DEFAULT_PASS: RabbitDev123!
    ports:
      - "5672:5672"
      - "15672:15672"

  minio:
    image: minio/minio:latest
    container_name: campaign_saas_minio
    command: server /data --console-address ":9001"
    environment:
      MINIO_ROOT_USER: minio_admin
      MINIO_ROOT_PASSWORD: MinioDevPassword123!
    ports:
      - "9000:9000"
      - "9001:9001"
    volumes:
      - miniodata_dev:/data

  camunda:
    image: camunda/camunda-bpm-platform:run-latest
    container_name: campaign_saas_camunda
    environment:
      WAIT_FOR: postgres:5432
    ports:
      - "8088:8080"

  jsreport:
    image: jsreport/jsreport:latest
    container_name: campaign_saas_jsreport
    environment:
      trustUserCode: "true"
    ports:
      - "5488:5488"

volumes:
  pgdata_dev:
  miniodata_dev:
```

---

## 4. Langkah-Langkah Menjalankan Proyek Secara Lokal

### Langkah 1: Jalankan Infrastruktur Dependencies
```bash
docker compose -f docker-compose.dev.yml up -d
```
Verifikasi seluruh container berjalan aktif dengan `docker ps`.

### Langkah 2: Konfigurasi Appsettings Backend
File `src/server/Presentation/Api/appsettings.Development.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=campaign_saas_dev;Username=saas_admin;Password=DevPassword123!"
  },
  "Redis": {
    "Configuration": "localhost:6379"
  },
  "RabbitMQ": {
    "Host": "localhost",
    "Username": "rabbit_user",
    "Password": "RabbitDev123!"
  },
  "MinIO": {
    "Endpoint": "localhost:9000",
    "AccessKey": "minio_admin",
    "SecretKey": "MinioDevPassword123!",
    "UseSSL": false
  },
  "Camunda": {
    "RestUri": "http://localhost:8088/engine-rest"
  },
  "JsReport": {
    "Uri": "http://localhost:5488"
  }
}
```

### Langkah 3: Terapkan Database Migration
```bash
dotnet ef database update --project src/server/Infrastructure --startup-project src/server/Presentation/Api
```

### Langkah 4: Jalankan Backend .NET 10 Web API
```bash
dotnet run --project src/server/Presentation/Api
# Swagger UI tersedia di http://localhost:5000/swagger
```

### Langkah 5: Jalankan Frontend Angular 21 Standalone
```bash
cd src/client
npm install
npm start
# Buka browser di http://localhost:4200 (otomatis diarahkan ke backend API via proxy.conf.json)
```

---

## 5. Akun Uji Coba Default (Seeded Dev Accounts)

Untuk mempermudah pengujian alur kerja tanpa perlu registrasi manual berulang:

| Peran | Email | Password | Organization ID (Tenant) |
| :--- | :--- | :--- | :--- |
| **SuperAdmin (Root)** | `superadmin@campaignsaas.local` | `SuperAdminDev123!` | `22222222-2222-2222-2222-222222222222` (System Scope) |
| **Agency Owner** | `owner@agency-alpha.local` | `OwnerDev123!` | `11111111-1111-1111-1111-111111111111` |
| **Campaign Manager** | `cm@agency-alpha.local` | `CmDev123!` | `11111111-1111-1111-1111-111111111111` |
| **Content Reviewer** | `reviewer@agency-alpha.local` | `ReviewerDev123!` | `11111111-1111-1111-1111-111111111111` |
| **Creator (KOL)** | `creator@beauty.local` | `CreatorDev123!` | `11111111-1111-1111-1111-111111111111` |

---

## 6. Inisialisasi Otomatis MinIO Buckets
Aplikasi .NET API menyertakan background initialization service (`MinioBucketInitializer`) saat startup di environment `Development` yang secara otomatis membuat 5 bucket jika belum tersedia:
- `campaign-assets`
- `creator-content`
- `reports`
- `documents`
- `exports`

---

---

## 7. Konfigurasi Angular Proxy Dinamis (`proxy.conf.mjs`)
Untuk menghindari kendala CORS dan mendukung endpoint backend yang dinamis (misal backend berjalan di IP WSL, container, atau remote):
```javascript
// proxy.conf.mjs
const target = process.env.API_TARGET || 'http://localhost:5000';
console.log(`[Angular Proxy] Routing /api requests to: ${target}`);

export default {
  '/api': {
    target: target,
    secure: false,
    changeOrigin: true,
    logLevel: 'debug'
  }
};
```

---

## 8. Troubleshooting & Solusi Konflik Port Lokal

| Masalah / Error | Penyebab Umum | Solusi Cepat |
| :--- | :--- | :--- |
| `Bind for 0.0.0.0:5432 failed: port is already allocated` | Service PostgreSQL lokal di OS host sedang aktif | Matikan service PostgreSQL lokal atau ubah port host di `docker-compose.dev.yml` menjadi `"5433:5432"`. |
| `Camunda engine connection refused (port 8080)` | Container Camunda masih dalam proses boot JVM | Tunggu 15-30 detik hingga log Camunda menunjukkan `Camunda BPM platform started`. |
| `MinIO presigned URL download failed (SSL error)` | Mengakses endpoint HTTPS pada MinIO lokal | Pastikan `UseSSL: false` pada konfigurasi appsettings lokal. |
| `CORS error on Angular HTTP calls` | Memanggil langsung port 5000 tanpa proxy | Gunakan path relatif `/api/v1/...` di Angular service agar melalui proxy. |

---

## 9. Dynamic Environment Configuration (WSL2, Docker Desktop, Remote)

Setiap developer memiliki setup berbeda (Docker di WSL2, Docker Desktop Windows, Remote Server, atau DevContainer). Sistem dikonfigurasi untuk mendukung **Dynamic Environment Overrides**:

### 9.1 Hierarki Konfigurasi ASP.NET Core
1. `appsettings.json` (Base default)
2. `appsettings.Development.json` (Local developer defaults)
3. **Environment Variables / `.env` (Prioritas Tertinggi — Overrides)**

Di .NET, separator JSON hierarkis menggunakan tanda **double underscore (`__`)**:
- `ConnectionStrings:DefaultConnection` di-override oleh `ConnectionStrings__DefaultConnection`
- `Redis:Configuration` di-override oleh `Redis__Configuration`
- `MinIO:Endpoint` di-override oleh `MinIO__Endpoint`
- `Camunda:RestUri` di-override oleh `Camunda__RestUri`

### 9.2 Panduan Skenario Lingkungan (Scenarios)

#### Skenario 1: Docker Berjalan di WSL2 (Developer di Windows / Host)
- Port docker di WSL2 secara default di-forward otomatis ke `localhost` Windows oleh WSL2 networking.
- Jika localhost mirroring aktif: Cukup gunakan `localhost` (default).
- Jika menggunakan WSL Bridged IP (misal `172.x.x.x`):
  ```bash
  # Di Windows PowerShell sebelum menjalankan dotnet run
  $env:ConnectionStrings__DefaultConnection = "Host=172.28.160.1;Port=5432;Database=campaign_saas_dev;Username=saas_admin;Password=DevPassword123!"
  $env:Redis__Configuration = "172.28.160.1:6379"
  $env:RabbitMQ__Host = "172.28.160.1"
  $env:MinIO__Endpoint = "172.28.160.1:9000"
  $env:Camunda__RestUri = "http://172.28.160.1:8080/engine-rest"
  $env:JsReport__Uri = "http://172.28.160.1:5488"
  dotnet run --project src/server/Presentation/Api
  ```

#### Skenario 2: Full Containerized Dev (Semua di dalam Docker Network)
- Jika backend .NET dijalankan di dalam container Docker:
  ```bash
  ConnectionStrings__DefaultConnection="Host=postgres;Port=5432;Database=campaign_saas_dev;Username=saas_admin;Password=DevPassword123!"
  Redis__Configuration="redis:6379"
  RabbitMQ__Host="rabbitmq"
  MinIO__Endpoint="minio:9000"
  Camunda__RestUri="http://camunda:8080/engine-rest"
  JsReport__Uri="http://jsreport:5488"
  ```

### 9.3 File Template `.env.example`
Di root repositori disediakan template [`.env.example`](file:///d:/Jack/dotnet/campaign-saas/.env.example). 
Developer cukup menduplikasi menjadi `.env` dan menyesuaikan variabel sesuai lingkungan lokal masing-masing tanpa perlu mengubah file kode atau git-tracked configuration.

