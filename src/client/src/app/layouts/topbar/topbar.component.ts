import { Component, inject, signal, OnInit, HostListener, ElementRef, output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../core/services/auth.service';
import { NotificationService } from '../../core/services/notification.service';
import { InAppNotificationDto } from '../../core/models/notification.model';
import {
  LucideAngularModule,
  Menu,
  Bell,
  CheckCheck,
  Building2,
  ExternalLink,
  ChevronDown
} from 'lucide-angular';

@Component({
  selector: 'app-topbar',
  standalone: true,
  imports: [
    CommonModule,
    LucideAngularModule
  ],
  template: `
    <header class="h-16 bg-white border-b border-slate-200 px-4 sm:px-6 flex items-center justify-between sticky top-0 z-30 shadow-xs">
      <!-- Left: Mobile Menu Toggle & Title -->
      <div class="flex items-center gap-3">
        <button
          (click)="toggleSidebar.emit()"
          class="lg:hidden p-2 rounded-xl text-slate-600 hover:text-slate-900 hover:bg-slate-100 transition"
          aria-label="Open sidebar"
        >
          <lucide-icon [img]="MenuIcon" class="w-5 h-5"></lucide-icon>
        </button>

        <div class="hidden sm:flex items-center gap-2 px-3 py-1.5 bg-slate-100/80 rounded-full border border-slate-200">
          <lucide-icon [img]="Building2Icon" class="w-3.5 h-3.5 text-slate-500"></lucide-icon>
          <span class="text-xs font-semibold text-slate-700">{{ authService.orgName() }}</span>
          <span class="inline-block w-1.5 h-1.5 rounded-full bg-emerald-500"></span>
        </div>
      </div>

      <!-- Right: Action Buttons & Notifications & Profile -->
      <div class="flex items-center gap-3">
        <!-- Notification Dropdown Container -->
        <div class="relative notification-container">
          <button
            (click)="toggleNotifications()"
            class="relative p-2 rounded-xl text-slate-600 hover:text-indigo-600 hover:bg-indigo-50/60 transition"
            aria-label="Notifications"
          >
            <lucide-icon [img]="BellIcon" class="w-5 h-5"></lucide-icon>
            @if (notificationService.unreadCount() > 0) {
              <span class="absolute top-1 right-1 min-w-4 h-4 px-1 rounded-full bg-rose-500 text-white text-[10px] font-bold flex items-center justify-center ring-2 ring-white animate-pulse">
                {{ notificationService.unreadCount() > 9 ? '9+' : notificationService.unreadCount() }}
              </span>
            }
          </button>

          <!-- Notification Popover Menu -->
          @if (isNotificationsOpen()) {
            <div class="absolute right-0 mt-2 w-80 sm:w-96 bg-white rounded-2xl shadow-2xl border border-slate-200 z-50 overflow-hidden animate-in fade-in zoom-in-95 duration-150">
              <!-- Popover Header -->
              <div class="p-4 border-b border-slate-100 flex items-center justify-between bg-slate-50/70">
                <div class="flex items-center gap-2">
                  <h3 class="font-semibold text-sm text-slate-900">Notifications</h3>
                  @if (notificationService.unreadCount() > 0) {
                    <span class="px-2 py-0.5 rounded-full bg-indigo-100 text-indigo-700 text-xs font-semibold">
                      {{ notificationService.unreadCount() }} new
                    </span>
                  }
                </div>

                @if (notificationService.unreadCount() > 0) {
                  <button
                    (click)="markAllAsRead()"
                    class="text-xs font-medium text-indigo-600 hover:text-indigo-700 hover:underline flex items-center gap-1 cursor-pointer"
                  >
                    <lucide-icon [img]="CheckCheckIcon" class="w-3.5 h-3.5"></lucide-icon>
                    <span>Mark all read</span>
                  </button>
                }
              </div>

              <!-- Notifications List -->
              <div class="max-h-80 overflow-y-auto divide-y divide-slate-100">
                @if (notificationService.notifications().length === 0) {
                  <div class="p-8 text-center text-slate-400">
                    <lucide-icon [img]="BellIcon" class="w-8 h-8 mx-auto mb-2 opacity-40"></lucide-icon>
                    <p class="text-xs">No notifications yet</p>
                  </div>
                } @else {
                  @for (item of notificationService.notifications(); track item.id) {
                    <div
                      class="p-3.5 hover:bg-slate-50 transition flex items-start gap-3 cursor-pointer"
                      [ngClass]="{ 'bg-indigo-50/40': !item.isRead }"
                      (click)="onNotificationClick(item)"
                    >
                      <div class="mt-1">
                        @if (!item.isRead) {
                          <span class="block w-2 h-2 rounded-full bg-indigo-600"></span>
                        } @else {
                          <span class="block w-2 h-2 rounded-full bg-slate-300"></span>
                        }
                      </div>

                      <div class="flex-1 min-w-0">
                        <div class="flex items-center justify-between gap-1">
                          <p class="text-xs font-semibold text-slate-900 truncate">{{ item.title }}</p>
                          <span class="text-[10px] text-slate-400 shrink-0">{{ formatTime(item.createdAt) }}</span>
                        </div>
                        <p class="text-xs text-slate-600 mt-0.5 line-clamp-2 leading-relaxed">{{ item.message }}</p>

                        @if (item.linkUrl) {
                          <div class="mt-1.5 flex items-center gap-1 text-[11px] font-medium text-indigo-600">
                            <span>Open details</span>
                            <lucide-icon [img]="ExternalLinkIcon" class="w-3 h-3"></lucide-icon>
                          </div>
                        }
                      </div>
                    </div>
                  }
                }
              </div>
            </div>
          }
        </div>

        <div class="h-6 w-px bg-slate-200"></div>

        <!-- User Profile Pill -->
        <div class="flex items-center gap-2.5 pl-1">
          <div class="w-8 h-8 rounded-full bg-indigo-600 text-white flex items-center justify-center font-bold text-xs shadow-sm">
            {{ authService.userFullName().charAt(0) }}
          </div>
          <div class="hidden md:block text-left">
            <p class="text-xs font-semibold text-slate-800 leading-none">{{ authService.userFullName() }}</p>
            <p class="text-[10px] font-medium text-indigo-600 mt-0.5">{{ authService.userRole() }}</p>
          </div>
        </div>
      </div>
    </header>
  `
})
export class TopbarComponent implements OnInit {
  readonly authService = inject(AuthService);
  readonly notificationService = inject(NotificationService);
  private elementRef = inject(ElementRef);

  readonly toggleSidebar = output<void>();
  readonly isNotificationsOpen = signal<boolean>(false);

  // Lucide Icons
  readonly MenuIcon = Menu;
  readonly BellIcon = Bell;
  readonly CheckCheckIcon = CheckCheck;
  readonly Building2Icon = Building2;
  readonly ExternalLinkIcon = ExternalLink;
  readonly ChevronDownIcon = ChevronDown;

  ngOnInit() {
    if (this.authService.isAuthenticated()) {
      this.notificationService.loadUnreadCount().subscribe();
    }
  }

  toggleNotifications() {
    const newState = !this.isNotificationsOpen();
    this.isNotificationsOpen.set(newState);
    if (newState) {
      this.notificationService.loadNotifications(false, 15).subscribe();
    }
  }

  onNotificationClick(item: InAppNotificationDto) {
    if (!item.isRead) {
      this.notificationService.markAsRead(item.id).subscribe();
    }
  }

  markAllAsRead() {
    this.notificationService.markAllAsRead().subscribe();
  }

  formatTime(isoDate: string): string {
    if (!isoDate) return '';
    const date = new Date(isoDate);
    const now = new Date();
    const diffMs = now.getTime() - date.getTime();
    const diffMinutes = Math.floor(diffMs / 60000);

    if (diffMinutes < 1) return 'Just now';
    if (diffMinutes < 60) return `${diffMinutes}m ago`;
    const diffHours = Math.floor(diffMinutes / 60);
    if (diffHours < 24) return `${diffHours}h ago`;
    return date.toLocaleDateString();
  }

  @HostListener('document:click', ['$event'])
  onClickOutside(event: MouseEvent) {
    if (this.isNotificationsOpen() && !this.elementRef.nativeElement.querySelector('.notification-container')?.contains(event.target)) {
      this.isNotificationsOpen.set(false);
    }
  }
}
