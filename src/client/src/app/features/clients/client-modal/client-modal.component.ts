import { Component, EventEmitter, Input, Output, inject, signal, effect } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, FormArray, Validators, ReactiveFormsModule } from '@angular/forms';
import { LucideAngularModule, X, Plus, Trash2, Building2, Mail, Phone, Briefcase, Loader2, Save } from 'lucide-angular';
import { toast } from 'ngx-sonner';
import { ClientService } from '../../../core/services/client.service';
import { Client, ClientContact, CreateClientRequest, UpdateClientRequest } from '../../../core/models/client.model';

@Component({
  selector: 'app-client-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, LucideAngularModule],
  template: `
    @if (isOpen) {
      <div class="fixed inset-0 z-50 overflow-y-auto bg-slate-900/60 backdrop-blur-xs flex items-center justify-center p-4 sm:p-6 animate-in fade-in duration-200">
        <div
          class="bg-white rounded-3xl shadow-2xl border border-slate-100 w-full max-w-2xl overflow-hidden transform transition-all"
          (click)="$event.stopPropagation()"
        >
          <!-- Modal Header -->
          <div class="px-6 py-5 border-b border-slate-100 flex items-center justify-between bg-slate-50/50">
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-2xl bg-indigo-50 text-indigo-600 flex items-center justify-center font-bold">
                <lucide-icon [img]="Building2Icon" class="w-5 h-5"></lucide-icon>
              </div>
              <div>
                <h3 class="text-lg font-bold text-slate-900">
                  {{ client ? 'Edit Client Profile' : 'Add New Client / Brand' }}
                </h3>
                <p class="text-xs text-slate-500">
                  {{ client ? 'Update brand details and PIC contact info.' : 'Register a new brand to manage campaigns.' }}
                </p>
              </div>
            </div>

            <button
              type="button"
              (click)="onCancel()"
              class="w-8 h-8 rounded-full text-slate-400 hover:text-slate-600 hover:bg-slate-100 flex items-center justify-center transition"
            >
              <lucide-icon [img]="XIcon" class="w-5 h-5"></lucide-icon>
            </button>
          </div>

          <!-- Modal Body & Form -->
          <form [formGroup]="clientForm" (ngSubmit)="onSubmit()" class="p-6 space-y-6 max-h-[75vh] overflow-y-auto">
            <!-- Brand & Company Info -->
            <div class="space-y-4">
              <h4 class="text-xs font-bold text-slate-700 uppercase tracking-wider">Brand Information</h4>
              <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <div class="space-y-1.5">
                  <label class="block text-xs font-semibold text-slate-700">
                    Brand Name <span class="text-rose-500">*</span>
                  </label>
                  <input
                    type="text"
                    formControlName="name"
                    placeholder="e.g. Tokopedia, Garnier, Samsung"
                    class="w-full px-3.5 py-2.5 rounded-xl border border-slate-200 text-sm focus:outline-hidden focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-600 transition"
                    [class.border-rose-300]="isFieldInvalid('name')"
                  />
                  @if (isFieldInvalid('name')) {
                    <p class="text-[11px] text-rose-500 font-medium">Brand name is required.</p>
                  }
                </div>

                <div class="space-y-1.5">
                  <label class="block text-xs font-semibold text-slate-700">Company Legal Name</label>
                  <input
                    type="text"
                    formControlName="companyName"
                    placeholder="e.g. PT Tokopedia Indonesia"
                    class="w-full px-3.5 py-2.5 rounded-xl border border-slate-200 text-sm focus:outline-hidden focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-600 transition"
                  />
                </div>
              </div>
            </div>

            <!-- Contacts FormArray -->
            <div class="space-y-4 pt-2 border-t border-slate-100">
              <div class="flex items-center justify-between">
                <div>
                  <h4 class="text-xs font-bold text-slate-700 uppercase tracking-wider">PIC Contacts (Person In Charge)</h4>
                  <p class="text-[11px] text-slate-400">Brand managers or marketing executives to contact.</p>
                </div>
                <button
                  type="button"
                  (click)="addContact()"
                  class="inline-flex items-center gap-1.5 px-3 py-1.5 rounded-xl bg-indigo-50 hover:bg-indigo-100 text-indigo-700 text-xs font-semibold transition"
                >
                  <lucide-icon [img]="PlusIcon" class="w-3.5 h-3.5"></lucide-icon>
                  <span>Add Contact</span>
                </button>
              </div>

              <div formArrayName="contacts" class="space-y-3">
                @for (contactGroup of contactsFormArray.controls; track $index) {
                  <div
                    [formGroupName]="$index"
                    class="p-4 rounded-2xl bg-slate-50 border border-slate-200/80 relative space-y-3"
                  >
                    <div class="flex items-center justify-between">
                      <span class="text-xs font-bold text-indigo-700">Contact #{{ $index + 1 }}</span>
                      @if (contactsFormArray.length > 1) {
                        <button
                          type="button"
                          (click)="removeContact($index)"
                          class="text-slate-400 hover:text-rose-600 p-1 rounded-lg transition"
                          title="Remove contact"
                        >
                          <lucide-icon [img]="Trash2Icon" class="w-4 h-4"></lucide-icon>
                        </button>
                      }
                    </div>

                    <div class="grid grid-cols-1 sm:grid-cols-2 gap-3">
                      <div class="space-y-1">
                        <label class="block text-[11px] font-semibold text-slate-600">Contact Name *</label>
                        <input
                          type="text"
                          formControlName="name"
                          placeholder="e.g. Sarah Jenkins"
                          class="w-full px-3 py-2 rounded-xl bg-white border border-slate-200 text-xs focus:outline-hidden focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-600"
                        />
                      </div>

                      <div class="space-y-1">
                        <label class="block text-[11px] font-semibold text-slate-600">Email Address *</label>
                        <input
                          type="email"
                          formControlName="email"
                          placeholder="e.g. sarah@brand.com"
                          class="w-full px-3 py-2 rounded-xl bg-white border border-slate-200 text-xs focus:outline-hidden focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-600"
                        />
                      </div>

                      <div class="space-y-1">
                        <label class="block text-[11px] font-semibold text-slate-600">Phone Number</label>
                        <input
                          type="text"
                          formControlName="phoneNumber"
                          placeholder="e.g. +62 812-3456-7890"
                          class="w-full px-3 py-2 rounded-xl bg-white border border-slate-200 text-xs focus:outline-hidden focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-600"
                        />
                      </div>

                      <div class="space-y-1">
                        <label class="block text-[11px] font-semibold text-slate-600">Role / Position</label>
                        <input
                          type="text"
                          formControlName="position"
                          placeholder="e.g. Senior Brand Manager"
                          class="w-full px-3 py-2 rounded-xl bg-white border border-slate-200 text-xs focus:outline-hidden focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-600"
                        />
                      </div>
                    </div>
                  </div>
                }
              </div>
            </div>

            <!-- Form Action Footer -->
            <div class="pt-4 border-t border-slate-100 flex items-center justify-end gap-3">
              <button
                type="button"
                (click)="onCancel()"
                [disabled]="isSubmitting()"
                class="px-4 py-2.5 rounded-xl border border-slate-200 text-slate-700 hover:bg-slate-50 text-xs font-semibold transition"
              >
                Cancel
              </button>

              <button
                type="submit"
                [disabled]="isSubmitting() || clientForm.invalid"
                class="px-5 py-2.5 rounded-xl bg-indigo-600 hover:bg-indigo-500 disabled:bg-indigo-300 text-white text-xs font-semibold shadow-md shadow-indigo-600/20 transition flex items-center gap-2"
              >
                @if (isSubmitting()) {
                  <lucide-icon [img]="Loader2Icon" class="w-4 h-4 animate-spin"></lucide-icon>
                  <span>Saving...</span>
                } @else {
                  <lucide-icon [img]="SaveIcon" class="w-4 h-4"></lucide-icon>
                  <span>{{ client ? 'Update Client' : 'Create Client' }}</span>
                }
              </button>
            </div>
          </form>
        </div>
      </div>
    }
  `
})
export class ClientModalComponent {
  @Input() isOpen = false;
  @Input() client: Client | null = null;
  @Output() close = new EventEmitter<void>();
  @Output() saved = new EventEmitter<Client>();

  private fb = inject(FormBuilder);
  private clientService = inject(ClientService);

  readonly isSubmitting = signal<boolean>(false);

  readonly XIcon = X;
  readonly PlusIcon = Plus;
  readonly Trash2Icon = Trash2;
  readonly Building2Icon = Building2;
  readonly MailIcon = Mail;
  readonly PhoneIcon = Phone;
  readonly BriefcaseIcon = Briefcase;
  readonly Loader2Icon = Loader2;
  readonly SaveIcon = Save;

  clientForm: FormGroup = this.fb.group({
    name: ['', [Validators.required, Validators.maxLength(200)]],
    companyName: ['', [Validators.maxLength(200)]],
    contacts: this.fb.array([])
  });

  get contactsFormArray(): FormArray {
    return this.clientForm.get('contacts') as FormArray;
  }

  constructor() {
    effect(() => {
      // Re-populate when client input changes
      if (this.isOpen) {
        this.populateForm();
      }
    });
  }

  private populateForm(): void {
    this.contactsFormArray.clear();

    if (this.client) {
      this.clientForm.patchValue({
        name: this.client.name,
        companyName: this.client.companyName || ''
      });

      if (this.client.contacts && this.client.contacts.length > 0) {
        for (const contact of this.client.contacts) {
          this.contactsFormArray.push(this.createContactGroup(contact));
        }
      } else {
        this.contactsFormArray.push(this.createContactGroup());
      }
    } else {
      this.clientForm.reset({
        name: '',
        companyName: ''
      });
      this.contactsFormArray.push(this.createContactGroup());
    }
  }

  createContactGroup(contact?: ClientContact): FormGroup {
    return this.fb.group({
      name: [contact?.name || '', [Validators.required]],
      email: [contact?.email || '', [Validators.required, Validators.email]],
      phoneNumber: [contact?.phoneNumber || ''],
      position: [contact?.position || '']
    });
  }

  addContact(): void {
    this.contactsFormArray.push(this.createContactGroup());
  }

  removeContact(index: number): void {
    if (this.contactsFormArray.length > 1) {
      this.contactsFormArray.removeAt(index);
    }
  }

  isFieldInvalid(fieldName: string): boolean {
    const field = this.clientForm.get(fieldName);
    return !!(field && field.invalid && (field.dirty || field.touched));
  }

  onCancel(): void {
    this.clientForm.reset();
    this.close.emit();
  }

  onSubmit(): void {
    if (this.clientForm.invalid) {
      this.clientForm.markAllAsTouched();
      return;
    }

    this.isSubmitting.set(true);
    const formVal = this.clientForm.value;

    const contactsPayload: ClientContact[] = (formVal.contacts || []).filter(
      (c: ClientContact) => c.name?.trim() && c.email?.trim()
    );

    if (this.client) {
      const updateReq: UpdateClientRequest = {
        name: formVal.name.trim(),
        companyName: formVal.companyName?.trim() || undefined,
        contacts: contactsPayload
      };

      this.clientService.updateClient(this.client.id, updateReq).subscribe({
        next: res => {
          this.isSubmitting.set(false);
          toast.success('Client updated successfully');
          this.saved.emit(res.data);
          this.onCancel();
        },
        error: err => {
          this.isSubmitting.set(false);
          const msg = err.error?.detail || err.error?.message || 'Failed to update client';
          toast.error(msg);
        }
      });
    } else {
      const createReq: CreateClientRequest = {
        name: formVal.name.trim(),
        companyName: formVal.companyName?.trim() || undefined,
        contacts: contactsPayload
      };

      this.clientService.createClient(createReq).subscribe({
        next: res => {
          this.isSubmitting.set(false);
          toast.success('Client registered successfully');
          this.saved.emit(res.data);
          this.onCancel();
        },
        error: err => {
          this.isSubmitting.set(false);
          const msg = err.error?.detail || err.error?.message || 'Failed to create client';
          toast.error(msg);
        }
      });
    }
  }
}
