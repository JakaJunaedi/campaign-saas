# Definition of Done (DoD) — Campaign SaaS

## 1. Document Control
- **Title**: Definition of Done (DoD) Standard
- **Purpose**: Standar kualitas wajib sebelum User Story atau Task dianggap selesai (*Done*).
- **Status**: ACCEPTED
- **Scope**: Quality & Engineering Standards

---

## 2. Checklist Definition of Done

### 2.1 Backend (.NET 10)
- [ ] Kode mematuhi batasan Modular Monolith (tidak ada akses langsung ke DB/Entity modul lain).
- [ ] Entitas bisnis mengimplementasikan validasi invariant dan tenant isolation (`OrganizationId`).
- [ ] Input DTO divalidasi menggunakan FluentValidation.
- [ ] Unit Tests mencakup business logic domain dengan coverage minimal 80%.
- [ ] Integration Tests memverifikasi database persistence dan API response.
- [ ] Tidak ada compiler warnings (<Nullable>enable</Nullable> dipatuhi penuh).

### 2.2 Frontend (Angular 21 Standalone)
- [ ] Komponen berjenis Standalone Component murni (tanpa NgModule).
- [ ] State dan reaktivitas menggunakan Angular Signals (`signal()`, `computed()`, `effect()`).
- [ ] Menggunakan Modern Control Flow (`@if`, `@for`, `@defer`).
- [ ] Form menggunakan Typed Reactive Forms dengan validasi error message yang jelas.
- [ ] Desain responsif dan diuji pada resolusi desktop dan mobile viewport.

### 2.3 Deployment & Version Control
- [ ] Pull Request telah direview dan lulus seluruh pipeline CI (Lint, Test, Build).
- [ ] Dokumentasi API / OpenAPI Swagger terbarui secara otomatis.
- [ ] Tidak ada secret, token, atau connection string yang di-hardcode ke repositori.
