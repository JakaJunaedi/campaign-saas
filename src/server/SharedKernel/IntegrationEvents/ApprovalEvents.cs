namespace CampaignSaaS.SharedKernel.IntegrationEvents;

public record ContentApprovedIntegrationEvent(
    Guid EventId,
    DateTime OccurredOnUtc,
    Guid OrganizationId,
    Guid ContentSubmissionId,
    Guid ReviewId,
    Guid ReviewerId,
    string? FeedbackNotes
) : IIntegrationEvent
{
    public static ContentApprovedIntegrationEvent Create(
        Guid organizationId,
        Guid submissionId,
        Guid reviewId,
        Guid reviewerId,
        string? feedbackNotes) =>
        new(Guid.NewGuid(), DateTime.UtcNow, organizationId, submissionId, reviewId, reviewerId, feedbackNotes);
}

public record RevisionRequestedIntegrationEvent(
    Guid EventId,
    DateTime OccurredOnUtc,
    Guid OrganizationId,
    Guid ContentSubmissionId,
    Guid ReviewId,
    Guid ReviewerId,
    string FeedbackNotes
) : IIntegrationEvent
{
    public static RevisionRequestedIntegrationEvent Create(
        Guid organizationId,
        Guid submissionId,
        Guid reviewId,
        Guid reviewerId,
        string feedbackNotes) =>
        new(Guid.NewGuid(), DateTime.UtcNow, organizationId, submissionId, reviewId, reviewerId, feedbackNotes);
}

public record ContentRejectedIntegrationEvent(
    Guid EventId,
    DateTime OccurredOnUtc,
    Guid OrganizationId,
    Guid ContentSubmissionId,
    Guid ReviewId,
    Guid ReviewerId,
    string FeedbackNotes
) : IIntegrationEvent
{
    public static ContentRejectedIntegrationEvent Create(
        Guid organizationId,
        Guid submissionId,
        Guid reviewId,
        Guid reviewerId,
        string feedbackNotes) =>
        new(Guid.NewGuid(), DateTime.UtcNow, organizationId, submissionId, reviewId, reviewerId, feedbackNotes);
}
