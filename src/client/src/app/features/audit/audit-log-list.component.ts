import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  LucideAngularModule,
  ShieldCheck,
  Search,
  Filter,
  RefreshCw,
  Loader2,
  Calendar,
  User,
  Layers,
  ChevronLeft,
  ChevronRight,
  Eye,
  FileCode,
  Tag
} from 'lucide-angular';
import { toast } from 'ngx-sonner';
import { AuditService } from '../../core/services/audit.service';
import { AuditLog } from '../../core/models/audit.model';
import { AuditDetailModalComponent } from './audit-detail-modal/audit-detail-modal.component';

@Component({
  selector: 'app-audit-log-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    LucideAngularModule,
    AuditDetailModalComponent
  ],
  template: `
    <div class="space-y-6">
      <!-- Header -->
      <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <div class="flex items-center gap-2">
            <h1 class="text-2xl font-black tracking-tight text-slate-900">Security & Audit Logs</h1>
            <span class="px-2.5 py-0.5 rounded-full bg-indigo-50 text-indigo-700 text-xs font-bold">
              {{ totalCount() }} Events
            </span>
          </div>
          <p class="text-xs text-slate-500 mt-1">
            Immutable cross-module tenant audit trail and security event logging.
          </p>
        </div>

        <button
          type="button"
          (click)="loadAuditLogs()"
          class="p-2.5 rounded-xl border border-slate-200 text-slate-600 hover:bg-slate-50 transition self-start sm:self-auto flex items-center gap-2 text-xs font-semibold"
          title="Refresh Audit Logs"
        >
          <lucide-icon [img]="RefreshCwIcon" class="w-4 h-4" [class.animate-spin]="isLoading()"></lucide-icon>
          <span>Refresh</span>
        </button>
      </div>

      <!-- Filters & Search Toolbar -->
      <div class="p-4 bg-white rounded-2xl border border-slate-200/80 shadow-xs flex flex-col md:flex-row md:items-center gap-3">
        <!-- Module Filter Dropdown -->
        <div class="flex items-center gap-2">
          <span class="text-xs font-bold text-slate-600 shrink-0">Module:</span>
          <select
            [(ngModel)]="selectedModule"
            (ngModelChange)="onFilterChange()"
            class="px-3 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs font-semibold text-slate-800 focus:outline-hidden focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-600 transition cursor-pointer"
          >
            <option value="ALL">All Modules</option>
            <option value="Identity">Identity</option>
            <option value="Client">Client</option>
            <option value="Campaign">Campaign</option>
            <option value="Creator">Creator</option>
            <option value="Deliverable">Deliverable</option>
            <option value="Approval">Approval</option>
            <option value="Reporting">Reporting</option>
          </select>
        </div>

        <!-- Action Filter Dropdown -->
        <div class="flex items-center gap-2">
          <span class="text-xs font-bold text-slate-600 shrink-0">Action:</span>
          <select
            [(ngModel)]="selectedAction"
            (ngModelChange)="onFilterChange()"
            class="px-3 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs font-semibold text-slate-800 focus:outline-hidden focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-600 transition cursor-pointer"
          >
            <option value="ALL">All Actions</option>
            <option value="Created">Created</option>
            <option value="Updated">Updated</option>
            <option value="Deleted">Deleted</option>
            <option value="StatusChanged">StatusChanged</option>
          </select>
        </div>

        <!-- Search by Entity ID or Query -->
        <div class="relative flex-1">
          <lucide-icon [img]="SearchIcon" class="w-4 h-4 absolute left-3.5 top-1/2 -translate-y-1/2 text-slate-400"></lucide-icon>
          <input
            type="text"
            [(ngModel)]="searchEntityId"
            (keyup.enter)="onFilterChange()"
            placeholder="Search by Entity UUID..."
            class="w-full pl-9 pr-3.5 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs font-semibold text-slate-800 focus:outline-hidden focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-600 transition"
          />
        </div>

        <button
          type="button"
          (click)="onFilterChange()"
          class="px-4 py-2 rounded-xl bg-slate-900 text-white hover:bg-slate-800 text-xs font-semibold transition shrink-0"
        >
          Apply Filters
        </button>
      </div>

      <!-- Logs Content Table -->
      @if (isLoading()) {
        <div class="p-16 bg-white rounded-2xl border border-slate-200/80 shadow-xs text-center space-y-3">
          <lucide-icon [img]="Loader2Icon" class="w-8 h-8 text-indigo-600 animate-spin mx-auto"></lucide-icon>
          <p class="text-xs text-slate-500 font-medium">Fetching tenant audit trail...</p>
        </div>
      } @else if (logs().length === 0) {
        <div class="p-16 bg-white rounded-2xl border border-slate-200/80 shadow-xs text-center space-y-3 max-w-md mx-auto">
          <div class="w-12 h-12 rounded-2xl bg-indigo-50 text-indigo-600 flex items-center justify-center mx-auto">
            <lucide-icon [img]="ShieldCheckIcon" class="w-6 h-6"></lucide-icon>
          </div>
          <h3 class="text-sm font-bold text-slate-900">No Audit Logs Found</h3>
          <p class="text-xs text-slate-500 leading-relaxed">
            There are no recorded audit events matching the selected module, action, or entity filters.
          </p>
        </div>
      } @else {
        <div class="bg-white rounded-2xl border border-slate-200/80 shadow-xs overflow-hidden">
          <div class="overflow-x-auto">
            <table class="w-full text-left border-collapse">
              <thead>
                <tr class="border-b border-slate-100 bg-slate-50/50 text-[11px] font-bold text-slate-500 uppercase tracking-wider">
                  <th class="py-3.5 px-6">Timestamp</th>
                  <th class="py-3.5 px-4">Actor</th>
                  <th class="py-3.5 px-4">Module & Entity</th>
                  <th class="py-3.5 px-4">Action</th>
                  <th class="py-3.5 px-4">Entity ID</th>
                  <th class="py-3.5 px-6 text-right">Payload</th>
                </tr>
              </thead>
              <tbody class="divide-y divide-slate-100 text-xs">
                @for (log of logs(); track log.id) {
                  <tr class="hover:bg-slate-50/75 transition group">
                    <!-- Timestamp -->
                    <td class="py-3.5 px-6 text-slate-600 font-medium">
                      {{ log.timestamp | date:'medium' }}
                    </td>

                    <!-- Actor -->
                    <td class="py-3.5 px-4 font-semibold text-slate-800">
                      {{ log.actorEmail || 'System' }}
                    </td>

                    <!-- Module & Entity -->
                    <td class="py-3.5 px-4">
                      <span class="inline-flex items-center gap-1.5 px-2.5 py-0.5 rounded-full bg-slate-100 text-slate-700 font-semibold text-[11px]">
                        <span>{{ log.module }}</span>
                        <span class="text-slate-400">•</span>
                        <span>{{ log.entityName }}</span>
                      </span>
                    </td>

                    <!-- Action Badge -->
                    <td class="py-3.5 px-4">
                      <span
                        class="px-2.5 py-0.5 rounded-full text-[10px] font-bold uppercase tracking-wider inline-block"
                        [ngClass]="getActionBadgeClass(log.action)"
                      >
                        {{ log.action }}
                      </span>
                    </td>

                    <!-- Entity ID -->
                    <td class="py-3.5 px-4 font-mono text-slate-500 text-[11px]">
                      {{ log.entityId.substring(0, 8) }}...
                    </td>

                    <!-- Inspect Payload Button -->
                    <td class="py-3.5 px-6 text-right">
                      <button
                        type="button"
                        (click)="openDetail(log)"
                        class="px-3 py-1.5 rounded-xl border border-slate-200 text-slate-700 hover:bg-indigo-50 hover:text-indigo-600 hover:border-indigo-200 text-xs font-semibold transition inline-flex items-center gap-1.5"
                      >
                        <lucide-icon [img]="FileCodeIcon" class="w-3.5 h-3.5"></lucide-icon>
                        <span>Inspect</span>
                      </button>
                    </td>
                  </tr>
                }
              </tbody>
            </table>
          </div>

          <!-- Pagination Bar -->
          <div class="px-6 py-4 border-t border-slate-100 flex flex-col sm:flex-row sm:items-center justify-between gap-3 bg-slate-50/50">
            <div class="text-xs text-slate-500">
              Showing <span class="font-bold text-slate-700">{{ ((currentPage() - 1) * pageSize) + 1 }}</span> to
              <span class="font-bold text-slate-700">{{ getEndItemIndex() }}</span> of
              <span class="font-bold text-slate-700">{{ totalCount() }}</span> logs
            </div>

            <div class="flex items-center gap-2 self-center sm:self-auto">
              <button
                type="button"
                (click)="prevPage()"
                [disabled]="currentPage() <= 1"
                class="p-2 rounded-xl border border-slate-200 text-slate-600 hover:bg-white disabled:opacity-40 transition"
              >
                <lucide-icon [img]="ChevronLeftIcon" class="w-4 h-4"></lucide-icon>
              </button>

              <span class="text-xs font-bold text-slate-700 px-2">
                Page {{ currentPage() }} of {{ totalPages() }}
              </span>

              <button
                type="button"
                (click)="nextPage()"
                [disabled]="currentPage() >= totalPages()"
                class="p-2 rounded-xl border border-slate-200 text-slate-600 hover:bg-white disabled:opacity-40 transition"
              >
                <lucide-icon [img]="ChevronRightIcon" class="w-4 h-4"></lucide-icon>
              </button>
            </div>
          </div>
        </div>
      }

      <!-- Audit Detail Modal -->
      <app-audit-detail-modal
        [isOpen]="isDetailModalOpen()"
        [log]="selectedLogForDetail()"
        (close)="closeDetail()"
      ></app-audit-detail-modal>
    </div>
  `
})
export class AuditLogListComponent implements OnInit {
  private auditService = inject(AuditService);

  readonly logs = signal<AuditLog[]>([]);
  readonly totalCount = signal<number>(0);
  readonly currentPage = signal<number>(1);
  readonly pageSize = 50;
  readonly isLoading = signal<boolean>(false);

  selectedModule = 'ALL';
  selectedAction = 'ALL';
  searchEntityId = '';

  readonly isDetailModalOpen = signal<boolean>(false);
  readonly selectedLogForDetail = signal<AuditLog | null>(null);

  // Icons
  readonly ShieldCheckIcon = ShieldCheck;
  readonly SearchIcon = Search;
  readonly FilterIcon = Filter;
  readonly RefreshCwIcon = RefreshCw;
  readonly Loader2Icon = Loader2;
  readonly CalendarIcon = Calendar;
  readonly UserIcon = User;
  readonly LayersIcon = Layers;
  readonly ChevronLeftIcon = ChevronLeft;
  readonly ChevronRightIcon = ChevronRight;
  readonly EyeIcon = Eye;
  readonly FileCodeIcon = FileCode;
  readonly TagIcon = Tag;

  ngOnInit(): void {
    this.loadAuditLogs();
  }

  loadAuditLogs(): void {
    this.isLoading.set(true);
    const mod = this.selectedModule === 'ALL' ? undefined : this.selectedModule;
    const act = this.selectedAction === 'ALL' ? undefined : this.selectedAction;
    const entity = this.searchEntityId.trim() || undefined;

    this.auditService
      .getAuditLogs(mod, act, entity, this.currentPage(), this.pageSize)
      .subscribe({
        next: res => {
          this.isLoading.set(false);
          if (res.success && res.data) {
            this.logs.set(res.data.items);
            this.totalCount.set(res.data.totalCount);
          }
        },
        error: () => {
          this.isLoading.set(false);
          toast.error('Failed to load audit logs');
        }
      });
  }

  onFilterChange(): void {
    this.currentPage.set(1);
    this.loadAuditLogs();
  }

  totalPages(): number {
    return Math.ceil(this.totalCount() / this.pageSize) || 1;
  }

  getEndItemIndex(): number {
    return Math.min(this.currentPage() * this.pageSize, this.totalCount());
  }

  prevPage(): void {
    if (this.currentPage() > 1) {
      this.currentPage.update(p => p - 1);
      this.loadAuditLogs();
    }
  }

  nextPage(): void {
    if (this.currentPage() < this.totalPages()) {
      this.currentPage.update(p => p + 1);
      this.loadAuditLogs();
    }
  }

  openDetail(log: AuditLog): void {
    this.selectedLogForDetail.set(log);
    this.isDetailModalOpen.set(true);
  }

  closeDetail(): void {
    this.isDetailModalOpen.set(false);
    this.selectedLogForDetail.set(null);
  }

  getActionBadgeClass(action: string): string {
    switch (action?.toLowerCase()) {
      case 'created':
      case 'inserted':
        return 'bg-emerald-50 text-emerald-700 border border-emerald-200';
      case 'updated':
      case 'modified':
        return 'bg-blue-50 text-blue-700 border border-blue-200';
      case 'deleted':
      case 'removed':
        return 'bg-rose-50 text-rose-700 border border-rose-200';
      case 'statuschanged':
        return 'bg-violet-50 text-violet-700 border border-violet-200';
      default:
        return 'bg-slate-100 text-slate-700 border border-slate-200';
    }
  }
}
