import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="space-y-6">
      <div class="flex items-center justify-between">
        <div>
          <h2 class="text-2xl font-bold text-white tracking-tight">Phase 0: Foundation & Architecture Skeleton</h2>
          <p class="text-slate-400 text-sm mt-1">Status modul .NET 10 Modular Monolith & Angular 21 Standalone</p>
        </div>
        <div class="px-3 py-1 rounded-md bg-emerald-500/10 text-emerald-400 border border-emerald-500/20 text-xs font-semibold">
          Architecture Verified
        </div>
      </div>

      <!-- Modules Grid -->
      <div class="grid grid-cols-1 md:grid-cols-3 gap-4">
        @for (mod of modules(); track mod.name) {
          <div class="p-5 rounded-xl bg-slate-950 border border-slate-800 hover:border-slate-700 transition space-y-3">
            <div class="flex items-center justify-between">
              <h3 class="font-semibold text-slate-200">{{ mod.name }}</h3>
              <span class="text-[10px] font-mono uppercase px-2 py-0.5 rounded bg-slate-800 text-slate-400">{{ mod.status }}</span>
            </div>
            <p class="text-xs text-slate-400">{{ mod.desc }}</p>
            <div class="pt-2 border-t border-slate-900 flex items-center justify-between text-[11px] text-slate-500">
              <span>Domain / App / Infra</span>
              <span class="text-emerald-400">Ready</span>
            </div>
          </div>
        }
      </div>
    </div>
  `
})
export class DashboardComponent {
  readonly modules = signal([
    { name: 'Identity & Multi-Tenancy', status: 'Phase 1 Target', desc: 'Auth JWT, Organization isolation, RBAC Roles' },
    { name: 'Client Management', status: 'Phase 2 Target', desc: 'Brand profiles & multi-PIC contacts' },
    { name: 'Campaign Management', status: 'Phase 3 Target', desc: 'Lifecycle, budget, dates, and brief' },
    { name: 'Creator CRM', status: 'Phase 4 Target', desc: 'Creator database, categories, social handles' },
    { name: 'Deliverable Management', status: 'Phase 5 Target', desc: 'Items, deadlines, platform content types' },
    { name: 'Approval & Workflow', status: 'Phase 6 Target', desc: 'Camunda review loop, media preview, versioning' },
    { name: 'Reporting Engine', status: 'Phase 7 Target', desc: 'Manual metrics & jsreport PDF export' },
    { name: 'Notification Service', status: 'Phase 8 Target', desc: 'In-app real-time event notifications' },
    { name: 'Audit Trail', status: 'Phase 9 Target', desc: 'Cross-tenant security & activity logs' }
  ]);
}
