export type DeliverableStatus =
  | 'Draft'
  | 'Submitted'
  | 'Approved'
  | 'RevisionRequested'
  | 'Rejected'
  | 'Published'
  | 'Completed';

export interface Deliverable {
  id: string;
  organizationId: string;
  campaignId: string;
  campaignCreatorId: string;
  title: string;
  platform: string;
  contentType: string;
  briefNotes?: string;
  dueDate: string;
  postingDate?: string;
  status: DeliverableStatus | string;
  liveUrl?: string;
  proofMediaKey?: string;
  latestVersion: number;
  createdAt: string;
  updatedAt?: string;
}

export interface ContentSubmission {
  id: string;
  organizationId: string;
  deliverableId: string;
  versionNumber: number;
  mediaObjectKey: string;
  mediaFileName: string;
  mediaFileSize: number;
  caption?: string;
  downloadUrl?: string;
  submittedAt: string;
}

export interface PresignedUploadUrl {
  uploadUrl: string;
  objectKey: string;
  expiresInMinutes: number;
}

export interface CreateDeliverableRequest {
  campaignCreatorId: string;
  title: string;
  platform: string;
  contentType: string;
  briefNotes?: string;
  dueDate: string;
}

export interface UpdateDeliverableRequest {
  title: string;
  platform: string;
  contentType: string;
  briefNotes?: string;
  dueDate: string;
}

export interface CreateContentSubmissionRequest {
  mediaObjectKey: string;
  mediaFileName: string;
  mediaFileSize: number;
  caption?: string;
}

export interface SubmitPublishProofRequest {
  liveUrl: string;
  proofMediaKey?: string;
  postingDate?: string;
}

export interface GetPresignedUploadUrlRequest {
  fileName: string;
  contentType: string;
  fileSize: number;
  campaignId?: string;
  deliverableId?: string;
}
