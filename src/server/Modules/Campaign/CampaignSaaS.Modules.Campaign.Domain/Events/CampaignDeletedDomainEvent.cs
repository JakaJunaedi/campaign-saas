namespace CampaignSaaS.Modules.Campaign.Domain.Events;

using CampaignSaaS.SharedKernel.Domain;

public record CampaignDeletedDomainEvent(
    Guid CampaignId,
    Guid OrganizationId,
    DateTimeOffset OccurredOn) : IDomainEvent;
