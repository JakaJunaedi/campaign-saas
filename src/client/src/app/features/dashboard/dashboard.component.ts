import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
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
  Bell
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
        <div class="p-5 bg-white rounded-2xl border border-slate-200/80 shadow-xs hover:shadow-md transition space-y-3">
          <div class="flex items-center justify-between">
            <span class="text-xs font-medium text-slate-500 uppercase tracking-wider">Active Campaigns</span>
            <div class="w-9 h-9 rounded-xl bg-indigo-50 text-indigo-600 flex items-center justify-center">
              <lucide-icon [img]="MegaphoneIcon" class="w-4 h-4"></lucide-icon>
            </div>
          </div>
          <div>
            <span class="text-2xl font-bold text-slate-900">12</span>
            <span class="text-xs text-emerald-600 font-medium ml-2">+2 this month</span>
          </div>
          <p class="text-[11px] text-slate-400">Across 8 brand clients</p>
        </div>

        <div class="p-5 bg-white rounded-2xl border border-slate-200/80 shadow-xs hover:shadow-md transition space-y-3">
          <div class="flex items-center justify-between">
            <span class="text-xs font-medium text-slate-500 uppercase tracking-wider">Creator CRM Roster</span>
            <div class="w-9 h-9 rounded-xl bg-violet-50 text-violet-600 flex items-center justify-center">
              <lucide-icon [img]="UsersIcon" class="w-4 h-4"></lucide-icon>
            </div>
          </div>
          <div>
            <span class="text-2xl font-bold text-slate-900">84</span>
            <span class="text-xs text-indigo-600 font-medium ml-2">Creators active</span>
          </div>
          <p class="text-[11px] text-slate-400">TikTok, Instagram, YouTube</p>
        </div>

        <div class="p-5 bg-white rounded-2xl border border-slate-200/80 shadow-xs hover:shadow-md transition space-y-3">
          <div class="flex items-center justify-between">
            <span class="text-xs font-medium text-slate-500 uppercase tracking-wider">Pending Reviews</span>
            <div class="w-9 h-9 rounded-xl bg-amber-50 text-amber-600 flex items-center justify-center">
              <lucide-icon [img]="ClockIcon" class="w-4 h-4"></lucide-icon>
            </div>
          </div>
          <div>
            <span class="text-2xl font-bold text-slate-900">7</span>
            <span class="text-xs text-amber-600 font-medium ml-2">Needs approval</span>
          </div>
          <p class="text-[11px] text-slate-400">Submissions awaiting feedback</p>
        </div>

        <div class="p-5 bg-white rounded-2xl border border-slate-200/80 shadow-xs hover:shadow-md transition space-y-3">
          <div class="flex items-center justify-between">
            <span class="text-xs font-medium text-slate-500 uppercase tracking-wider">Reports Ready</span>
            <div class="w-9 h-9 rounded-xl bg-emerald-50 text-emerald-600 flex items-center justify-center">
              <lucide-icon [img]="BarChart3Icon" class="w-4 h-4"></lucide-icon>
            </div>
          </div>
          <div>
            <span class="text-2xl font-bold text-slate-900">100%</span>
            <span class="text-xs text-emerald-600 font-medium ml-2">Async PDF engine</span>
          </div>
          <p class="text-[11px] text-slate-400">jsreport + MinIO storage</p>
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
          <div class="p-3.5 rounded-xl bg-slate-50 border border-slate-200/60 text-center space-y-1.5">
            <div class="w-7 h-7 rounded-lg bg-indigo-100 text-indigo-700 flex items-center justify-center mx-auto text-xs font-bold">1</div>
            <p class="text-xs font-semibold text-slate-800">Client</p>
            <p class="text-[10px] text-slate-400">Brand & PICs</p>
          </div>

          <div class="p-3.5 rounded-xl bg-slate-50 border border-slate-200/60 text-center space-y-1.5">
            <div class="w-7 h-7 rounded-lg bg-indigo-100 text-indigo-700 flex items-center justify-center mx-auto text-xs font-bold">2</div>
            <p class="text-xs font-semibold text-slate-800">Campaign</p>
            <p class="text-[10px] text-slate-400">Brief & Budget</p>
          </div>

          <div class="p-3.5 rounded-xl bg-slate-50 border border-slate-200/60 text-center space-y-1.5">
            <div class="w-7 h-7 rounded-lg bg-indigo-100 text-indigo-700 flex items-center justify-center mx-auto text-xs font-bold">3</div>
            <p class="text-xs font-semibold text-slate-800">Roster</p>
            <p class="text-[10px] text-slate-400">Kanban Board</p>
          </div>

          <div class="p-3.5 rounded-xl bg-slate-50 border border-slate-200/60 text-center space-y-1.5">
            <div class="w-7 h-7 rounded-lg bg-indigo-100 text-indigo-700 flex items-center justify-center mx-auto text-xs font-bold">4</div>
            <p class="text-xs font-semibold text-slate-800">Deliverable</p>
            <p class="text-[10px] text-slate-400">Submission</p>
          </div>

          <div class="p-3.5 rounded-xl bg-slate-50 border border-slate-200/60 text-center space-y-1.5">
            <div class="w-7 h-7 rounded-lg bg-indigo-100 text-indigo-700 flex items-center justify-center mx-auto text-xs font-bold">5</div>
            <p class="text-xs font-semibold text-slate-800">Approval</p>
            <p class="text-[10px] text-slate-400">Review & Revision</p>
          </div>

          <div class="p-3.5 rounded-xl bg-slate-50 border border-slate-200/60 text-center space-y-1.5">
            <div class="w-7 h-7 rounded-lg bg-indigo-100 text-indigo-700 flex items-center justify-center mx-auto text-xs font-bold">6</div>
            <p class="text-xs font-semibold text-slate-800">Reporting</p>
            <p class="text-[10px] text-slate-400">PDF Generation</p>
          </div>
        </div>
      </div>
    </div>
  `
})
export class DashboardComponent {
  readonly authService = inject(AuthService);

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
}
