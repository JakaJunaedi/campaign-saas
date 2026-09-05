namespace CampaignSaaS.Modules.Creator.Domain.Events;

using CampaignSaaS.SharedKernel.Domain;

public record CreatorDeletedDomainEvent(
    Guid CreatorId,
    Guid OrganizationId,
    DateTimeOffset OccurredOn) : IDomainEvent;
