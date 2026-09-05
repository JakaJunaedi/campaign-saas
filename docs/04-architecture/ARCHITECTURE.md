# System Architecture & Modular Monolith Design — Campaign SaaS

## 1. Document Control
- **Title**: System Architecture Specification
- **Purpose**: Menjadi Single Source of Truth untuk arsitektur teknis sistem, batasan modul, dan pedoman implementasi backend .NET 10 dan frontend Angular 21.
- **Status**: ACCEPTED
- **Scope**: System Architecture & Module Boundaries
- **Architectural Paradigm**: **Modular Monolith (Microservices-Ready)**

---

## 2. High-Level Architecture

```mermaid
graph TD
    ClientSPA[Angular 21 Frontend (SPA)] -->|HTTPS / REST API| HostAPI[ASP.NET Core 10 Web Host API]

    subgraph ModularMonolith [.NET 10 Modular Monolith Application]
        HostAPI --> ModIdentity[Module: Identity]
        HostAPI --> ModClient[Module: Client]
        HostAPI --> ModCampaign[Module: Campaign]
        HostAPI --> ModCreator[Module: Creator]
        HostAPI --> ModDeliverable[Module: Deliverable]
        HostAPI --> ModApproval[Module: Approval]
        HostAPI --> ModReporting[Module: Reporting]
        HostAPI --> ModNotification[Module: Notification]
        HostAPI --> ModAudit[Module: Audit]
    end

    subgraph InfrastructureServices [Shared Infrastructure & State]
        ModIdentity --> PG[(PostgreSQL Database)]
        ModClient --> PG
        ModCampaign --> PG
        ModCreator --> PG
        ModDeliverable --> PG
        ModApproval --> PG
        ModReporting --> PG
        ModNotification --> PG
        ModAudit --> PG

        HostAPI --> Redis[(Redis Cache & Idempotency)]
        HostAPI --> RabbitMQ[(RabbitMQ Message Broker)]
        HostAPI --> Camunda[Camunda BPMN Workflow Engine]
        HostAPI --> MinIO[(MinIO Object Storage)]
        HostAPI --> jsreport[jsreport PDF Service]
    end
```

---

## 3. Modular Monolith Architecture & Boundaries

Sistem dibangun sebagai satu unit deployment monolitik (*Single Deployable Unit*) tetapi dibagi secara ketat menjadi 9 modul terisolasi.

### 3.1 Daftar Modul & Tanggung Jawab
1. **`Identity`**: Otentikasi, manajemen organisasi (multi-tenancy), user, role, dan permission.
2. **`Client`**: Pengelolaan data brand pengiklan dan kontak PIC klien.
3. **`Campaign`**: Pengelolaan siklus hidup campaign, budget, jadwal, dan brief.
4. **`Creator`**: CRM database profil kreator, akun media sosial, dan kategori niche.
5. **`Deliverable`**: Manajemen item deliverable, penugasan ke kreator, upload draft konten, dan versioning media.
6. **`Approval`**: Alur evaluasi konten, permintaan revisi, persetujuan, dan komentar tim.
7. **`Reporting`**: Pengelolaan metrik performa manual, kalkulasi engagement rate/CPE, dan orkestrasi pembuatan PDF report.
8. **`Notification`**: Pengiriman notifikasi in-app kepada user dan kreator.
9. **`Audit`**: Pencatatan audit trail untuk aksi bisnis kritis.

---

## 4. Struktur Internal Setiap Modul
Setiap modul memiliki struktur 4 layer standar:
```plaintext
Modules/
└── {ModuleName}/
    ├── Domain/               # Entities, Enums, Value Objects, Domain Events, Invariants
    ├── Application/          # CQRS Commands & Queries, DTOs, Handlers, FluentValidators
    ├── Infrastructure/       # EF Core Configurations, Repositories, Gateway Adapters
    └── Contracts/            # Public Integration Events & In-Process Query Interfaces
```

---

## 5. Strict Module Rules (Wajib Dipatuhi AI Agent & Developer)
1. **Rule 1**: Modul TIDAK BOLEH mengakses repository atau `DbContext` modul lain secara langsung.
2. **Rule 2**: Modul TIDAK BOLEH menggunakan Entity modul lain sebagai dependency/foreign entity navigation.
3. **Rule 3**: Hindari ketergantungan melingkar (*circular dependency*) antar modul.
4. **Rule 4**: Aturan bisnis (*business rules*) harus dimiliki sepenuhnya oleh modul domain yang bersangkutan.
5. **Rule 5**: Gunakan Identifier primitif (`Guid`) dan Contract untuk referensi lintas modul (contoh: `Deliverable.CreatorId`, BUKAN `Deliverable.CreatorEntity`).
6. **Rule 6**: Layer Infrastructure tidak boleh mengandung business logic.
7. **Rule 7**: Controller / Minimal API endpoints tidak boleh mengandung business logic (hanya meneruskan ke CQRS handler).
8. **Rule 8**: DILARANG membuat microservice baru selama fase MVP.
9. **Rule 9**: DILARANG memecah database menjadi multi-database per modul selama MVP.
10. **Rule 10**: DILARANG menambahkan library/teknologi baru tanpa architectural justification.

---

## 6. Komunikasi Antar-Modul
- **Synchronous (In-Process)**: Menggunakan interface contracts / mediator query in-memory untuk membaca data referensi sederhana.
- **Asynchronous (Event-Driven)**: Menggunakan RabbitMQ integration events untuk operasi berantai (contoh: `DeliverableSubmittedEvent` memicu pembuatan notifikasi dan audit log).
- **Outbox Pattern**: Mencegah kegagalan pengiriman event asinkron saat transaksi database berhasil disimpan.
