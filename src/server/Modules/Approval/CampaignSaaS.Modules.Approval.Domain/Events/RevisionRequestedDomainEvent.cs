namespace CampaignSaaS.Modules.Approval.Domain.Events;

using CampaignSaaS.SharedKernel.Domain;

public record RevisionRequestedDomainEvent(
    Guid ReviewId,
    Guid OrganizationId,
    Guid ContentSubmissionId,
    Guid ReviewerId,
    string? FeedbackNotes,
    DateTimeOffset ReviewedAt) : IDomainEvent;
