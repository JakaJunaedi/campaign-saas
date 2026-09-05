# Master Test Plan — Campaign SaaS

## 1. Document Control
- **Title**: Master Test Plan & Quality Assurance Strategy
- **Purpose**: Menetapkan strategi pengujian menyeluruh (Unit, Integration, Security, Multi-Tenant Isolation, E2E).
- **Status**: ACCEPTED
- **Scope**: Testing & Quality Standards

---

## 2. Test Pyramid & Automation Strategy

```plaintext
          /         /       E2E Tests (Playwright / Cypress) - ~10%
        /           /         API & Integration Tests (WebApplicationFactory + Testcontainers) - ~30%
      /________     /           Unit Tests (.NET xUnit / FluentAssertions / Angular Jasmine) - ~60%
    /____________```

---

## 3. Critical Security & Isolation Test Cases (Wajib Ada)

### 3.1 Tenant Isolation Tests (CRITICAL)
- **TEST-ISO-01**: User dari `Organization A` mencoba melakukan query `GET /api/v1/campaigns/{id_of_org_b}` -> WAJIB mengembalikan `404 Not Found` atau `403 Forbidden`.
- **TEST-ISO-02**: Creator dari `Organization A` mencoba mengunggah draft ke deliverable `Organization B` -> WAJIB ditolak.
- **TEST-ISO-03**: Direct query database tanpa tenant context wajib difilter oleh Global Query Filter EF Core.

### 3.2 State Machine Invariant Tests
- **TEST-STA-01**: Deliverable berstatus `Pending` tidak dapat langsung diubah menjadi `Published` tanpa melalui `Submitted` dan `Approved`.
- **TEST-STA-02**: Creator tidak dapat submit konten pada campaign yang berstatus `Paused` atau `Cancelled`.

---

## 4. Performance & Load Testing Targets
- **API Throughput**: 500 concurrent requests dengan p95 latency < 300ms.
- **File Upload Stress**: 50 simultaneous video uploads (100MB each) langsung ke MinIO tanpa lonjakan beban CPU/RAM signifikan pada API host.
