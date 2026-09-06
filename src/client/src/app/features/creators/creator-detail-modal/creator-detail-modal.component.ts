import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  LucideAngularModule,
  X,
  Users,
  Mail,
  Phone,
  Tag,
  ExternalLink,
  Edit3,
  UserPlus,
  Calendar,
  Share2,
  CheckCircle2
} from 'lucide-angular';
import { Creator } from '../../../core/models/creator.model';

@Component({
  selector: 'app-creator-detail-modal',
  standalone: true,
  imports: [CommonModule, LucideAngularModule],
  template: `
    @if (isOpen && creator) {
      <div class="fixed inset-0 z-50 overflow-y-auto bg-slate-900/60 backdrop-blur-xs flex items-center justify-center p-4 sm:p-6 animate-in fade-in duration-200">
        <div
          class="bg-white rounded-3xl shadow-2xl border border-slate-100 w-full max-w-xl overflow-hidden transform transition-all"
          (click)="$event.stopPropagation()"
        >
          <!-- Header Hero Banner -->
          <div class="p-6 bg-gradient-to-r from-slate-900 via-indigo-950 to-slate-900 text-white relative">
            <button
              type="button"
              (click)="close.emit()"
              class="absolute top-5 right-5 w-8 h-8 rounded-full text-slate-400 hover:text-white hover:bg-white/10 flex items-center justify-center transition"
            >
              <lucide-icon [img]="XIcon" class="w-5 h-5"></lucide-icon>
            </button>

            <div class="flex items-start gap-4">
              <div class="w-14 h-14 rounded-2xl bg-indigo-500/20 border border-indigo-400/30 text-indigo-300 flex items-center justify-center font-black text-2xl shrink-0">
                {{ creator.fullName.charAt(0).toUpperCase() }}
              </div>
              <div class="space-y-1.5">
                <div class="flex items-center gap-2">
                  <span class="px-2.5 py-0.5 rounded-full bg-indigo-500/20 border border-indigo-400/30 text-indigo-200 text-xs font-semibold">
                    {{ creator.niche }}
                  </span>
                  <span
                    class="px-2 py-0.5 rounded-md text-[10px] font-bold uppercase tracking-wider"
                    [class]="creator.status === 'Active' ? 'bg-emerald-500/20 text-emerald-300' : 'bg-slate-700 text-slate-300'"
                  >
                    {{ creator.status }}
                  </span>
                </div>
                <h3 class="text-xl font-black text-white tracking-tight">{{ creator.fullName }}</h3>
              </div>
            </div>
          </div>

          <!-- Body Info -->
          <div class="p-6 space-y-6 max-h-[70vh] overflow-y-auto">
            <!-- Contact Information -->
            <div class="grid grid-cols-1 sm:grid-cols-2 gap-3">
              @if (creator.email) {
                <a
                  [href]="'mailto:' + creator.email"
                  class="p-3.5 rounded-2xl bg-slate-50 border border-slate-200/70 flex items-center gap-3 hover:border-indigo-300 hover:bg-indigo-50/30 transition text-xs text-slate-700"
                >
                  <div class="w-8 h-8 rounded-xl bg-white border border-slate-200 flex items-center justify-center text-slate-500 shrink-0">
                    <lucide-icon [img]="MailIcon" class="w-4 h-4"></lucide-icon>
                  </div>
                  <div class="truncate">
                    <span class="text-[10px] text-slate-400 block font-medium">Email Address</span>
                    <span class="font-bold truncate">{{ creator.email }}</span>
                  </div>
                </a>
              }

              @if (creator.phoneNumber) {
                <a
                  [href]="'tel:' + creator.phoneNumber"
                  class="p-3.5 rounded-2xl bg-slate-50 border border-slate-200/70 flex items-center gap-3 hover:border-indigo-300 hover:bg-indigo-50/30 transition text-xs text-slate-700"
                >
                  <div class="w-8 h-8 rounded-xl bg-white border border-slate-200 flex items-center justify-center text-slate-500 shrink-0">
                    <lucide-icon [img]="PhoneIcon" class="w-4 h-4"></lucide-icon>
                  </div>
                  <div>
                    <span class="text-[10px] text-slate-400 block font-medium">Phone / WhatsApp</span>
                    <span class="font-bold">{{ creator.phoneNumber }}</span>
                  </div>
                </a>
              }
            </div>

            <!-- Social Media Channels & Audience Reach -->
            <div class="space-y-3">
              <h4 class="text-xs font-bold text-slate-700 uppercase tracking-wider">
                Social Channels & Reach ({{ creator.socialAccounts.length }})
              </h4>

              @if (creator.socialAccounts.length === 0) {
                <div class="p-4 rounded-2xl bg-slate-50 border border-slate-200/60 text-center text-xs text-slate-400">
                  No social channels registered for this creator.
                </div>
              } @else {
                <div class="space-y-2.5">
                  @for (acc of creator.socialAccounts; track $index) {
                    <div class="p-4 rounded-2xl bg-slate-50 border border-slate-200/70 flex items-center justify-between gap-3 text-xs">
                      <div class="flex items-center gap-3">
                        <div class="w-9 h-9 rounded-xl bg-white border border-slate-200 font-bold flex items-center justify-center text-indigo-600 shrink-0">
                          {{ acc.platform.charAt(0) }}
                        </div>
                        <div>
                          <div class="flex items-center gap-1.5">
                            <span class="font-bold text-slate-900">{{ acc.handle }}</span>
                            <span class="text-[10px] text-slate-400">({{ acc.platform }})</span>
                          </div>
                          <span class="text-[11px] font-extrabold text-indigo-600">
                            {{ formatFollowers(acc.followerCount) }} followers
                          </span>
                        </div>
                      </div>

                      <a
                        [href]="acc.profileUrl"
                        target="_blank"
                        rel="noopener noreferrer"
                        class="p-2 rounded-xl text-slate-400 hover:text-indigo-600 hover:bg-white border border-transparent hover:border-slate-200 transition"
                        title="Open Social Profile"
                      >
                        <lucide-icon [img]="ExternalLinkIcon" class="w-4 h-4"></lucide-icon>
                      </a>
                    </div>
                  }
                </div>
              }
            </div>
          </div>

          <!-- Footer Actions -->
          <div class="p-4 bg-slate-50 border-t border-slate-100 flex items-center justify-between gap-3">
            <button
              type="button"
              (click)="close.emit()"
              class="px-4 py-2 rounded-xl border border-slate-200 text-slate-700 hover:bg-slate-100 text-xs font-semibold transition"
            >
              Close
            </button>

            <div class="flex items-center gap-2">
              <button
                type="button"
                (click)="edit.emit(creator)"
                class="px-3.5 py-2 rounded-xl border border-slate-200 text-slate-700 hover:bg-slate-100 text-xs font-semibold transition flex items-center gap-1.5"
              >
                <lucide-icon [img]="Edit3Icon" class="w-3.5 h-3.5"></lucide-icon>
                <span>Edit</span>
              </button>

              <button
                type="button"
                (click)="assignToRoster.emit(creator)"
                class="px-4 py-2 rounded-xl bg-indigo-600 hover:bg-indigo-500 text-white text-xs font-semibold shadow-xs transition flex items-center gap-1.5"
              >
                <lucide-icon [img]="UserPlusIcon" class="w-3.5 h-3.5"></lucide-icon>
                <span>Assign to Campaign</span>
              </button>
            </div>
          </div>
        </div>
      </div>
    }
  `
})
export class CreatorDetailModalComponent {
  @Input() isOpen = false;
  @Input() creator: Creator | null = null;
  @Output() close = new EventEmitter<void>();
  @Output() edit = new EventEmitter<Creator>();
  @Output() assignToRoster = new EventEmitter<Creator>();

  readonly XIcon = X;
  readonly UsersIcon = Users;
  readonly MailIcon = Mail;
  readonly PhoneIcon = Phone;
  readonly TagIcon = Tag;
  readonly ExternalLinkIcon = ExternalLink;
  readonly Edit3Icon = Edit3;
  readonly UserPlusIcon = UserPlus;
  readonly CalendarIcon = Calendar;
  readonly Share2Icon = Share2;
  readonly CheckCircle2Icon = CheckCircle2;

  formatFollowers(count: number): string {
    if (count >= 1_000_000) {
      return (count / 1_000_000).toFixed(1) + 'M';
    }
    if (count >= 1_000) {
      return (count / 1_000).toFixed(1) + 'K';
    }
    return count.toString();
  }
}
