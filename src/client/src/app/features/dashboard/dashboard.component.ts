import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { DashboardService } from '../../core/services/dashboard.service';
import { DashboardOverview } from '../../core/models/dashboard.model';
import {
  LucideAngularModule,
  Megaphone,
  Users,
  FolderKanban,
  BarChart3,
  CheckCircle2,
  Clock,
  ArrowRight,
  Sparkles,
  Building2,
  ShieldCheck,
  Bell,
  ChevronRight,
  RefreshCw
} from 'lucide-angular';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    LucideAngularModule
  ],
  template: `
    <div class="space-y-8">
      <!-- Welcome Header Banner -->
      <div class="p-6 sm:p-8 rounded-3xl bg-gradient-to-r from-indigo-900 via-indigo-950 to-slate-900 border border-indigo-800/40 text-white relative overflow-hidden shadow-xl shadow-indigo-950/30">
        <div class="absolute -right-10 -bottom-10 w-64 h-64 bg-indigo-500/10 rounded-full blur-3xl pointer-events-none"></div>

        <div class="relative z-10 flex flex-col md:flex-row md:items-center justify-between gap-6">
          <div class="space-y-2">
            <div class="inline-flex items-center gap-2 px-3 py-1 rounded-full bg-indigo-500/20 border border-indigo-400/30 text-indigo-200 text-xs font-medium">
              <lucide-icon [img]="SparklesIcon" class="w-3.5 h-3.5 text-indigo-300"></lucide-icon>
              <span>{{ authService.orgName() }} Workspace</span>
            </div>
            <h1 class="text-2xl sm:text-3xl font-extrabold tracking-tight text-white">
              Welcome back, {{ authService.userFullName() }}!
            </h1>
            <p class="text-indigo-200/80 text-sm max-w-2xl leading-relaxed">
              Manage your end-to-end influencer campaigns, creator roster boards, deliverable reviews, and client reports in one unified agency platform.
            </p>
          </div>

          <div class="flex items-center gap-3 shrink-0">
            <button
              (click)="loadOverview()"
              class="px-3.5 py-2.5 rounded-xl bg-white/10 hover:bg-white/15 border border-white/20 text-white text-xs font-semibold shadow-xs transition flex items-center gap-2"
              [disabled]="loading()"
            >
              <lucide-icon [img]="RefreshCwIcon" class="w-3.5 h-3.5" [class.animate-spin]="loading()"></lucide-icon>
              <span>Sync Data</span>
            </button>
            <a
              routerLink="/campaigns"
              class="px-4 py-2.5 rounded-xl bg-indigo-600 hover:bg-indigo-500 text-white text-xs font-semibold shadow-md shadow-indigo-600/30 transition flex items-center gap-2"
            >
              <lucide-icon [img]="MegaphoneIcon" class="w-4 h-4"></lucide-icon>
              <span>View Campaigns</span>
            </a>
          </div>
        </div>
      </div>

      <!-- Quick KPI Stat Cards -->
      <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-4 sm:gap-6">
        <!-- 1. Active Campaigns -->
        <div class="p-5 bg-white rounded-2xl border border-slate-200/80 shadow-xs hover:shadow-md transition space-y-3">
          <div class="flex items-center justify-between">
            <span class="text-xs font-medium text-slate-500 uppercase tracking-wider">Active Campaigns</span>
            <div class="w-9 h-9 rounded-xl bg-indigo-50 text-indigo-600 flex items-center justify-center">
              <lucide-icon [img]="MegaphoneIcon" class="w-4 h-4"></lucide-icon>
            </div>
          </div>
          <div>
            @if (loading()) {
              <div class="h-8 w-16 bg-slate-100 animate-pulse rounded-lg"></div>
            } @else {
              <div class="flex items-baseline gap-2">
                <span class="text-2xl font-bold text-slate-900">{{ overview()?.activeCampaignsCount ?? 0 }}</span>
                <span class="text-xs text-slate-500 font-medium">/ {{ overview()?.totalCampaignsCount ?? 0 }} total</span>
              </div>
            }
          </div>
          <p class="text-[11px] text-slate-400">Across {{ overview()?.totalClientsCount ?? 0 }} brand clients</p>
        </div>

        <!-- 2. Creator CRM Roster -->
        <div class="p-5 bg-white rounded-2xl border border-slate-200/80 shadow-xs hover:shadow-md transition space-y-3">
          <div class="flex items-center justify-between">
            <span class="text-xs font-medium text-slate-500 uppercase tracking-wider">Creator CRM Roster</span>
            <div class="w-9 h-9 rounded-xl bg-violet-50 text-violet-600 flex items-center justify-center">
              <lucide-icon [img]="UsersIcon" class="w-4 h-4"></lucide-icon>
            </div>
          </div>
          <div>
            @if (loading()) {
              <div class="h-8 w-16 bg-slate-100 animate-pulse rounded-lg"></div>
            } @else {
              <span class="text-2xl font-bold text-slate-900">{{ overview()?.totalCreatorsCount ?? 0 }}</span>
              <span class="text-xs text-indigo-600 font-medium ml-2">Enrolled Creators</span>
            }
          </div>
          <p class="text-[11px] text-slate-400">TikTok, Instagram, YouTube</p>
        </div>

        <!-- 3. Pending Reviews -->
        <div class="p-5 bg-white rounded-2xl border border-slate-200/80 shadow-xs hover:shadow-md transition space-y-3">
          <div class="flex items-center justify-between">
            <span class="text-xs font-medium text-slate-500 uppercase tracking-wider">Pending Reviews</span>
            <div class="w-9 h-9 rounded-xl bg-amber-50 text-amber-600 flex items-center justify-center">
              <lucide-icon [img]="ClockIcon" class="w-4 h-4"></lucide-icon>
            </div>
          </div>
          <div>
            @if (loading()) {
              <div class="h-8 w-16 bg-slate-100 animate-pulse rounded-lg"></div>
            } @else {
              <span class="text-2xl font-bold text-slate-900">{{ overview()?.pendingReviewsCount ?? 0 }}</span>
              <span class="text-xs text-amber-600 font-medium ml-2">Needs approval</span>
            }
          </div>
          <p class="text-[11px] text-slate-400">{{ overview()?.completedDeliverablesCount ?? 0 }} deliverables approved</p>
        </div>

        <!-- 4. Reports Ready -->
        <div class="p-5 bg-white rounded-2xl border border-slate-200/80 shadow-xs hover:shadow-md transition space-y-3">
          <div class="flex items-center justify-between">
            <span class="text-xs font-medium text-slate-500 uppercase tracking-wider">Reports Ready</span>
            <div class="w-9 h-9 rounded-xl bg-emerald-50 text-emerald-600 flex items-center justify-center">
              <lucide-icon [img]="BarChart3Icon" class="w-4 h-4"></lucide-icon>
            </div>
          </div>
          <div>
            @if (loading()) {
              <div class="h-8 w-16 bg-slate-100 animate-pulse rounded-lg"></div>
            } @else {
              <span class="text-2xl font-bold text-slate-900">{{ overview()?.totalReportsCount ?? 0 }}</span>
              <span class="text-xs text-emerald-600 font-medium ml-2">Generated PDF</span>
            }
          </div>
          <p class="text-[11px] text-slate-400">jsreport + MinIO storage</p>
        </div>
      </div>

      <!-- Action Items & Recent Campaigns Grid -->
      <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
        <!-- Urgent Action Items (Col 1) -->
        <div class="lg:col-span-1 p-6 bg-white rounded-2xl border border-slate-200/80 shadow-xs space-y-4">
          <div class="flex items-center justify-between">
            <div>
              <h2 class="text-base font-bold text-slate-900">Urgent Deliverables</h2>
              <p class="text-xs text-slate-500 mt-0.5">Tasks needing immediate agency review</p>
            </div>
            <a routerLink="/deliverables" class="text-xs text-indigo-600 hover:text-indigo-700 font-semibold flex items-center gap-1">
              <span>View all</span>
              <lucide-icon [img]="ChevronRightIcon" class="w-3.5 h-3.5"></lucide-icon>
            </a>
          </div>

          @if (loading()) {
            <div class="space-y-3">
              @for (i of [1, 2, 3]; track i) {
                <div class="p-3 rounded-xl bg-slate-50 border border-slate-100 animate-pulse space-y-2">
                  <div class="h-4 bg-slate-200 rounded-md w-3/4"></div>
                  <div class="h-3 bg-slate-200 rounded-md w-1/2"></div>
                </div>
              }
            </div>
          } @else if ((overview()?.actionItems?.length ?? 0) === 0) {
            <div class="text-center py-8 px-4 rounded-xl bg-slate-50 border border-dashed border-slate-200 space-y-2">
              <lucide-icon [img]="CheckCircle2Icon" class="w-8 h-8 text-emerald-500 mx-auto"></lucide-icon>
              <p class="text-xs font-semibold text-slate-700">All deliverables up to date!</p>
              <p class="text-[11px] text-slate-400">No pending submissions requiring immediate approval.</p>
            </div>
          } @else {
            <div class="space-y-3">
              @for (item of overview()?.actionItems; track item.deliverableId) {
                <a
                  routerLink="/deliverables"
                  class="block p-3.5 rounded-xl bg-slate-50/80 hover:bg-indigo-50/60 border border-slate-200/60 hover:border-indigo-200 transition group"
                >
                  <div class="flex items-start justify-between gap-2">
                    <div class="space-y-1">
                      <p class="text-xs font-bold text-slate-800 group-hover:text-indigo-700 transition">
                        {{ item.title }}
                      </p>
                      <div class="flex items-center gap-2 text-[11px] text-slate-500">
                        <span class="px-1.5 py-0.5 rounded bg-slate-200/60 text-slate-700 font-medium text-[10px]">{{ item.platform }}</span>
                        <span>Due: {{ item.dueDate }}</span>
                      </div>
                    </div>
                    <span
                      class="px-2 py-0.5 rounded-full text-[10px] font-semibold shrink-0"
                      [ngClass]="{
                        'bg-amber-100 text-amber-700': item.status === 'Submitted' || item.status === 'DraftSubmitted',
                        'bg-rose-100 text-rose-700': item.status === 'Revision',
                        'bg-slate-100 text-slate-600': item.status === 'Pending'
                      }"
                    >
                      {{ item.status }}
                    </span>
                  </div>
                </a>
              }
            </div>
          }
        </div>

        <!-- Recent Active Campaigns (Col 2-3) -->
        <div class="lg:col-span-2 p-6 bg-white rounded-2xl border border-slate-200/80 shadow-xs space-y-4">
          <div class="flex items-center justify-between">
            <div>
              <h2 class="text-base font-bold text-slate-900">Active Campaigns</h2>
              <p class="text-xs text-slate-500 mt-0.5">Real-time status of current client campaigns</p>
            </div>
            <a routerLink="/campaigns" class="text-xs text-indigo-600 hover:text-indigo-700 font-semibold flex items-center gap-1">
              <span>View all</span>
              <lucide-icon [img]="ChevronRightIcon" class="w-3.5 h-3.5"></lucide-icon>
            </a>
          </div>

          @if (loading()) {
            <div class="space-y-3">
              @for (i of [1, 2, 3]; track i) {
                <div class="p-4 rounded-xl bg-slate-50 border border-slate-100 animate-pulse space-y-2">
                  <div class="h-4 bg-slate-200 rounded-md w-1/3"></div>
                  <div class="h-3 bg-slate-200 rounded-md w-1/4"></div>
                </div>
              }
            </div>
          } @else if ((overview()?.recentCampaigns?.length ?? 0) === 0) {
            <div class="text-center py-10 px-4 rounded-xl bg-slate-50 border border-dashed border-slate-200 space-y-3">
              <lucide-icon [img]="MegaphoneIcon" class="w-8 h-8 text-slate-400 mx-auto"></lucide-icon>
              <p class="text-xs font-semibold text-slate-700">No active campaigns found</p>
              <a
                routerLink="/campaigns"
                class="inline-flex items-center gap-1.5 px-3 py-1.5 rounded-lg bg-indigo-600 text-white text-xs font-semibold hover:bg-indigo-500 transition"
              >
                <span>Launch New Campaign</span>
                <lucide-icon [img]="ArrowRightIcon" class="w-3.5 h-3.5"></lucide-icon>
              </a>
            </div>
          } @else {
            <div class="overflow-x-auto">
              <table class="w-full text-left text-xs text-slate-600">
                <thead class="bg-slate-50 border-b border-slate-200/80 text-[11px] font-bold uppercase text-slate-500 tracking-wider">
                  <tr>
                    <th class="py-3 px-3">Campaign & Client</th>
                    <th class="py-3 px-3">Timeline</th>
                    <th class="py-3 px-3">Budget</th>
                    <th class="py-3 px-3">Roster</th>
                    <th class="py-3 px-3 text-right">Action</th>
                  </tr>
                </thead>
                <tbody class="divide-y divide-slate-100">
                  @for (c of overview()?.recentCampaigns; track c.id) {
                    <tr class="hover:bg-slate-50/80 transition">
                      <td class="py-3 px-3">
                        <div class="font-bold text-slate-900">{{ c.title }}</div>
                        <div class="text-[11px] text-indigo-600 font-medium">{{ c.clientName }}</div>
                      </td>
                      <td class="py-3 px-3 whitespace-nowrap text-[11px] text-slate-500">
                        {{ c.startDate }} → {{ c.endDate }}
                      </td>
                      <td class="py-3 px-3 font-semibold text-slate-800">
                        Rp {{ c.budget | number }}
                      </td>
                      <td class="py-3 px-3">
                        <span class="inline-flex items-center gap-1 px-2 py-0.5 rounded-full bg-violet-50 text-violet-700 text-[11px] font-semibold">
                          <lucide-icon [img]="UsersIcon" class="w-3 h-3"></lucide-icon>
                          <span>{{ c.creatorsCount }}</span>
                        </span>
                      </td>
                      <td class="py-3 px-3 text-right">
                        <a
                          [routerLink]="['/campaigns', c.id]"
                          class="px-2.5 py-1.5 rounded-lg bg-slate-100 hover:bg-indigo-600 hover:text-white text-slate-700 text-[11px] font-semibold transition inline-flex items-center gap-1"
                        >
                          <span>Manage</span>
                          <lucide-icon [img]="ChevronRightIcon" class="w-3 h-3"></lucide-icon>
                        </a>
                      </td>
                    </tr>
                  }
                </tbody>
              </table>
            </div>
          }
        </div>
      </div>

      <!-- Core Workflow Life Cycle -->
      <div class="p-6 bg-white rounded-2xl border border-slate-200/80 shadow-xs space-y-4">
        <div class="flex items-center justify-between">
          <div>
            <h2 class="text-base font-bold text-slate-900">Agency Campaign Lifecycle</h2>
            <p class="text-xs text-slate-500 mt-0.5">Automated workflow from client onboarding to PDF export</p>
          </div>
          <span class="px-3 py-1 rounded-full bg-emerald-100 text-emerald-700 text-xs font-semibold">
            Active System
          </span>
        </div>

        <div class="grid grid-cols-2 md:grid-cols-6 gap-3 pt-2">
          <a routerLink="/clients" class="p-3.5 rounded-xl bg-slate-50 hover:bg-indigo-50/50 border border-slate-200/60 hover:border-indigo-200 text-center space-y-1.5 transition">
            <div class="w-7 h-7 rounded-lg bg-indigo-100 text-indigo-700 flex items-center justify-center mx-auto text-xs font-bold">1</div>
            <p class="text-xs font-semibold text-slate-800">Client</p>
            <p class="text-[10px] text-slate-400">Brand & PICs</p>
          </a>

          <a routerLink="/campaigns" class="p-3.5 rounded-xl bg-slate-50 hover:bg-indigo-50/50 border border-slate-200/60 hover:border-indigo-200 text-center space-y-1.5 transition">
            <div class="w-7 h-7 rounded-lg bg-indigo-100 text-indigo-700 flex items-center justify-center mx-auto text-xs font-bold">2</div>
            <p class="text-xs font-semibold text-slate-800">Campaign</p>
            <p class="text-[10px] text-slate-400">Brief & Budget</p>
          </a>

          <a routerLink="/creators" class="p-3.5 rounded-xl bg-slate-50 hover:bg-indigo-50/50 border border-slate-200/60 hover:border-indigo-200 text-center space-y-1.5 transition">
            <div class="w-7 h-7 rounded-lg bg-indigo-100 text-indigo-700 flex items-center justify-center mx-auto text-xs font-bold">3</div>
            <p class="text-xs font-semibold text-slate-800">Roster</p>
            <p class="text-[10px] text-slate-400">Kanban Board</p>
          </a>

          <a routerLink="/deliverables" class="p-3.5 rounded-xl bg-slate-50 hover:bg-indigo-50/50 border border-slate-200/60 hover:border-indigo-200 text-center space-y-1.5 transition">
            <div class="w-7 h-7 rounded-lg bg-indigo-100 text-indigo-700 flex items-center justify-center mx-auto text-xs font-bold">4</div>
            <p class="text-xs font-semibold text-slate-800">Deliverable</p>
            <p class="text-[10px] text-slate-400">Submission</p>
          </a>

          <a routerLink="/deliverables" class="p-3.5 rounded-xl bg-slate-50 hover:bg-indigo-50/50 border border-slate-200/60 hover:border-indigo-200 text-center space-y-1.5 transition">
            <div class="w-7 h-7 rounded-lg bg-indigo-100 text-indigo-700 flex items-center justify-center mx-auto text-xs font-bold">5</div>
            <p class="text-xs font-semibold text-slate-800">Approval</p>
            <p class="text-[10px] text-slate-400">Review & Revision</p>
          </a>

          <a routerLink="/reports" class="p-3.5 rounded-xl bg-slate-50 hover:bg-indigo-50/50 border border-slate-200/60 hover:border-indigo-200 text-center space-y-1.5 transition">
            <div class="w-7 h-7 rounded-lg bg-indigo-100 text-indigo-700 flex items-center justify-center mx-auto text-xs font-bold">6</div>
            <p class="text-xs font-semibold text-slate-800">Reporting</p>
            <p class="text-[10px] text-slate-400">PDF Generation</p>
          </a>
        </div>
      </div>
    </div>
  `
})
export class DashboardComponent implements OnInit {
  readonly authService = inject(AuthService);
  private readonly dashboardService = inject(DashboardService);

  readonly overview = signal<DashboardOverview | null>(null);
  readonly loading = signal<boolean>(true);

  readonly SparklesIcon = Sparkles;
  readonly MegaphoneIcon = Megaphone;
  readonly UsersIcon = Users;
  readonly FolderKanbanIcon = FolderKanban;
  readonly BarChart3Icon = BarChart3;
  readonly ClockIcon = Clock;
  readonly CheckCircle2Icon = CheckCircle2;
  readonly Building2Icon = Building2;
  readonly ShieldCheckIcon = ShieldCheck;
  readonly BellIcon = Bell;
  readonly ArrowRightIcon = ArrowRight;
  readonly ChevronRightIcon = ChevronRight;
  readonly RefreshCwIcon = RefreshCw;

  ngOnInit(): void {
    this.loadOverview();
  }

  loadOverview(): void {
    this.loading.set(true);
    this.dashboardService.getOverview().subscribe({
      next: (res) => {
        if (res.success && res.data) {
          this.overview.set(res.data);
        }
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
      }
    });
  }
}
