# ADR-004: Server-Side Multi-Tenancy Isolation via EF Core Global Query Filters

## Status
ACCEPTED

## Context
Campaign SaaS adalah platform multi-tenant di mana kebocoran data antar agensi (Tenant A melihat data Client/Creator Tenant B) adalah pelanggaran keamanan fatal (*catastrophic security failure*). Model arsitektur database yang dipilih adalah *Shared Database, Shared Schema* untuk efisiensi biaya infrastruktur pada fase MVP.

## Decision
Kami memutuskan untuk menerapkan perlindungan **Server-Side Defense-in-Depth Multi-Tenancy**:
1. Setiap entitas bisnis wajib mengimplementasikan interface `ITenantEntity` yang memiliki property `OrganizationId`.
2. Setiap request API mengekstrak `org_id` dari token JWT terotentikasi dan menyimpannya di Scoped Service `ITenantContext`.
3. Entity Framework Core 10 dikonfigurasi dengan **Global Query Filter** pada `OnModelCreating`:
   ```csharp
   modelBuilder.Entity<T>().HasQueryFilter(e => e.OrganizationId == _tenantContext.OrganizationId && !e.IsDeleted);
   ```
4. Semua operasi `SaveChanges()` secara otomatis mengisi `OrganizationId` dari `ITenantContext` sebelum persist ke PostgreSQL.

## Alternatives Considered
- **Database per Tenant**: Ditolak untuk MVP karena kompleksitas manajemen migrasi database dan biaya server tinggi.
- **Manual WHERE Clause di Setiap Repository**: Ditolak karena rentan terhadap kelalaian developer (*human error*).

## Consequences
- **Positif**:
  - Perlindungan otomatis dan konsisten terhadap kebocoran data lintas-tenant.
  - Kode query use-case (CQRS Handlers) tetap bersih tanpa perlu menulis `WHERE organization_id = ...` berulang-ulang.
- **Negatif**:
  - Background worker / super-admin query yang perlu membaca lintas-tenant harus secara eksplisit memanggil `.IgnoreQueryFilters()`.

## Implementation Impact
- Unit test wajib menyertakan skenario `TEST-ISO-01` (memastikan query ditolak saat beda tenant).
