namespace CampaignSaaS.Modules.Approval.Domain.Entities;

using CampaignSaaS.Modules.Approval.Domain.Enums;
using CampaignSaaS.Modules.Approval.Domain.Events;
using CampaignSaaS.SharedKernel.Domain;
using CampaignSaaS.SharedKernel.MultiTenancy;

public class ApprovalReview : AggregateRoot<Guid>, ITenantEntity
{
    public Guid OrganizationId { get; private set; }
    public Guid ContentSubmissionId { get; private set; }
    public Guid ReviewerId { get; private set; }
    public ReviewDecision Decision { get; private set; }
    public string? FeedbackNotes { get; private set; }
    public DateTimeOffset ReviewedAt { get; private set; } = DateTimeOffset.UtcNow;

    private ApprovalReview() { }

    private ApprovalReview(
        Guid id,
        Guid organizationId,
        Guid contentSubmissionId,
        Guid reviewerId,
        ReviewDecision decision,
        string? feedbackNotes) : base(id)
    {
        OrganizationId = organizationId;
        ContentSubmissionId = contentSubmissionId;
        ReviewerId = reviewerId;
        Decision = decision;
        FeedbackNotes = feedbackNotes?.Trim();
        ReviewedAt = DateTimeOffset.UtcNow;
    }

    public static ApprovalReview Create(
        Guid organizationId,
        Guid contentSubmissionId,
        Guid reviewerId,
        ReviewDecision decision,
        string? feedbackNotes)
    {
        var review = new ApprovalReview(
            Guid.NewGuid(),
            organizationId,
            contentSubmissionId,
            reviewerId,
            decision,
            feedbackNotes);

        review.AddDomainEvent(new ReviewSubmittedDomainEvent(
            review.Id,
            review.OrganizationId,
            review.ContentSubmissionId,
            review.ReviewerId,
            review.Decision,
            review.FeedbackNotes,
            review.ReviewedAt));

        switch (decision)
        {
            case ReviewDecision.Approved:
                review.AddDomainEvent(new ContentApprovedDomainEvent(
                    review.Id,
                    review.OrganizationId,
                    review.ContentSubmissionId,
                    review.ReviewerId,
                    review.ReviewedAt));
                break;

            case ReviewDecision.RevisionRequested:
                review.AddDomainEvent(new RevisionRequestedDomainEvent(
                    review.Id,
                    review.OrganizationId,
                    review.ContentSubmissionId,
                    review.ReviewerId,
                    review.FeedbackNotes,
                    review.ReviewedAt));
                break;

            case ReviewDecision.Rejected:
                review.AddDomainEvent(new ContentRejectedDomainEvent(
                    review.Id,
                    review.OrganizationId,
                    review.ContentSubmissionId,
                    review.ReviewerId,
                    review.FeedbackNotes,
                    review.ReviewedAt));
                break;
        }

        return review;
    }
}
