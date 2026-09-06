import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { AdminService } from '../../core/services/admin.service';
import { AdminOverview, AdminOrganizationItem } from '../../core/models/admin.model';
import { toast } from 'ngx-sonner';
import {
  LucideAngularModule,
  ShieldCheck,
  Building2,
  Users,
  Activity,
  Search,
  RefreshCw,
  PowerOff,
  CheckCircle2,
  AlertTriangle,
  ChevronLeft,
  ChevronRight
} from 'lucide-angular';

@Component({
  selector: 'app-admin-overview',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    LucideAngularModule
  ],
  template: `
    <div class="space-y-8">
      <!-- Header Banner -->
      <div class="p-6 sm:p-8 rounded-3xl bg-gradient-to-r from-slate-900 via-indigo-950 to-slate-900 border border-slate-800 text-white relative overflow-hidden shadow-xl shadow-slate-950/40">
        <div class="absolute -right-10 -bottom-10 w-64 h-64 bg-indigo-500/10 rounded-full blur-3xl pointer-events-none"></div>

        <div class="relative z-10 flex flex-col md:flex-row md:items-center justify-between gap-6">
          <div class="space-y-2">
            <div class="inline-flex items-center gap-2 px-3 py-1 rounded-full bg-rose-500/20 border border-rose-400/30 text-rose-200 text-xs font-semibold">
              <lucide-icon [img]="ShieldCheckIcon" class="w-3.5 h-3.5 text-rose-400"></lucide-icon>
              <span>SuperAdmin Access</span>
            </div>
            <h1 class="text-2xl sm:text-3xl font-extrabold tracking-tight text-white">
              Global Platform Management
            </h1>
            <p class="text-slate-300 text-sm max-w-2xl leading-relaxed">
              SuperAdmin control plane for multi-tenant organizations, global user base, system health monitoring, and tenant lifecycle operations.
            </p>
          </div>

          <div class="flex items-center gap-3 shrink-0">
            <button
              (click)="refreshAll()"
              class="px-4 py-2.5 rounded-xl bg-white/10 hover:bg-white/15 border border-white/20 text-white text-xs font-semibold shadow-xs transition flex items-center gap-2"
              [disabled]="loadingOverview() || loadingOrgs()"
            >
              <lucide-icon [img]="RefreshCwIcon" class="w-4 h-4" [class.animate-spin]="loadingOverview() || loadingOrgs()"></lucide-icon>
              <span>Refresh Control Plane</span>
            </button>
          </div>
        </div>
      </div>

      <!-- SuperAdmin Platform Stats -->
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 sm:gap-6">
        <div class="p-5 bg-white rounded-2xl border border-slate-200/80 shadow-xs space-y-3">
          <div class="flex items-center justify-between">
            <span class="text-xs font-medium text-slate-500 uppercase tracking-wider">Total Tenants</span>
            <div class="w-9 h-9 rounded-xl bg-indigo-50 text-indigo-600 flex items-center justify-center">
              <lucide-icon [img]="Building2Icon" class="w-4 h-4"></lucide-icon>
            </div>
          </div>
          <div>
            @if (loadingOverview()) {
              <div class="h-8 w-16 bg-slate-100 animate-pulse rounded-lg"></div>
            } @else {
              <span class="text-2xl font-bold text-slate-900">{{ overview()?.totalTenants ?? 0 }}</span>
              <span class="text-xs text-indigo-600 font-medium ml-2">Organizations</span>
            }
          </div>
          <p class="text-[11px] text-slate-400">Isolated database schemas</p>
        </div>

        <div class="p-5 bg-white rounded-2xl border border-slate-200/80 shadow-xs space-y-3">
          <div class="flex items-center justify-between">
            <span class="text-xs font-medium text-slate-500 uppercase tracking-wider">Active Tenants</span>
            <div class="w-9 h-9 rounded-xl bg-emerald-50 text-emerald-600 flex items-center justify-center">
              <lucide-icon [img]="CheckCircle2Icon" class="w-4 h-4"></lucide-icon>
            </div>
          </div>
          <div>
            @if (loadingOverview()) {
              <div class="h-8 w-16 bg-slate-100 animate-pulse rounded-lg"></div>
            } @else {
              <span class="text-2xl font-bold text-slate-900">{{ overview()?.activeTenants ?? 0 }}</span>
              <span class="text-xs text-emerald-600 font-medium ml-2">Live & Active</span>
            }
          </div>
          <p class="text-[11px] text-slate-400">100% operational status</p>
        </div>

        <div class="p-5 bg-white rounded-2xl border border-slate-200/80 shadow-xs space-y-3">
          <div class="flex items-center justify-between">
            <span class="text-xs font-medium text-slate-500 uppercase tracking-wider">Global Users</span>
            <div class="w-9 h-9 rounded-xl bg-violet-50 text-violet-600 flex items-center justify-center">
              <lucide-icon [img]="UsersIcon" class="w-4 h-4"></lucide-icon>
            </div>
          </div>
          <div>
            @if (loadingOverview()) {
              <div class="h-8 w-16 bg-slate-100 animate-pulse rounded-lg"></div>
            } @else {
              <span class="text-2xl font-bold text-slate-900">{{ overview()?.totalUsers ?? 0 }}</span>
              <span class="text-xs text-violet-600 font-medium ml-2">Platform Users</span>
            }
          </div>
          <p class="text-[11px] text-slate-400">Cross-tenant aggregate</p>
        </div>

        <div class="p-5 bg-white rounded-2xl border border-slate-200/80 shadow-xs space-y-3">
          <div class="flex items-center justify-between">
            <span class="text-xs font-medium text-slate-500 uppercase tracking-wider">System Health</span>
            <div class="w-9 h-9 rounded-xl bg-teal-50 text-teal-600 flex items-center justify-center">
              <lucide-icon [img]="ActivityIcon" class="w-4 h-4"></lucide-icon>
            </div>
          </div>
          <div>
            @if (loadingOverview()) {
              <div class="h-8 w-16 bg-slate-100 animate-pulse rounded-lg"></div>
            } @else {
              <span class="text-2xl font-bold text-emerald-600">{{ overview()?.systemStatus ?? 'Healthy' }}</span>
            }
          </div>
          <p class="text-[11px] text-slate-400">PostgreSQL + RabbitMQ + MinIO</p>
        </div>
      </div>

      <!-- Tenants Directory Table -->
      <div class="p-6 bg-white rounded-2xl border border-slate-200/80 shadow-xs space-y-5">
        <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
          <div>
            <h2 class="text-base font-bold text-slate-900">Tenant Organizations Master</h2>
            <p class="text-xs text-slate-500 mt-0.5">Manage tenant accounts, statuses, and workspace access</p>
          </div>

          <div class="flex items-center gap-3">
            <!-- Search -->
            <div class="relative w-64">
              <lucide-icon [img]="SearchIcon" class="w-4 h-4 absolute left-3 top-1/2 -translate-y-1/2 text-slate-400"></lucide-icon>
              <input
                type="text"
                [(ngModel)]="searchTerm"
                (ngModelChange)="onSearchChange()"
                placeholder="Search organizations..."
                class="w-full pl-9 pr-3 py-2 text-xs rounded-xl border border-slate-200 bg-slate-50 focus:bg-white focus:outline-hidden focus:border-indigo-500 focus:ring-1 focus:ring-indigo-500 transition"
              />
            </div>

            <!-- Status Filter -->
            <select
              [(ngModel)]="statusFilter"
              (ngModelChange)="loadOrganizations()"
              class="px-3 py-2 text-xs rounded-xl border border-slate-200 bg-slate-50 focus:bg-white focus:outline-hidden focus:border-indigo-500 font-medium"
            >
              <option value="">All Statuses</option>
              <option value="Active">Active</option>
              <option value="Suspended">Suspended</option>
            </select>
          </div>
        </div>

        @if (loadingOrgs()) {
          <div class="space-y-3 py-4">
            @for (i of [1, 2, 3, 4]; track i) {
              <div class="h-12 bg-slate-100 animate-pulse rounded-xl"></div>
            }
          </div>
        } @else if (organizations().length === 0) {
          <div class="text-center py-12 px-4 rounded-xl bg-slate-50 border border-dashed border-slate-200 space-y-2">
            <lucide-icon [img]="Building2Icon" class="w-8 h-8 text-slate-400 mx-auto"></lucide-icon>
            <p class="text-xs font-semibold text-slate-700">No organizations found</p>
            <p class="text-[11px] text-slate-400">Try adjusting your search query or filter.</p>
          </div>
        } @else {
          <div class="overflow-x-auto">
            <table class="w-full text-left text-xs text-slate-600">
              <thead class="bg-slate-50 border-b border-slate-200/80 text-[11px] font-bold uppercase text-slate-500 tracking-wider">
                <tr>
                  <th class="py-3 px-4">Organization</th>
                  <th class="py-3 px-4">Slug Identifier</th>
                  <th class="py-3 px-4">Members</th>
                  <th class="py-3 px-4">Status</th>
                  <th class="py-3 px-4">Created Date</th>
                  <th class="py-3 px-4 text-right">Actions</th>
                </tr>
              </thead>
              <tbody class="divide-y divide-slate-100">
                @for (org of organizations(); track org.id) {
                  <tr class="hover:bg-slate-50/80 transition">
                    <td class="py-3 px-4 font-bold text-slate-900">
                      {{ org.name }}
                    </td>
                    <td class="py-3 px-4 font-mono text-[11px] text-indigo-600 font-medium">
                      {{ org.slug }}
                    </td>
                    <td class="py-3 px-4">
                      <span class="inline-flex items-center gap-1.5 px-2 py-0.5 rounded-full bg-slate-100 text-slate-700 font-semibold text-[11px]">
                        <lucide-icon [img]="UsersIcon" class="w-3 h-3 text-slate-500"></lucide-icon>
                        <span>{{ org.usersCount }}</span>
                      </span>
                    </td>
                    <td class="py-3 px-4">
                      <span
                        class="px-2.5 py-0.5 rounded-full text-[11px] font-semibold"
                        [ngClass]="{
                          'bg-emerald-100 text-emerald-700': org.status === 'Active',
                          'bg-rose-100 text-rose-700': org.status === 'Suspended',
                          'bg-amber-100 text-amber-700': org.status === 'Trial'
                        }"
                      >
                        {{ org.status }}
                      </span>
                    </td>
                    <td class="py-3 px-4 text-slate-500 whitespace-nowrap text-[11px]">
                      {{ org.createdAt | date:'mediumDate' }}
                    </td>
                    <td class="py-3 px-4 text-right">
                      @if (org.status === 'Active') {
                        <button
                          (click)="toggleOrgStatus(org, 'Suspended')"
                          class="px-3 py-1.5 rounded-lg bg-rose-50 hover:bg-rose-100 text-rose-700 text-[11px] font-semibold transition inline-flex items-center gap-1.5"
                          title="Suspend Organization"
                        >
                          <lucide-icon [img]="PowerOffIcon" class="w-3.5 h-3.5"></lucide-icon>
                          <span>Suspend</span>
                        </button>
                      } @else {
                        <button
                          (click)="toggleOrgStatus(org, 'Active')"
                          class="px-3 py-1.5 rounded-lg bg-emerald-50 hover:bg-emerald-100 text-emerald-700 text-[11px] font-semibold transition inline-flex items-center gap-1.5"
                          title="Activate Organization"
                        >
                          <lucide-icon [img]="CheckCircle2Icon" class="w-3.5 h-3.5"></lucide-icon>
                          <span>Activate</span>
                        </button>
                      }
                    </td>
                  </tr>
                }
              </tbody>
            </table>
          </div>

          <!-- Pagination -->
          <div class="flex items-center justify-between pt-3 border-t border-slate-100 text-xs text-slate-500">
            <span>Showing {{ organizations().length }} of {{ totalCount() }} organizations</span>
            <div class="flex items-center gap-2">
              <button
                [disabled]="pageNumber() <= 1"
                (click)="onPageChange(pageNumber() - 1)"
                class="px-2.5 py-1.5 rounded-lg border border-slate-200 hover:bg-slate-50 disabled:opacity-50 transition"
              >
                <lucide-icon [img]="ChevronLeftIcon" class="w-4 h-4"></lucide-icon>
              </button>
              <span class="px-2 py-1 font-semibold text-slate-700">{{ pageNumber() }} / {{ totalPages() }}</span>
              <button
                [disabled]="pageNumber() >= totalPages()"
                (click)="onPageChange(pageNumber() + 1)"
                class="px-2.5 py-1.5 rounded-lg border border-slate-200 hover:bg-slate-50 disabled:opacity-50 transition"
              >
                <lucide-icon [img]="ChevronRightIcon" class="w-4 h-4"></lucide-icon>
              </button>
            </div>
          </div>
        }
      </div>
    </div>
  `
})
export class AdminOverviewComponent implements OnInit {
  private readonly adminService = inject(AdminService);

  readonly overview = signal<AdminOverview | null>(null);
  readonly organizations = signal<AdminOrganizationItem[]>([]);
  readonly loadingOverview = signal<boolean>(true);
  readonly loadingOrgs = signal<boolean>(true);

  searchTerm = '';
  statusFilter = '';
  pageNumber = signal<number>(1);
  pageSize = signal<number>(10);
  totalCount = signal<number>(0);
  totalPages = signal<number>(1);

  readonly ShieldCheckIcon = ShieldCheck;
  readonly Building2Icon = Building2;
  readonly UsersIcon = Users;
  readonly ActivityIcon = Activity;
  readonly SearchIcon = Search;
  readonly RefreshCwIcon = RefreshCw;
  readonly PowerOffIcon = PowerOff;
  readonly CheckCircle2Icon = CheckCircle2;
  readonly AlertTriangleIcon = AlertTriangle;
  readonly ChevronLeftIcon = ChevronLeft;
  readonly ChevronRightIcon = ChevronRight;

  ngOnInit(): void {
    this.refreshAll();
  }

  refreshAll(): void {
    this.loadOverview();
    this.loadOrganizations();
  }

  loadOverview(): void {
    this.loadingOverview.set(true);
    this.adminService.getOverview().subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.overview.set(res.data);
        }
        this.loadingOverview.set(false);
      },
      error: () => {
        this.loadingOverview.set(false);
      }
    });
  }

  loadOrganizations(): void {
    this.loadingOrgs.set(true);
    this.adminService.getOrganizations(
      this.searchTerm,
      this.statusFilter,
      this.pageNumber(),
      this.pageSize()
    ).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.organizations.set(res.data.items);
          this.totalCount.set(res.data.totalCount);
          this.totalPages.set(res.data.totalPages || 1);
        }
        this.loadingOrgs.set(false);
      },
      error: () => {
        this.loadingOrgs.set(false);
      }
    });
  }

  onSearchChange(): void {
    this.pageNumber.set(1);
    this.loadOrganizations();
  }

  onPageChange(page: number): void {
    this.pageNumber.set(page);
    this.loadOrganizations();
  }

  toggleOrgStatus(org: AdminOrganizationItem, newStatus: string): void {
    this.adminService.updateOrganizationStatus(org.id, { status: newStatus }).subscribe({
      next: (res) => {
        if (res.success && res.data) {
          toast.success(`Organization '${org.name}' is now ${newStatus}`);
          this.loadOrganizations();
          this.loadOverview();
        }
      },
      error: () => {
        toast.error('Failed to update organization status');
      }
    });
  }
}
