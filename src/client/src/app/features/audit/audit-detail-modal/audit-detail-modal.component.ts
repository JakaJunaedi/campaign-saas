import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import {
  LucideAngularModule,
  ShieldCheck,
  X,
  Code2,
  Calendar,
  User,
  Layers,
  Globe,
  Tag
} from 'lucide-angular';
import { AuditLog } from '../../../core/models/audit.model';

@Component({
  selector: 'app-audit-detail-modal',
  standalone: true,
  imports: [
    CommonModule,
    LucideAngularModule
  ],
  template: `
    @if (isOpen && log) {
      <div class="fixed inset-0 z-50 overflow-y-auto bg-slate-900/60 backdrop-blur-xs flex items-center justify-center p-4 animate-in fade-in duration-200">
        <div
          class="bg-white rounded-3xl shadow-2xl border border-slate-100 w-full max-w-2xl overflow-hidden animate-in zoom-in-95 duration-150"
          (click)="$event.stopPropagation()"
        >
          <!-- Header -->
          <div class="px-6 py-5 border-b border-slate-100 flex items-center justify-between bg-slate-50/50">
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-2xl bg-indigo-50 text-indigo-600 flex items-center justify-center font-bold">
                <lucide-icon [img]="ShieldCheckIcon" class="w-5 h-5"></lucide-icon>
              </div>
              <div>
                <h3 class="text-base font-bold text-slate-900">Audit Trail Event Detail</h3>
                <p class="text-xs text-slate-500">
                  Event ID #{{ log.id.substring(0, 8) }} • {{ log.module }}.{{ log.entityName }}
                </p>
              </div>
            </div>

            <button
              type="button"
              (click)="close.emit()"
              class="p-2 rounded-xl text-slate-400 hover:text-slate-600 hover:bg-slate-100 transition"
            >
              <lucide-icon [img]="XIcon" class="w-4 h-4"></lucide-icon>
            </button>
          </div>

          <!-- Content Body -->
          <div class="p-6 space-y-5">
            <!-- Metadata Grid -->
            <div class="grid grid-cols-2 sm:grid-cols-3 gap-3 p-4 rounded-2xl bg-slate-50 border border-slate-100 text-xs">
              <div>
                <span class="text-[10px] font-bold text-slate-400 uppercase tracking-wider block">Timestamp</span>
                <span class="font-semibold text-slate-800">{{ log.timestamp | date:'medium' }}</span>
              </div>

              <div>
                <span class="text-[10px] font-bold text-slate-400 uppercase tracking-wider block">Actor</span>
                <span class="font-semibold text-slate-800">{{ log.actorEmail || 'System Process' }}</span>
              </div>

              <div>
                <span class="text-[10px] font-bold text-slate-400 uppercase tracking-wider block">Action</span>
                <span
                  class="px-2 py-0.5 rounded-md text-[10px] font-bold uppercase tracking-wider inline-block"
                  [ngClass]="getActionBadgeClass(log.action)"
                >
                  {{ log.action }}
                </span>
              </div>

              <div>
                <span class="text-[10px] font-bold text-slate-400 uppercase tracking-wider block">Module & Entity</span>
                <span class="font-semibold text-slate-800">{{ log.module }} / {{ log.entityName }}</span>
              </div>

              <div>
                <span class="text-[10px] font-bold text-slate-400 uppercase tracking-wider block">Entity ID</span>
                <span class="font-mono text-slate-700 text-[11px] block truncate" [title]="log.entityId">
                  {{ log.entityId }}
                </span>
              </div>

              <div>
                <span class="text-[10px] font-bold text-slate-400 uppercase tracking-wider block">IP Address</span>
                <span class="font-mono text-slate-700 text-[11px]">{{ log.ipAddress || '127.0.0.1' }}</span>
              </div>
            </div>

            <!-- Changes / Payload JSON Viewer -->
            <div class="space-y-2">
              <div class="flex items-center justify-between">
                <label class="text-xs font-bold text-slate-700 uppercase tracking-wider flex items-center gap-1.5">
                  <lucide-icon [img]="Code2Icon" class="w-3.5 h-3.5 text-slate-400"></lucide-icon>
                  <span>Payload & Changes Diff (JSON)</span>
                </label>
                <span class="text-[10px] text-slate-400 font-mono">UTF-8 JSON</span>
              </div>

              <div class="p-4 rounded-2xl bg-slate-950 text-emerald-400 font-mono text-xs overflow-x-auto max-h-80 border border-slate-800 leading-relaxed shadow-inner">
                <pre class="whitespace-pre-wrap">{{ formatJson(log.changesJson) }}</pre>
              </div>
            </div>

            <!-- Close Action -->
            <div class="flex items-center justify-end pt-2 border-t border-slate-100">
              <button
                type="button"
                (click)="close.emit()"
                class="px-5 py-2.5 rounded-xl bg-slate-900 hover:bg-slate-800 text-white text-xs font-semibold transition"
              >
                Close Inspector
              </button>
            </div>
          </div>
        </div>
      </div>
    }
  `
})
export class AuditDetailModalComponent {
  @Input() isOpen = false;
  @Input() log: AuditLog | null = null;
  @Output() close = new EventEmitter<void>();

  // Icons
  readonly ShieldCheckIcon = ShieldCheck;
  readonly XIcon = X;
  readonly Code2Icon = Code2;
  readonly CalendarIcon = Calendar;
  readonly UserIcon = User;
  readonly LayersIcon = Layers;
  readonly GlobeIcon = Globe;
  readonly TagIcon = Tag;

  formatJson(rawJson?: string): string {
    if (!rawJson) return '// No payload recorded for this event.';
    try {
      const parsed = JSON.parse(rawJson);
      return JSON.stringify(parsed, null, 2);
    } catch {
      return rawJson;
    }
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
