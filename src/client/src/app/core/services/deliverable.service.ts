import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';
import {
  Deliverable,
  ContentSubmission,
  PresignedUploadUrl,
  CreateDeliverableRequest,
  UpdateDeliverableRequest,
  CreateContentSubmissionRequest,
  SubmitPublishProofRequest,
  GetPresignedUploadUrlRequest
} from '../models/deliverable.model';

@Injectable({
  providedIn: 'root'
})
export class DeliverableService {
  private http = inject(HttpClient);

  getDeliverablesByCampaign(campaignId: string): Observable<ApiResponse<Deliverable[]>> {
    return this.http.get<ApiResponse<Deliverable[]>>(`/api/v1/campaigns/${campaignId}/deliverables`);
  }

  getDeliverableById(id: string): Observable<ApiResponse<Deliverable>> {
    return this.http.get<ApiResponse<Deliverable>>(`/api/v1/deliverables/${id}`);
  }

  createDeliverable(campaignId: string, request: CreateDeliverableRequest): Observable<ApiResponse<Deliverable>> {
    return this.http.post<ApiResponse<Deliverable>>(`/api/v1/campaigns/${campaignId}/deliverables`, request);
  }

  updateDeliverable(id: string, request: UpdateDeliverableRequest): Observable<ApiResponse<Deliverable>> {
    return this.http.put<ApiResponse<Deliverable>>(`/api/v1/deliverables/${id}`, request);
  }

  getContentSubmissions(deliverableId: string): Observable<ApiResponse<ContentSubmission[]>> {
    return this.http.get<ApiResponse<ContentSubmission[]>>(`/api/v1/deliverables/${deliverableId}/submissions`);
  }

  createContentSubmission(
    deliverableId: string,
    request: CreateContentSubmissionRequest
  ): Observable<ApiResponse<ContentSubmission>> {
    return this.http.post<ApiResponse<ContentSubmission>>(
      `/api/v1/deliverables/${deliverableId}/submissions`,
      request
    );
  }

  submitPublishProof(
    deliverableId: string,
    request: SubmitPublishProofRequest
  ): Observable<ApiResponse<Deliverable>> {
    return this.http.post<ApiResponse<Deliverable>>(
      `/api/v1/deliverables/${deliverableId}/publish-proof`,
      request
    );
  }

  getPresignedUploadUrl(request: GetPresignedUploadUrlRequest): Observable<ApiResponse<PresignedUploadUrl>> {
    return this.http.post<ApiResponse<PresignedUploadUrl>>('/api/v1/files/presigned-upload', request);
  }

  uploadFileToPresignedUrl(uploadUrl: string, file: File): Observable<any> {
    const headers = new HttpHeaders({
      'Content-Type': file.type || 'application/octet-stream'
    });
    return this.http.put(uploadUrl, file, { headers });
  }
}
