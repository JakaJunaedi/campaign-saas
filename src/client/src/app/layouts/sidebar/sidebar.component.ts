import { Component, inject, output } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';
import {
  LucideAngularModule,
  LayoutDashboard,
  Building2,
  Megaphone,
  Users,
  FolderKanban,
  BarChart3,
  ShieldCheck,
  LogOut,
  X
} from 'lucide-angular';

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    RouterLinkActive,
    LucideAngularModule
  ],
  template: `
    <aside class="flex flex-col h-full bg-slate-900 text-slate-300 border-r border-slate-800 w-64 select-none">
      <!-- Logo & Brand Header -->
      <div class="h-16 flex items-center justify-between px-5 border-b border-slate-800">
        <div class="flex items-center gap-3">
          <div class="w-9 h-9 rounded-xl bg-gradient-to-tr from-indigo-500 to-violet-500 flex items-center justify-center text-white font-bold shadow-md shadow-indigo-500/20">
            CS
          </div>
          <div>
            <h1 class="font-bold text-base text-white tracking-tight leading-none">Campaign SaaS</h1>
            <span class="text-[11px] text-indigo-400 font-medium tracking-wide uppercase">Agency Platform</span>
          </div>
        </div>

        <!-- Mobile Close Button -->
        <button
          (click)="closeSidebar.emit()"
          class="lg:hidden p-1.5 rounded-lg text-slate-400 hover:text-white hover:bg-slate-800 transition"
          aria-label="Close sidebar"
        >
          <lucide-icon [img]="XIcon" class="w-5 h-5"></lucide-icon>
        </button>
      </div>

      <!-- Current Organization Info Badge -->
      <div class="p-3 mx-3 my-3 bg-slate-800/60 rounded-xl border border-slate-800 flex items-center gap-3">
        <div class="w-8 h-8 rounded-lg bg-indigo-950 border border-indigo-700/50 flex items-center justify-center text-indigo-300">
          <lucide-icon [img]="Building2Icon" class="w-4 h-4"></lucide-icon>
        </div>
        <div class="min-w-0 flex-1">
          <p class="text-xs font-semibold text-white truncate">{{ authService.orgName() }}</p>
          <p class="text-[10px] text-slate-400 truncate">Slug: {{ authService.currentOrganization()?.slug || 'workspace' }}</p>
        </div>
      </div>

      <!-- Navigation Links -->
      <nav class="flex-1 px-3 space-y-1 overflow-y-auto">
        <div class="px-3 py-1.5 text-[10px] font-semibold uppercase tracking-wider text-slate-500">Main Menu</div>

        <a
          routerLink="/dashboard"
          routerLinkActive="bg-indigo-600 text-white shadow-md shadow-indigo-600/20 font-semibold"
          [routerLinkActiveOptions]="{ exact: true }"
          (click)="closeSidebar.emit()"
          class="flex items-center gap-3 px-3 py-2.5 rounded-xl text-sm text-slate-300 hover:bg-slate-800 hover:text-white transition group"
        >
          <lucide-icon [img]="LayoutDashboardIcon" class="w-4 h-4 text-slate-400 group-hover:text-white transition"></lucide-icon>
          <span>Dashboard</span>
        </a>

        <a
          routerLink="/clients"
          routerLinkActive="bg-indigo-600 text-white shadow-md shadow-indigo-600/20 font-semibold"
          (click)="closeSidebar.emit()"
          class="flex items-center gap-3 px-3 py-2.5 rounded-xl text-sm text-slate-300 hover:bg-slate-800 hover:text-white transition group"
        >
          <lucide-icon [img]="Building2Icon" class="w-4 h-4 text-slate-400 group-hover:text-white transition"></lucide-icon>
          <span>Clients & Brands</span>
        </a>

        <a
          routerLink="/campaigns"
          routerLinkActive="bg-indigo-600 text-white shadow-md shadow-indigo-600/20 font-semibold"
          (click)="closeSidebar.emit()"
          class="flex items-center gap-3 px-3 py-2.5 rounded-xl text-sm text-slate-300 hover:bg-slate-800 hover:text-white transition group"
        >
          <lucide-icon [img]="MegaphoneIcon" class="w-4 h-4 text-slate-400 group-hover:text-white transition"></lucide-icon>
          <span>Campaigns</span>
        </a>

        <a
          routerLink="/creators"
          routerLinkActive="bg-indigo-600 text-white shadow-md shadow-indigo-600/20 font-semibold"
          (click)="closeSidebar.emit()"
          class="flex items-center gap-3 px-3 py-2.5 rounded-xl text-sm text-slate-300 hover:bg-slate-800 hover:text-white transition group"
        >
          <lucide-icon [img]="UsersIcon" class="w-4 h-4 text-slate-400 group-hover:text-white transition"></lucide-icon>
          <span>Creators CRM</span>
        </a>

        <a
          routerLink="/deliverables"
          routerLinkActive="bg-indigo-600 text-white shadow-md shadow-indigo-600/20 font-semibold"
          (click)="closeSidebar.emit()"
          class="flex items-center gap-3 px-3 py-2.5 rounded-xl text-sm text-slate-300 hover:bg-slate-800 hover:text-white transition group"
        >
          <lucide-icon [img]="FolderKanbanIcon" class="w-4 h-4 text-slate-400 group-hover:text-white transition"></lucide-icon>
          <span>Deliverables & Reviews</span>
        </a>

        <a
          routerLink="/reports"
          routerLinkActive="bg-indigo-600 text-white shadow-md shadow-indigo-600/20 font-semibold"
          (click)="closeSidebar.emit()"
          class="flex items-center gap-3 px-3 py-2.5 rounded-xl text-sm text-slate-300 hover:bg-slate-800 hover:text-white transition group"
        >
          <lucide-icon [img]="BarChart3Icon" class="w-4 h-4 text-slate-400 group-hover:text-white transition"></lucide-icon>
          <span>Reporting & PDF</span>
        </a>

        <div class="pt-4 px-3 py-1.5 text-[10px] font-semibold uppercase tracking-wider text-slate-500">Security & Governance</div>

        <a
          routerLink="/audit-logs"
          routerLinkActive="bg-indigo-600 text-white shadow-md shadow-indigo-600/20 font-semibold"
          (click)="closeSidebar.emit()"
          class="flex items-center gap-3 px-3 py-2.5 rounded-xl text-sm text-slate-300 hover:bg-slate-800 hover:text-white transition group"
        >
          <lucide-icon [img]="ShieldCheckIcon" class="w-4 h-4 text-slate-400 group-hover:text-white transition"></lucide-icon>
          <span>Audit Logs</span>
        </a>
      </nav>

      <!-- User Profile & Logout Bottom Section -->
      <div class="p-3 border-t border-slate-800">
        <div class="flex items-center justify-between p-2 rounded-xl bg-slate-800/40">
          <div class="flex items-center gap-2.5 min-w-0">
            <div class="w-8 h-8 rounded-full bg-gradient-to-tr from-violet-600 to-pink-600 flex items-center justify-center text-xs font-bold text-white uppercase shrink-0">
              {{ authService.userFullName().charAt(0) }}
            </div>
            <div class="min-w-0 flex-1">
              <p class="text-xs font-medium text-white truncate">{{ authService.userFullName() }}</p>
              <p class="text-[10px] text-slate-400 truncate">{{ authService.userRole() || 'Member' }}</p>
            </div>
          </div>

          <button
            (click)="authService.logout()"
            class="p-1.5 rounded-lg text-slate-400 hover:text-rose-400 hover:bg-rose-950/40 transition"
            title="Sign Out"
          >
            <lucide-icon [img]="LogOutIcon" class="w-4 h-4"></lucide-icon>
          </button>
        </div>
      </div>
    </aside>
  `
})
export class SidebarComponent {
  readonly authService = inject(AuthService);
  readonly closeSidebar = output<void>();

  // Lucide Icons
  readonly LayoutDashboardIcon = LayoutDashboard;
  readonly Building2Icon = Building2;
  readonly MegaphoneIcon = Megaphone;
  readonly UsersIcon = Users;
  readonly FolderKanbanIcon = FolderKanban;
  readonly BarChart3Icon = BarChart3;
  readonly ShieldCheckIcon = ShieldCheck;
  readonly LogOutIcon = LogOut;
  readonly XIcon = X;
}
