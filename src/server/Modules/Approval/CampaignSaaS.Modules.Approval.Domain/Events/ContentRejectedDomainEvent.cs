namespace CampaignSaaS.Modules.Approval.Domain.Events;

using CampaignSaaS.SharedKernel.Domain;

public record ContentRejectedDomainEvent(
    Guid ReviewId,
    Guid OrganizationId,
    Guid ContentSubmissionId,
    Guid ReviewerId,
    string? FeedbackNotes,
    DateTimeOffset ReviewedAt) : IDomainEvent;
