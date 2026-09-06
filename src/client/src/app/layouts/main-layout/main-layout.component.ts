import { Component, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { NgxSonnerToaster } from 'ngx-sonner';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { TopbarComponent } from '../topbar/topbar.component';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [
    CommonModule,
    RouterOutlet,
    NgxSonnerToaster,
    SidebarComponent,
    TopbarComponent
  ],
  template: `
    <div class="min-h-screen bg-slate-50 flex">
      <!-- Desktop Sidebar (Sticky, Hidden on mobile) -->
      <div class="hidden lg:block shrink-0 sticky top-0 h-screen z-40">
        <app-sidebar></app-sidebar>
      </div>

      <!-- Mobile Sidebar Drawer Backdrop Overlay -->
      @if (isMobileSidebarOpen()) {
        <div
          (click)="closeSidebar()"
          class="fixed inset-0 bg-slate-950/60 backdrop-blur-xs z-50 lg:hidden animate-in fade-in duration-200"
        ></div>
      }

      <!-- Mobile Sidebar Drawer -->
      <div
        class="fixed inset-y-0 left-0 z-50 lg:hidden transform transition-transform duration-300 ease-in-out"
        [class.-translate-x-full]="!isMobileSidebarOpen()"
        [class.translate-x-0]="isMobileSidebarOpen()"
      >
        <app-sidebar (closeSidebar)="closeSidebar()"></app-sidebar>
      </div>

      <!-- Main Content Area -->
      <div class="flex-1 flex flex-col min-w-0">
        <app-topbar (toggleSidebar)="toggleSidebar()"></app-topbar>

        <main class="flex-1 p-4 sm:p-6 lg:p-8 max-w-7xl w-full mx-auto">
          <router-outlet></router-outlet>
        </main>
      </div>

      <!-- Global Sonner Toast Notifications Container -->
      <ngx-sonner-toaster position="top-right" richColors closeButton />
    </div>
  `
})
export class MainLayoutComponent {
  readonly isMobileSidebarOpen = signal<boolean>(false);

  toggleSidebar() {
    this.isMobileSidebarOpen.update(v => !v);
  }

  closeSidebar() {
    this.isMobileSidebarOpen.set(false);
  }
}
