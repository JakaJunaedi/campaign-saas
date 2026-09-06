export type ReviewDecision = 'Approved' | 'RevisionRequested' | 'Rejected';

export interface ApprovalReview {
  id: string;
  organizationId: string;
  contentSubmissionId: string;
  reviewerId: string;
  decision: ReviewDecision | string;
  feedbackNotes?: string;
  reviewedAt: string;
}

export interface SubmitReviewRequest {
  decision: ReviewDecision | string;
  feedbackNotes?: string;
  reviewerId?: string;
}
