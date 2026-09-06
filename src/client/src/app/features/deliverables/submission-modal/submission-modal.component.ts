import { Component, EventEmitter, Input, Output, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, Validators, ReactiveFormsModule } from '@angular/forms';
import {
  LucideAngularModule,
  X,
  UploadCloud,
  FileVideo,
  FileImage,
  File,
  CheckCircle2,
  Loader2,
  Save,
  AlertCircle
} from 'lucide-angular';
import { toast } from 'ngx-sonner';
import { DeliverableService } from '../../../core/services/deliverable.service';
import { Deliverable, ContentSubmission } from '../../../core/models/deliverable.model';

@Component({
  selector: 'app-submission-modal',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, LucideAngularModule],
  template: `
    @if (isOpen && deliverable) {
      <div class="fixed inset-0 z-50 overflow-y-auto bg-slate-900/60 backdrop-blur-xs flex items-center justify-center p-4 sm:p-6 animate-in fade-in duration-200">
        <div
          class="bg-white rounded-3xl shadow-2xl border border-slate-100 w-full max-w-xl overflow-hidden transform transition-all"
          (click)="$event.stopPropagation()"
        >
          <!-- Header -->
          <div class="px-6 py-5 border-b border-slate-100 flex items-center justify-between bg-slate-50/50">
            <div class="flex items-center gap-3">
              <div class="w-10 h-10 rounded-2xl bg-indigo-50 text-indigo-600 flex items-center justify-center font-bold">
                <lucide-icon [img]="UploadCloudIcon" class="w-5 h-5"></lucide-icon>
              </div>
              <div>
                <h3 class="text-lg font-bold text-slate-900">
                  Submit Content Draft (v{{ deliverable.latestVersion + 1 }})
                </h3>
                <p class="text-xs text-slate-500">
                  Upload video/image preview and copy draft for review.
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

          <!-- Form -->
          <form [formGroup]="submissionForm" (ngSubmit)="onSubmit()" class="p-6 space-y-4">
            <!-- Deliverable Info Card -->
            <div class="p-3.5 rounded-2xl bg-slate-50 border border-slate-200/80 text-xs flex items-center justify-between">
              <div>
                <span class="text-[10px] font-bold text-slate-400 uppercase tracking-wider block">Deliverable</span>
                <span class="font-bold text-slate-900">{{ deliverable.title }}</span>
              </div>
              <span class="px-2 py-0.5 rounded-md bg-white border border-slate-200 font-bold text-slate-600">
                {{ deliverable.platform }} • {{ deliverable.contentType }}
              </span>
            </div>

            <!-- Media File Upload Dropzone -->
            <div class="space-y-1.5">
              <label class="block text-xs font-semibold text-slate-700">
                Media File (MP4, MOV, PNG, JPG) <span class="text-rose-500">*</span>
              </label>

              @if (!selectedFile()) {
                <div
                  (click)="fileInput.click()"
                  class="border-2 border-dashed border-slate-300 hover:border-indigo-500 rounded-3xl p-6 text-center cursor-pointer bg-slate-50/50 hover:bg-indigo-50/30 transition group space-y-2"
                >
                  <input
                    #fileInput
                    type="file"
                    (change)="onFileSelected($event)"
                    accept="video/*,image/*"
                    class="hidden"
                  />
                  <div class="w-10 h-10 rounded-2xl bg-white border border-slate-200 text-slate-400 group-hover:text-indigo-600 flex items-center justify-center mx-auto transition">
                    <lucide-icon [img]="UploadCloudIcon" class="w-5 h-5"></lucide-icon>
                  </div>
                  <div>
                    <p class="text-xs font-bold text-slate-700 group-hover:text-indigo-600 transition">
                      Click to choose draft video or image
                    </p>
                    <p class="text-[11px] text-slate-400">Up to 250MB supported</p>
                  </div>
                </div>
              } @else {
                <div class="p-4 rounded-2xl bg-indigo-50/60 border border-indigo-200 flex items-center justify-between gap-3 text-xs">
                  <div class="flex items-center gap-3">
                    <div class="w-9 h-9 rounded-xl bg-white text-indigo-600 flex items-center justify-center font-bold shrink-0 shadow-2xs">
                      @if (selectedFile()?.type?.startsWith('video')) {
                        <lucide-icon [img]="FileVideoIcon" class="w-5 h-5"></lucide-icon>
                      } @else {
                        <lucide-icon [img]="FileImageIcon" class="w-5 h-5"></lucide-icon>
                      }
                    </div>
                    <div class="truncate max-w-[240px]">
                      <p class="font-bold text-slate-900 truncate">{{ selectedFile()?.name }}</p>
                      <p class="text-[11px] text-slate-500">{{ formatFileSize(selectedFile()?.size || 0) }}</p>
                    </div>
                  </div>

                  <button
                    type="button"
                    (click)="clearSelectedFile()"
                    class="text-slate-400 hover:text-rose-600 p-1 rounded-lg transition"
                    title="Change file"
                  >
                    <lucide-icon [img]="XIcon" class="w-4 h-4"></lucide-icon>
                  </button>
                </div>
              }
            </div>

            <!-- Caption / Script Draft -->
            <div class="space-y-1.5">
              <label class="block text-xs font-semibold text-slate-700">Caption / Script Text</label>
              <textarea
                formControlName="caption"
                rows="3"
                placeholder="Insert proposed social media caption text, tags, and hashtags..."
                class="w-full px-3.5 py-2.5 rounded-xl border border-slate-200 text-xs focus:outline-hidden focus:ring-2 focus:ring-indigo-500/20 focus:border-indigo-600 transition"
              ></textarea>
            </div>

            <!-- Upload Progress Indicator -->
            @if (isUploading()) {
              <div class="p-3.5 rounded-2xl bg-slate-900 text-white space-y-2">
                <div class="flex items-center justify-between text-xs">
                  <span class="font-medium text-slate-300">Uploading to MinIO storage...</span>
                  <span class="font-bold text-indigo-300">{{ uploadProgress() }}%</span>
                </div>
                <div class="w-full bg-slate-800 rounded-full h-1.5 overflow-hidden">
                  <div
                    class="bg-indigo-500 h-full transition-all duration-300 rounded-full"
                    [style.width.%]="uploadProgress()"
                  ></div>
                </div>
              </div>
            }

            <!-- Footer Actions -->
            <div class="pt-4 border-t border-slate-100 flex items-center justify-end gap-3">
              <button
                type="button"
                (click)="onCancel()"
                [disabled]="isSubmitting() || isUploading()"
                class="px-4 py-2.5 rounded-xl border border-slate-200 text-slate-700 hover:bg-slate-50 text-xs font-semibold transition"
              >
                Cancel
              </button>

              <button
                type="submit"
                [disabled]="!selectedFile() || isSubmitting() || isUploading()"
                class="px-5 py-2.5 rounded-xl bg-indigo-600 hover:bg-indigo-500 disabled:bg-slate-200 disabled:text-slate-400 text-white text-xs font-semibold shadow-md shadow-indigo-600/20 transition flex items-center gap-2"
              >
                @if (isSubmitting() || isUploading()) {
                  <lucide-icon [img]="Loader2Icon" class="w-4 h-4 animate-spin"></lucide-icon>
                  <span>Submitting...</span>
                } @else {
                  <lucide-icon [img]="SaveIcon" class="w-4 h-4"></lucide-icon>
                  <span>Submit Draft</span>
                }
              </button>
            </div>
          </form>
        </div>
      </div>
    }
  `
})
export class SubmissionModalComponent {
  @Input() isOpen = false;
  @Input() deliverable: Deliverable | null = null;
  @Output() close = new EventEmitter<void>();
  @Output() submitted = new EventEmitter<ContentSubmission>();

  private fb = inject(FormBuilder);
  private deliverableService = inject(DeliverableService);

  readonly selectedFile = signal<File | null>(null);
  readonly isUploading = signal<boolean>(false);
  readonly isSubmitting = signal<boolean>(false);
  readonly uploadProgress = signal<number>(0);

  readonly XIcon = X;
  readonly UploadCloudIcon = UploadCloud;
  readonly FileVideoIcon = FileVideo;
  readonly FileImageIcon = FileImage;
  readonly FileIcon = File;
  readonly CheckCircle2Icon = CheckCircle2;
  readonly Loader2Icon = Loader2;
  readonly SaveIcon = Save;
  readonly AlertCircleIcon = AlertCircle;

  submissionForm: FormGroup = this.fb.group({
    caption: ['']
  });

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile.set(input.files[0]);
    }
  }

  clearSelectedFile(): void {
    this.selectedFile.set(null);
  }

  formatFileSize(bytes: number): string {
    if (bytes >= 1_000_000) {
      return (bytes / 1_000_000).toFixed(1) + ' MB';
    }
    if (bytes >= 1_000) {
      return (bytes / 1_000).toFixed(1) + ' KB';
    }
    return bytes + ' B';
  }

  onCancel(): void {
    this.submissionForm.reset();
    this.selectedFile.set(null);
    this.close.emit();
  }

  onSubmit(): void {
    const file = this.selectedFile();
    const currentDeliverable = this.deliverable;
    if (!file || !currentDeliverable) return;

    this.isUploading.set(true);
    this.uploadProgress.set(20);

    // 1. Get presigned URL
    this.deliverableService
      .getPresignedUploadUrl({
        fileName: file.name,
        contentType: file.type || 'application/octet-stream',
        fileSize: file.size,
        campaignId: currentDeliverable.campaignId,
        deliverableId: currentDeliverable.id
      })
      .subscribe({
        next: presignedRes => {
          this.uploadProgress.set(50);
          const { uploadUrl, objectKey } = presignedRes.data;

          // 2. Upload binary file
          this.deliverableService.uploadFileToPresignedUrl(uploadUrl, file).subscribe({
            next: () => {
              this.uploadProgress.set(80);
              this.isUploading.set(false);
              this.isSubmitting.set(true);

              // 3. Create content submission record
              const caption = this.submissionForm.get('caption')?.value?.trim() || undefined;
              this.deliverableService
                .createContentSubmission(currentDeliverable.id, {
                  mediaObjectKey: objectKey,
                  mediaFileName: file.name,
                  mediaFileSize: file.size,
                  caption
                })
                .subscribe({
                  next: subRes => {
                    this.isSubmitting.set(false);
                    toast.success(`Draft v${subRes.data.versionNumber} submitted successfully`);
                    this.submitted.emit(subRes.data);
                    this.onCancel();
                  },
                  error: err => {
                    this.isSubmitting.set(false);
                    const msg = err.error?.detail || err.error?.message || 'Failed to record submission';
                    toast.error(msg);
                  }
                });
            },
            error: () => {
              this.isUploading.set(false);
              toast.error('Failed to upload file to storage');
            }
          });
        },
        error: err => {
          this.isUploading.set(false);
          const msg = err.error?.detail || err.error?.message || 'Failed to request upload signature';
          toast.error(msg);
        }
      });
  }
}
