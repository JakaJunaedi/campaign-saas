namespace CampaignSaaS.Modules.Approval.Domain.Events;

using CampaignSaaS.Modules.Approval.Domain.Enums;
using CampaignSaaS.SharedKernel.Domain;

public record ReviewSubmittedDomainEvent(
    Guid ReviewId,
    Guid OrganizationId,
    Guid ContentSubmissionId,
    Guid ReviewerId,
    ReviewDecision Decision,
    string? FeedbackNotes,
    DateTimeOffset ReviewedAt) : IDomainEvent;
