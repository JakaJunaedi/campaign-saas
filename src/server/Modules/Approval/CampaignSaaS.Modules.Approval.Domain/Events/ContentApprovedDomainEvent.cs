namespace CampaignSaaS.Modules.Approval.Domain.Events;

using CampaignSaaS.SharedKernel.Domain;

public record ContentApprovedDomainEvent(
    Guid ReviewId,
    Guid OrganizationId,
    Guid ContentSubmissionId,
    Guid ReviewerId,
    DateTimeOffset ReviewedAt) : IDomainEvent;
