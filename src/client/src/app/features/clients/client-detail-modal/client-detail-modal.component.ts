import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule, X, Building2, Mail, Phone, Briefcase, Calendar, Edit3 } from 'lucide-angular';
import { Client } from '../../../core/models/client.model';

@Component({
  selector: 'app-client-detail-modal',
  standalone: true,
  imports: [CommonModule, LucideAngularModule],
  template: `
    @if (isOpen && client) {
      <div class="fixed inset-0 z-50 overflow-y-auto bg-slate-900/60 backdrop-blur-xs flex items-center justify-center p-4 sm:p-6 animate-in fade-in duration-200">
        <div
          class="bg-white rounded-3xl shadow-2xl border border-slate-100 w-full max-w-xl overflow-hidden transform transition-all"
          (click)="$event.stopPropagation()"
        >
          <!-- Header Banner -->
          <div class="p-6 bg-gradient-to-r from-slate-900 to-indigo-950 text-white relative">
            <button
              type="button"
              (click)="close.emit()"
              class="absolute top-5 right-5 w-8 h-8 rounded-full text-slate-400 hover:text-white hover:bg-white/10 flex items-center justify-center transition"
            >
              <lucide-icon [img]="XIcon" class="w-5 h-5"></lucide-icon>
            </button>

            <div class="flex items-start gap-4">
              <div class="w-12 h-12 rounded-2xl bg-indigo-500/20 border border-indigo-400/30 text-indigo-300 flex items-center justify-center font-bold text-xl shrink-0">
                {{ client.name.charAt(0).toUpperCase() }}
              </div>
              <div class="space-y-1">
                <h3 class="text-xl font-extrabold tracking-tight text-white">{{ client.name }}</h3>
                @if (client.companyName) {
                  <p class="text-xs text-indigo-200/80 font-medium flex items-center gap-1.5">
                    <lucide-icon [img]="Building2Icon" class="w-3.5 h-3.5"></lucide-icon>
                    <span>{{ client.companyName }}</span>
                  </p>
                }
              </div>
            </div>
          </div>

          <!-- Body Info -->
          <div class="p-6 space-y-6 max-h-[70vh] overflow-y-auto">
            <!-- Registration Date -->
            <div class="flex items-center gap-2 text-xs text-slate-500">
              <lucide-icon [img]="CalendarIcon" class="w-4 h-4 text-slate-400"></lucide-icon>
              <span>Added to CRM on {{ client.createdAt | date:'mediumDate' }}</span>
            </div>

            <!-- PIC Contacts List -->
            <div class="space-y-3">
              <div class="flex items-center justify-between">
                <h4 class="text-xs font-bold text-slate-700 uppercase tracking-wider">
                  Assigned PIC Contacts ({{ client.contacts.length }})
                </h4>
              </div>

              @if (client.contacts.length === 0) {
                <div class="p-4 rounded-2xl bg-slate-50 border border-slate-200/60 text-center text-xs text-slate-400">
                  No PIC contacts recorded for this client.
                </div>
              } @else {
                <div class="grid grid-cols-1 gap-3">
                  @for (contact of client.contacts; track $index) {
                    <div class="p-4 rounded-2xl bg-slate-50 border border-slate-200/70 space-y-2">
                      <div class="flex items-center justify-between">
                        <span class="text-sm font-bold text-slate-900">{{ contact.name }}</span>
                        @if (contact.position) {
                          <span class="px-2.5 py-0.5 rounded-full bg-indigo-50 text-indigo-700 text-[11px] font-semibold flex items-center gap-1">
                            <lucide-icon [img]="BriefcaseIcon" class="w-3 h-3"></lucide-icon>
                            <span>{{ contact.position }}</span>
                          </span>
                        }
                      </div>

                      <div class="grid grid-cols-1 sm:grid-cols-2 gap-2 pt-1 text-xs text-slate-600">
                        <a
                          [href]="'mailto:' + contact.email"
                          class="flex items-center gap-1.5 hover:text-indigo-600 transition truncate"
                        >
                          <lucide-icon [img]="MailIcon" class="w-3.5 h-3.5 text-slate-400 shrink-0"></lucide-icon>
                          <span class="truncate">{{ contact.email }}</span>
                        </a>

                        @if (contact.phoneNumber) {
                          <a
                            [href]="'tel:' + contact.phoneNumber"
                            class="flex items-center gap-1.5 hover:text-indigo-600 transition truncate"
                          >
                            <lucide-icon [img]="PhoneIcon" class="w-3.5 h-3.5 text-slate-400 shrink-0"></lucide-icon>
                            <span>{{ contact.phoneNumber }}</span>
                          </a>
                        }
                      </div>
                    </div>
                  }
                </div>
              }
            </div>
          </div>

          <!-- Footer Actions -->
          <div class="p-4 bg-slate-50 border-t border-slate-100 flex items-center justify-end gap-3">
            <button
              type="button"
              (click)="close.emit()"
              class="px-4 py-2 rounded-xl border border-slate-200 text-slate-700 hover:bg-slate-100 text-xs font-semibold transition"
            >
              Close
            </button>
            <button
              type="button"
              (click)="edit.emit(client)"
              class="px-4 py-2 rounded-xl bg-indigo-600 hover:bg-indigo-500 text-white text-xs font-semibold shadow-xs transition flex items-center gap-1.5"
            >
              <lucide-icon [img]="Edit3Icon" class="w-3.5 h-3.5"></lucide-icon>
              <span>Edit Profile</span>
            </button>
          </div>
        </div>
      </div>
    }
  `
})
export class ClientDetailModalComponent {
  @Input() isOpen = false;
  @Input() client: Client | null = null;
  @Output() close = new EventEmitter<void>();
  @Output() edit = new EventEmitter<Client>();

  readonly XIcon = X;
  readonly Building2Icon = Building2;
  readonly MailIcon = Mail;
  readonly PhoneIcon = Phone;
  readonly BriefcaseIcon = Briefcase;
  readonly CalendarIcon = Calendar;
  readonly Edit3Icon = Edit3;
}
