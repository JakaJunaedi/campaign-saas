namespace CampaignSaaS.Modules.Deliverable.Domain.Events;

using CampaignSaaS.SharedKernel.Domain;

public record ContentSubmittedDomainEvent(
    Guid DeliverableId,
    Guid OrganizationId,
    Guid SubmissionId,
    int VersionNumber,
    string MediaObjectKey,
    DateTimeOffset SubmittedAt) : IDomainEvent;

