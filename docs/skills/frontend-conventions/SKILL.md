# Campaign SaaS — Frontend Development Conventions

Panduan ini dibaca sebelum menulis atau mengubah kode di **frontend Angular 21** project Campaign SaaS. Wajib dipatuhi bersama `AGENTS.md` sebagai source of truth utama.

## Stack Frontend

- **Framework**: Angular 21, **Standalone Components** (tidak pakai NgModule)
- **Reactivity**: **Signals** — hindari pola lama berbasis `Observable` + `async` pipe untuk state lokal component
- **Control Flow**: pakai syntax modern `@if / @for / @defer`, jangan pakai `*ngIf / *ngFor` (deprecated di project ini)
- **Language**: TypeScript 5.x, strict mode aktif
- **Styling**: **Tailwind CSS v4** — jangan tulis custom CSS kecuali benar-benar tidak bisa dicapai lewat utility class
- **Icons**: **Lucide Angular**
- **Drag & Drop**: `@angular/cdk/drag-drop` (dipakai khusus untuk Kanban Roster Board di modul Creator Assignment)
- **Notifications**: **ngx-sonner** untuk toast, jangan buat toast custom sendiri

## Struktur Folder

```
src/app/
├── core/                       # Singleton services, interceptors, guards
│   ├── interceptors/           # Auth token, error handling, tenant header
│   ├── guards/                 # Auth guard, role guard
│   └── services/               # Service lintas-fitur (ApiClient, TenantContext)
├── shared/                     # Component/pipe/directive reusable lintas fitur
│   ├── components/
│   ├── pipes/
│   └── directives/
├── features/                   # Satu folder per domain/modul, sejajar dengan modul backend
│   ├── campaign/
│   │   ├── pages/              # Route-level component (smart component)
│   │   ├── components/         # Presentational component (dumb component)
│   │   ├── services/           # API service khusus fitur ini
│   │   ├── models/             # Interface/type sesuai API contract
│   │   └── campaign.routes.ts  # Lazy-loaded route config
│   ├── creator/
│   ├── deliverable/
│   ├── approval/
│   └── reporting/
└── app.routes.ts               # Root routing, lazy load tiap feature
```

Struktur `features/` sejajar dengan modul backend (Campaign, Creator, Deliverable, Approval, Reporting, dst) supaya mudah ditelusuri lintas stack.

## Component Convention

- Semua component **Standalone** (`standalone: true` implisit di Angular 21, tidak perlu declare di NgModule).
- Pisahkan **smart component** (di `pages/`, handle data fetching & state) dari **presentational component** (di `components/`, hanya terima `input()` dan emit `output()`, tanpa service injection).
- Gunakan `input()` dan `output()` function-based API (bukan decorator `@Input()/@Output()` lama), sesuai standar Angular modern.
- Naming file: `kebab-case`, contoh `campaign-list.page.ts`, `creator-card.component.ts`.
- Naming class: `PascalCase`, contoh `CampaignListPage`, `CreatorCardComponent`.

```typescript
@Component({
  selector: 'app-creator-card',
  standalone: true,
  imports: [LucideAngularModule],
  template: `...`
})
export class CreatorCardComponent {
  creator = input.required<Creator>();
  assign = output<string>();
}
```

## State Management dengan Signals

- State lokal component pakai `signal()`, computed value pakai `computed()`, side effect pakai `effect()` secukupnya (hindari overuse `effect` untuk logic yang sebetulnya bisa jadi `computed`).
- Untuk state yang dibagi antar component dalam satu fitur, buat service dengan `providedIn: 'root'` atau scoped ke route, isinya signal-based store — jangan pakai NgRx/Akita kecuali sudah ada architectural justification (ADR) baru.

```typescript
export class CampaignStore {
  private campaigns = signal<Campaign[]>([]);
  readonly activeCampaigns = computed(() =>
    this.campaigns().filter(c => c.status === 'Active')
  );
}
```

## Modern Control Flow

Selalu pakai syntax built-in Angular 21:
```html
@if (campaign(); as c) {
  <app-campaign-detail [campaign]="c" />
} @else {
  <app-empty-state />
}

@for (item of deliverables(); track item.id) {
  <app-deliverable-row [deliverable]="item" />
}

@defer (on viewport) {
  <app-reporting-chart [data]="metrics()" />
} @placeholder {
  <app-skeleton />
}
```
- `@for` WAJIB pakai `track` dengan unique identifier (biasanya `id`), jangan `track $index` kecuali list benar-benar statis.
- `@defer` dipakai untuk komponen berat/non-critical (chart, PDF preview) supaya tidak memperlambat initial render.

## API Communication

- Setiap fitur punya service sendiri di `features/<fitur>/services/`, memanggil backend lewat `HttpClient`.
- Tipe request/response WAJIB didefinisikan di `models/`, sinkron dengan API Contract backend (`API_CONTRACT.md`) — jangan pakai `any`.
- Semua request otomatis menyertakan tenant/auth header lewat HTTP Interceptor di `core/interceptors/`, jangan set header manual di tiap service call.
- Error handling terpusat lewat interceptor yang mem-parsing RFC 7807 `ProblemDetails` dari backend, lalu tampilkan lewat `ngx-sonner` — jangan `try-catch` + `alert()` manual di component.

## Styling (Tailwind CSS v4)

- Gunakan utility class langsung di template.
- Untuk pola yang berulang (button, card, badge), buat komponen shared di `shared/components/`, bukan menyalin-tempel class panjang di banyak tempat.
- Ikuti design token yang sudah disepakati di `docs/03-design/` (kalau ada) untuk warna, spacing, radius — jangan hardcode warna hex baru tanpa alasan.

## Routing & Lazy Loading

- Setiap feature folder punya `*.routes.ts` sendiri, di-lazy-load dari `app.routes.ts` pakai `loadChildren`/`loadComponent`.
- Guard (`AuthGuard`, `RoleGuard`) diterapkan di level route config, bukan dicek manual di dalam component.

## Testing

- Unit test component pakai Angular Testing Library atau `TestBed` standar — testing behavior (apa yang user lihat/klik), bukan implementation detail internal.
- E2E test pakai **Playwright**, mencakup core workflow: Campaign creation → Creator assignment → Deliverable submission → Approval.
- Jangan hapus test yang sudah ada tanpa alasan eksplisit.

## Checklist Sebelum Commit/PR

- [ ] Component standalone, tidak ada NgModule baru
- [ ] Pakai Signals untuk state, bukan pola lama `BehaviorSubject` untuk local state
- [ ] Pakai `@if/@for/@defer`, bukan `*ngIf/*ngFor`
- [ ] `@for` punya `track` dengan identifier yang jelas
- [ ] Tidak ada `any` di model/interface API
- [ ] Styling pakai Tailwind utility class, bukan custom CSS baru
- [ ] Smart/presentational component terpisah dengan jelas
- [ ] Route baru sudah lazy-loaded + guard terpasang jika perlu proteksi