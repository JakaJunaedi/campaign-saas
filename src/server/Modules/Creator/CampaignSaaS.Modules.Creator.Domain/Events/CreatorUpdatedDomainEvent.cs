namespace CampaignSaaS.Modules.Creator.Domain.Events;

using CampaignSaaS.SharedKernel.Domain;

public record CreatorUpdatedDomainEvent(
    Guid CreatorId,
    Guid OrganizationId,
    string FullName,
    DateTimeOffset OccurredOn) : IDomainEvent;
