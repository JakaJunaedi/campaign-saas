import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ApiResponse } from '../models/api-response.model';
import { ApprovalReview, SubmitReviewRequest } from '../models/approval.model';

@Injectable({
  providedIn: 'root'
})
export class ApprovalService {
  private http = inject(HttpClient);

  submitReview(submissionId: string, request: SubmitReviewRequest): Observable<ApiResponse<ApprovalReview>> {
    return this.http.post<ApiResponse<ApprovalReview>>(
      `/api/v1/submissions/${submissionId}/reviews`,
      request
    );
  }

  getReviewsBySubmissionId(submissionId: string): Observable<ApiResponse<ApprovalReview[]>> {
    return this.http.get<ApiResponse<ApprovalReview[]>>(
      `/api/v1/submissions/${submissionId}/reviews`
    );
  }

  getReviewById(id: string): Observable<ApiResponse<ApprovalReview>> {
    return this.http.get<ApiResponse<ApprovalReview>>(`/api/v1/reviews/${id}`);
  }
}
