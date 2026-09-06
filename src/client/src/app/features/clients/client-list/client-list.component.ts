import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import {
  LucideAngularModule,
  Building2,
  Search,
  Plus,
  Edit,
  Trash2,
  Eye,
  Users,
  ChevronLeft,
  ChevronRight,
  AlertCircle,
  Loader2,
  RefreshCw
} from 'lucide-angular';
import { toast } from 'ngx-sonner';
import { ClientService } from '../../../core/services/client.service';
import { Client, ClientSummary, PagedResult } from '../../../core/models/client.model';
import { ClientModalComponent } from '../client-modal/client-modal.component';
import { ClientDetailModalComponent } from '../client-detail-modal/client-detail-modal.component';

@Component({
  selector: 'app-client-list',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    LucideAngularModule,
    ClientModalComponent,
    ClientDetailModalComponent
  ],
  template: `
    <div class="space-y-6">
      <!-- Page Header Banner -->
      <div class="flex flex-col sm:flex-row sm:items-center justify-between gap-4">
        <div>
          <div class="flex items-center gap-2">
            <h1 class="text-2xl font-black tracking-tight text-slate-900">Clients & Brands</h1>
            <span class="px-2.5 py-0.5 rounded-full bg-indigo-50 text-indigo-700 text-xs font-bold">
              {{ totalCount() }} Total
            </span>
          </div>
          <p class="text-xs text-slate-500 mt-1">
            Directory of client brand accounts and their Person-In-Charge (PIC) contacts.
          </p>
        </div>

        <div class="flex items-center gap-3">
          <button
            type="button"
            (click)="loadClients()"
            class="p-2.5 rounded-xl border border-slate-200 text-slate-600 hover:bg-slate-50 hover:text-slate-900 transition"
            title="Refresh list"
          >
            <lucide-icon [img]="RefreshCwIcon" class="w-4 h-4" [class.animate-spin]="isLoading()"></lucide-icon>
          </button>

          <button
            type="button"
            (click)="openCreateModal()"
            class="px-4 py-2.5 rounded-xl bg-indigo-600 hover:bg-indigo-500 text-white text-xs font-semibold shadow-md shadow-indigo-600/20 transition flex items-center gap-2"
          >
            <lucide-icon [img]="PlusIcon" class="w-4 h-4"></lucide-icon>
            <span>Add Client Brand</span>
          </button>
        </div>
      </div>

      <!-- Filter & Search Toolbar -->
      <div class="p-4 bg-white rounded-2xl border border-slate-200/80 shadow-xs flex flex-col sm:flex-row sm:items-center justify-between gap-3">
        <div class="relative flex-1 max-w-md">
          <lucide-icon
            [img]="SearchIcon"
            class="w-4 h-4 absolute left-3.5 top-1/2 -translate-y-1/2 text-slate-400"
          ></lucide-icon>
          <input
            type="text"
            [(ngModel)]="searchQuery"
            (ngModelChange)="onSearchChange()"
            placeholder="Search by brand or company name..."
            class="w-full pl-9.5 pr-4 py-2 bg-slate-50 border border-slate-200 rounded-xl text-xs focus:outline-hidden focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-600 transition"
          />
        </div>

        <div class="text-xs text-slate-500">
          Showing page {{ currentPage() }} of {{ totalPages() || 1 }}
        </div>
      </div>

      <!-- Clients Data Table -->
      <div class="bg-white rounded-2xl border border-slate-200/80 shadow-xs overflow-hidden">
        @if (isLoading()) {
          <div class="p-12 text-center space-y-3">
            <lucide-icon [img]="Loader2Icon" class="w-8 h-8 text-indigo-600 animate-spin mx-auto"></lucide-icon>
            <p class="text-xs text-slate-500 font-medium">Loading client directory...</p>
          </div>
        } @else if (clients().length === 0) {
          <div class="p-12 text-center space-y-3 max-w-sm mx-auto">
            <div class="w-12 h-12 rounded-2xl bg-indigo-50 text-indigo-600 flex items-center justify-center mx-auto">
              <lucide-icon [img]="Building2Icon" class="w-6 h-6"></lucide-icon>
            </div>
            <h3 class="text-sm font-bold text-slate-900">No Clients Found</h3>
            <p class="text-xs text-slate-500 leading-relaxed">
              {{ searchQuery ? 'No brand records match your search criteria.' : 'Start by onboarding your first brand client to manage campaigns.' }}
            </p>
            @if (!searchQuery) {
              <button
                type="button"
                (click)="openCreateModal()"
                class="mt-2 inline-flex items-center gap-1.5 px-4 py-2 rounded-xl bg-indigo-600 text-white text-xs font-semibold hover:bg-indigo-500 transition"
              >
                <lucide-icon [img]="PlusIcon" class="w-3.5 h-3.5"></lucide-icon>
                <span>Add Client</span>
              </button>
            }
          </div>
        } @else {
          <div class="overflow-x-auto">
            <table class="w-full text-left border-collapse">
              <thead>
                <tr class="border-b border-slate-100 bg-slate-50/50 text-[11px] font-bold text-slate-500 uppercase tracking-wider">
                  <th class="py-3.5 px-6">Brand Name</th>
                  <th class="py-3.5 px-6">Legal Entity</th>
                  <th class="py-3.5 px-6">PIC Contacts</th>
                  <th class="py-3.5 px-6">Added Date</th>
                  <th class="py-3.5 px-6 text-right">Actions</th>
                </tr>
              </thead>
              <tbody class="divide-y divide-slate-100 text-xs">
                @for (client of clients(); track client.id) {
                  <tr class="hover:bg-slate-50/75 transition group">
                    <!-- Brand Name -->
                    <td class="py-4 px-6">
                      <div class="flex items-center gap-3">
                        <div class="w-9 h-9 rounded-xl bg-indigo-50 text-indigo-600 font-bold flex items-center justify-center shrink-0 border border-indigo-100">
                          {{ client.name.charAt(0).toUpperCase() }}
                        </div>
                        <div>
                          <p class="font-bold text-slate-900 group-hover:text-indigo-600 transition">
                            {{ client.name }}
                          </p>
                        </div>
                      </div>
                    </td>

                    <!-- Company Name -->
                    <td class="py-4 px-6 text-slate-600">
                      {{ client.companyName || '—' }}
                    </td>

                    <!-- PIC Contacts Count -->
                    <td class="py-4 px-6">
                      <span class="inline-flex items-center gap-1.5 px-2.5 py-1 rounded-full bg-slate-100 text-slate-700 font-medium text-[11px]">
                        <lucide-icon [img]="UsersIcon" class="w-3 h-3 text-slate-500"></lucide-icon>
                        <span>{{ client.contactsCount }} {{ client.contactsCount === 1 ? 'contact' : 'contacts' }}</span>
                      </span>
                    </td>

                    <!-- Added Date -->
                    <td class="py-4 px-6 text-slate-500">
                      {{ client.createdAt | date:'mediumDate' }}
                    </td>

                    <!-- Actions -->
                    <td class="py-4 px-6 text-right">
                      <div class="flex items-center justify-end gap-1.5">
                        <button
                          type="button"
                          (click)="viewClientDetail(client.id)"
                          class="p-2 rounded-lg text-slate-400 hover:text-indigo-600 hover:bg-indigo-50 transition"
                          title="View Details"
                        >
                          <lucide-icon [img]="EyeIcon" class="w-4 h-4"></lucide-icon>
                        </button>

                        <button
                          type="button"
                          (click)="openEditModal(client.id)"
                          class="p-2 rounded-lg text-slate-400 hover:text-indigo-600 hover:bg-indigo-50 transition"
                          title="Edit Client"
                        >
                          <lucide-icon [img]="EditIcon" class="w-4 h-4"></lucide-icon>
                        </button>

                        <button
                          type="button"
                          (click)="confirmDelete(client)"
                          class="p-2 rounded-lg text-slate-400 hover:text-rose-600 hover:bg-rose-50 transition"
                          title="Delete Client"
                        >
                          <lucide-icon [img]="Trash2Icon" class="w-4 h-4"></lucide-icon>
                        </button>
                      </div>
                    </td>
                  </tr>
                }
              </tbody>
            </table>
          </div>

          <!-- Pagination Footer -->
          <div class="px-6 py-4 border-t border-slate-100 flex items-center justify-between">
            <span class="text-xs text-slate-500">
              Showing {{ (currentPage() - 1) * pageSize + 1 }} to {{ Math.min(currentPage() * pageSize, totalCount()) }} of {{ totalCount() }} clients
            </span>

            <div class="flex items-center gap-2">
              <button
                type="button"
                [disabled]="currentPage() <= 1"
                (click)="goToPage(currentPage() - 1)"
                class="p-2 rounded-xl border border-slate-200 text-slate-600 hover:bg-slate-50 disabled:opacity-40 disabled:cursor-not-allowed transition"
              >
                <lucide-icon [img]="ChevronLeftIcon" class="w-4 h-4"></lucide-icon>
              </button>

              <span class="text-xs font-semibold text-slate-700 px-2">
                {{ currentPage() }} / {{ totalPages() || 1 }}
              </span>

              <button
                type="button"
                [disabled]="currentPage() >= totalPages()"
                (click)="goToPage(currentPage() + 1)"
                class="p-2 rounded-xl border border-slate-200 text-slate-600 hover:bg-slate-50 disabled:opacity-40 disabled:cursor-not-allowed transition"
              >
                <lucide-icon [img]="ChevronRightIcon" class="w-4 h-4"></lucide-icon>
              </button>
            </div>
          </div>
        }
      </div>

      <!-- Delete Confirmation Modal -->
      @if (clientToDelete()) {
        <div class="fixed inset-0 z-50 overflow-y-auto bg-slate-900/60 backdrop-blur-xs flex items-center justify-center p-4 animate-in fade-in duration-200">
          <div
            class="bg-white rounded-3xl shadow-2xl border border-slate-100 w-full max-w-md p-6 space-y-5"
            (click)="$event.stopPropagation()"
          >
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-2xl bg-rose-50 text-rose-600 flex items-center justify-center shrink-0">
                <lucide-icon [img]="AlertCircleIcon" class="w-5 h-5"></lucide-icon>
              </div>
              <div>
                <h3 class="text-base font-bold text-slate-900">Delete Client Brand</h3>
                <p class="text-xs text-slate-500">This action cannot be undone.</p>
              </div>
            </div>

            <p class="text-xs text-slate-600 leading-relaxed">
              Are you sure you want to delete <span class="font-bold text-slate-900">"{{ clientToDelete()?.name }}"</span>?
              All associated PIC contact records will also be removed.
            </p>

            <div class="flex items-center justify-end gap-3 pt-2">
              <button
                type="button"
                (click)="clientToDelete.set(null)"
                [disabled]="isDeleting()"
                class="px-4 py-2 rounded-xl border border-slate-200 text-slate-700 text-xs font-semibold hover:bg-slate-50 transition"
              >
                Cancel
              </button>

              <button
                type="button"
                (click)="executeDelete()"
                [disabled]="isDeleting()"
                class="px-4 py-2 rounded-xl bg-rose-600 hover:bg-rose-500 text-white text-xs font-semibold shadow-xs transition flex items-center gap-2"
              >
                @if (isDeleting()) {
                  <lucide-icon [img]="Loader2Icon" class="w-3.5 h-3.5 animate-spin"></lucide-icon>
                  <span>Deleting...</span>
                } @else {
                  <lucide-icon [img]="Trash2Icon" class="w-3.5 h-3.5"></lucide-icon>
                  <span>Confirm Delete</span>
                }
              </button>
            </div>
          </div>
        </div>
      }

      <!-- Create & Edit Modal -->
      <app-client-modal
        [isOpen]="isModalOpen()"
        [client]="selectedClientForEdit()"
        (close)="closeModal()"
        (saved)="onClientSaved()"
      ></app-client-modal>

      <!-- View Detail Modal -->
      <app-client-detail-modal
        [isOpen]="isDetailModalOpen()"
        [client]="selectedClientForDetail()"
        (close)="closeDetailModal()"
        (edit)="onEditFromDetail($event)"
      ></app-client-detail-modal>
    </div>
  `
})
export class ClientListComponent implements OnInit {
  private clientService = inject(ClientService);

  readonly Math = Math;

  // Signals
  readonly clients = signal<ClientSummary[]>([]);
  readonly totalCount = signal<number>(0);
  readonly totalPages = signal<number>(1);
  readonly currentPage = signal<number>(1);
  readonly pageSize = 10;
  readonly isLoading = signal<boolean>(false);
  readonly isDeleting = signal<boolean>(false);

  searchQuery = '';
  private searchTimeout: any;

  // Modal State Signals
  readonly isModalOpen = signal<boolean>(false);
  readonly isDetailModalOpen = signal<boolean>(false);
  readonly selectedClientForEdit = signal<Client | null>(null);
  readonly selectedClientForDetail = signal<Client | null>(null);
  readonly clientToDelete = signal<ClientSummary | null>(null);

  // Icons
  readonly Building2Icon = Building2;
  readonly SearchIcon = Search;
  readonly PlusIcon = Plus;
  readonly EditIcon = Edit;
  readonly Trash2Icon = Trash2;
  readonly EyeIcon = Eye;
  readonly UsersIcon = Users;
  readonly ChevronLeftIcon = ChevronLeft;
  readonly ChevronRightIcon = ChevronRight;
  readonly AlertCircleIcon = AlertCircle;
  readonly Loader2Icon = Loader2;
  readonly RefreshCwIcon = RefreshCw;

  ngOnInit(): void {
    this.loadClients();
  }

  loadClients(): void {
    this.isLoading.set(true);
    this.clientService
      .getClients(this.searchQuery, this.currentPage(), this.pageSize)
      .subscribe({
        next: res => {
          this.isLoading.set(false);
          if (res.success && res.data) {
            this.clients.set(res.data.items);
            this.totalCount.set(res.data.totalCount);
            this.totalPages.set(res.data.totalPages);
          }
        },
        error: err => {
          this.isLoading.set(false);
          toast.error('Failed to load client directory');
        }
      });
  }

  onSearchChange(): void {
    clearTimeout(this.searchTimeout);
    this.searchTimeout = setTimeout(() => {
      this.currentPage.set(1);
      this.loadClients();
    }, 300);
  }

  goToPage(page: number): void {
    if (page >= 1 && page <= this.totalPages()) {
      this.currentPage.set(page);
      this.loadClients();
    }
  }

  openCreateModal(): void {
    this.selectedClientForEdit.set(null);
    this.isModalOpen.set(true);
  }

  openEditModal(clientId: string): void {
    this.clientService.getClientById(clientId).subscribe({
      next: res => {
        if (res.success && res.data) {
          this.selectedClientForEdit.set(res.data);
          this.isModalOpen.set(true);
        }
      },
      error: () => toast.error('Failed to fetch client details for editing')
    });
  }

  viewClientDetail(clientId: string): void {
    this.clientService.getClientById(clientId).subscribe({
      next: res => {
        if (res.success && res.data) {
          this.selectedClientForDetail.set(res.data);
          this.isDetailModalOpen.set(true);
        }
      },
      error: () => toast.error('Failed to fetch client profile')
    });
  }

  closeModal(): void {
    this.isModalOpen.set(false);
    this.selectedClientForEdit.set(null);
  }

  closeDetailModal(): void {
    this.isDetailModalOpen.set(false);
    this.selectedClientForDetail.set(null);
  }

  onEditFromDetail(client: Client): void {
    this.closeDetailModal();
    this.selectedClientForEdit.set(client);
    this.isModalOpen.set(true);
  }

  onClientSaved(): void {
    this.loadClients();
  }

  confirmDelete(client: ClientSummary): void {
    this.clientToDelete.set(client);
  }

  executeDelete(): void {
    const target = this.clientToDelete();
    if (!target) return;

    this.isDeleting.set(true);
    this.clientService.deleteClient(target.id).subscribe({
      next: () => {
        this.isDeleting.set(false);
        this.clientToDelete.set(null);
        toast.success(`Client "${target.name}" deleted successfully`);
        this.loadClients();
      },
      error: err => {
        this.isDeleting.set(false);
        const msg = err.error?.detail || err.error?.message || 'Failed to delete client';
        toast.error(msg);
      }
    });
  }
}
