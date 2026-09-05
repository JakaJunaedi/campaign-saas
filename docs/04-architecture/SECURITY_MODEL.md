# Security & Authorization Model — Campaign SaaS

## 1. Document Control
- **Title**: Security & Tenant Isolation Model
- **Purpose**: Kebijakan otentikasi, otorisasi RBAC, pencegahan kebocoran data antar-tenant, dan perlindungan file media.
- **Status**: ACCEPTED
- **Scope**: System-Wide Security Controls

---

## 2. Authentication & Session Management
- **Mekanisme**: Stateless JWT (JSON Web Token) dengan masa aktif 15 menit + Refresh Token di database/Redis.
- **Claims dalam JWT Payload**:
  - `sub`: User ID (`Guid`)
  - `org_id`: Active Organization / Tenant ID (`Guid`)
  - `role`: Peran pengguna (`SuperAdmin`, `AgencyOwner`, `CampaignManager`, `ContentReviewer`, `Creator`)
  - `email`: Email terdaftar

---

## 3. RBAC Matrix (Role-Based Access Control)

| Modul & Tindakan | SuperAdmin (Root) | Agency Owner | Campaign Manager | Content Reviewer | Creator (Guest) |
| :--- | :---: | :---: | :---: | :---: | :---: |
| **Platform Ops & Tenant Management** | ✅ | ❌ | ❌ | ❌ | ❌ |
| **Suspend / Activate Organizations** | ✅ | ❌ | ❌ | ❌ | ❌ |
| **Cross-Tenant Global Audit Logs** | ✅ | ❌ | ❌ | ❌ | ❌ |
| **Manage Org Settings & Team** | ✅ (Override) | ✅ | ❌ | ❌ | ❌ |
| **Manage Clients (CRUD)** | ✅ (Override) | ✅ | ✅ | ❌ | ❌ |
| **Create & Edit Campaign** | ✅ (Override) | ✅ | ✅ | ❌ | ❌ |
| **Assign Creator to Roster** | ✅ (Override) | ✅ | ✅ | ❌ | ❌ |
| **Manage Deliverables** | ✅ (Override) | ✅ | ✅ | ❌ | ❌ |
| **Submit Content Draft** | ❌ | ❌ | ❌ | ❌ | ✅ (Assigned Only) |
| **Review & Request Revision** | ✅ (Override) | ✅ | ✅ | ✅ | ❌ |
| **Approve / Reject Content** | ✅ (Override) | ✅ | ✅ | ✅ | ❌ |
| **Input Manual Metrics** | ✅ (Override) | ✅ | ✅ | ❌ | ❌ |
| **Generate & Download Report** | ✅ (Override) | ✅ | ✅ | ✅ | ❌ |

---

## 4. Multi-Tenant Isolation Enforcement
1. **Server-Side Validation**: Backend memvalidasi bahwa setiap mutasi data memastikan `Resource.OrganizationId == CurrentUser.OrganizationId`.
2. **EF Core Global Query Filter**: Query otomatis menambahkan kondisi `WHERE organization_id = @CurrentOrgId`.
3. **No Direct ID Enumeration**: Seluruh ID entitas menggunakan UUID v4 acak (mencegah Insecure Direct Object References / IDOR).

---

## 5. Secure File Storage & Access (MinIO)
- Bucket MinIO berstatus **Private**.
- Klien browser hanya dapat mengunggah dan mengunduh file melalui **Pre-Signed URL** yang dihasilkan oleh backend setelah memvalidasi hak akses (`Expiration: 15 menit`).
